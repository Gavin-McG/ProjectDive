using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Serialization;

/// <summary>
/// Renderer Feature for adding lighting using Layers with Illumination Rules / Colors
/// </summary>
public class SpriteLightingFeature : ScriptableRendererFeature
{
    [Serializable]
    public class LightLayerData
    {
        public enum LightMode { Light, Shadow, Emit }
        
        public string layerName = "Example Name";
        public bool active = true;
        public LayerMask layerMask;
        public Color color = Color.white;
        public float intensity = 1;
        public LightMode mode = LightMode.Light;
    }
    
    [Header("Lighting Details")]
    [SerializeField, Min(20)] int verticalResolution = 120;
    [SerializeField] bool showInScene = true;
    [SerializeField] Color backgroundColor = Color.white; 
    [SerializeField] float backgroundIntensity = 1.0f;
    [SerializeField] List<LightLayerData> lightLayers = new List<LightLayerData>();
    
    [Header("Materials")]
    [SerializeField] Shader lightShader;
    [SerializeField] Shader shadowShader;
    [SerializeField] Shader emitShader;
    [SerializeField] Shader gaussianShader;
    [SerializeField] Shader applyShader;
    
    [Header("Blur Details")]
    [SerializeField] int blurSize = 20;
    [SerializeField] float blurSigma = 10.0f;
    
    [Header("Apply Lighting Details")]
    [SerializeField] public float intensity = 1.0f;
    [SerializeField, Range(0,1)] public float brightDampening = 0.5f;
    
    private CreateTexturesPass createTexturesPass;
    private RenderLayersPass renderLayersPass;
    private BlurLightPass verticalBlurPass;
    private BlurLightPass horizontalBlurPass;
    private ApplyLightingPass applyLightPass;
    
    private class LightingData : ContextItem
    {
        public TextureHandle lightTexture;
        public TextureHandle tempTexture;
        public Vector2 texelSize;
        public CameraType camType;
        public bool showInScene;

        public override void Reset()
        {
            lightTexture = TextureHandle.nullHandle;
            tempTexture = TextureHandle.nullHandle;
            camType = CameraType.Game;
            texelSize = Vector2.zero;
            showInScene = false;
        }

        public bool IsValid()
        {
            return lightTexture.IsValid() && tempTexture.IsValid();
        }

        public bool DisplayPass()
        {
            return IsValid() && DisplayLighting(camType, showInScene);
        }
    }
    
    public static bool DisplayLighting(CameraType type, bool showInScene)
    {
        return type == CameraType.Game || (type == CameraType.SceneView && showInScene);
    }
    
    public override void Create()
    {
        createTexturesPass = new CreateTexturesPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            showInScene = showInScene,
            verticalResolution = verticalResolution,
        };
        renderLayersPass = new RenderLayersPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            backgroundColor = backgroundColor,
            backgroundIntensity = backgroundIntensity,
            lightLayers = lightLayers,
        };
        verticalBlurPass = new BlurLightPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            blurSize = blurSize,
            blurSigma = blurSigma,
            direction = BlurLightPass.Direction.Vertical,
        };
        horizontalBlurPass = new BlurLightPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            blurSize = blurSize,
            blurSigma = blurSigma,
            direction = BlurLightPass.Direction.Horizontal,
        };
        applyLightPass = new ApplyLightingPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            intensity = intensity,
            brightDampening = brightDampening,
        };
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Re-Assign Materials if necessary
        renderLayersPass.UpdateMaterials(lightShader, shadowShader, emitShader);
        verticalBlurPass.UpdateMaterials(gaussianShader);
        horizontalBlurPass.UpdateMaterials(gaussianShader);
        applyLightPass.UpdateMaterials(applyShader);
        
        // Add pass
        renderer.EnqueuePass(createTexturesPass);
        renderer.EnqueuePass(renderLayersPass);
        renderer.EnqueuePass(verticalBlurPass);
        renderer.EnqueuePass(horizontalBlurPass);
        renderer.EnqueuePass(applyLightPass);
    }
    
    #region CREATE_TEXTURES_PASS

    /// <summary>
    /// Simple pass to create the textures and shared data used by following passes in the feature
    /// </summary>
    private class CreateTexturesPass : ScriptableRenderPass
    {
        public bool showInScene = true;
        public int verticalResolution = 120;
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            TextureHandle mainTex = resourceData.activeColorTexture;

            // Create descriptions for HDR textures
            TextureDesc desc = mainTex.GetDescriptor(renderGraph);
            desc.name = "Shadow Temp Texture";
            float imageAspect = (float)desc.width / desc.height;
            desc.format = GraphicsFormat.R16G16B16A16_SFloat;
            desc.width = (int)(verticalResolution * imageAspect);
            desc.height = verticalResolution;
            desc.depthBufferBits = 0;
            
            // Create new Textures
            var textureData = frameData.Create<LightingData>();
            textureData.lightTexture = renderGraph.CreateTexture(desc);
            textureData.tempTexture = renderGraph.CreateTexture(desc);
            
            // Cache Texel Size
            Vector2 textureSize = new Vector2(desc.width, desc.height);
            textureData.texelSize = Vector2.one / textureSize;
            textureData.showInScene = showInScene;
            textureData.camType = cameraData.cameraType;
        }
    }
    
    #endregion
    
    #region RENDER_LAYERS_PASS

    /// <summary>
    /// Render the Background and each of the specified layers to the lightingTexture
    /// </summary>
    private class RenderLayersPass : ScriptableRenderPass
    {
        public Color backgroundColor;
        public float backgroundIntensity;
        public List<LightLayerData> lightLayers;
        
        private Material lightMaterial;
        private Material shadowMaterial;
        private Material emitMaterial;
        
        private static readonly int LightingColorID = Shader.PropertyToID("_LightingColor");
        private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
        
        static readonly ShaderTagId[] ShaderTags =
        {
            new ("UniversalForward"),
            new ("Universal2D"),
            new ("SRPDefaultUnlit")
        };
        private readonly List<ShaderTagId> shaderTagIdList = new ();
        
        class ClearPassData
        {
            public Color color;
        }

        class LayerPassData
        {
            public RendererListHandle rendererList;
            public Color color;
            public float intensity;
        }

        public void UpdateMaterials(Shader lightShader, Shader shadowShader, Shader emitShader)
        {
            if (lightMaterial == null) lightMaterial = new Material(lightShader);
            if (shadowMaterial == null) shadowMaterial = new Material(shadowShader);
            if (emitMaterial == null) emitMaterial = new Material(emitShader);
        }
        
        Material GetMaterial(LightLayerData.LightMode mode) => mode switch
        {
            LightLayerData.LightMode.Light => lightMaterial,
            LightLayerData.LightMode.Shadow => shadowMaterial,
            LightLayerData.LightMode.Emit => emitMaterial,
            _ => null
        };

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Get the required data
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            LightingData lightingData = frameData.Get<LightingData>();
            
            // Check if the pass should Render
            if (!lightingData.DisplayPass()) return;
            
            //----------------------------------------------------------------------
            //         Clear texture with background brightness color
            //----------------------------------------------------------------------
            
            using (var builder = renderGraph.AddRasterRenderPass<ClearPassData>("Background Lighting", out var passData))
            {
                passData.color = backgroundColor;
                
                builder.SetRenderAttachment(lightingData.lightTexture, 0);
                builder.SetRenderFunc((ClearPassData data, RasterGraphContext context) =>
                {
                    context.cmd.ClearRenderTarget(RTClearFlags.Color, data.color * backgroundIntensity, 1,0);
                });
            }
            
            //----------------------------------------------------------------------
            //                 Render Sprites for Light Layers
            //----------------------------------------------------------------------
            
            
            // Create the Drawing Settings
            shaderTagIdList.Clear();
            foreach (ShaderTagId sid in ShaderTags)
                shaderTagIdList.Add(sid);
            
            DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(
                shaderTagIdList, 
                renderingData, 
                cameraData, 
                lightData, 
                cameraData.defaultOpaqueSortFlags);
            
            // Draw each of the layers for the mask
            foreach (var layer in lightLayers)
            {
                if (!layer.active) continue;
                
                // Choose the correct material to use
                Material material = GetMaterial(layer.mode);
                if (material == null)
                {
                    Debug.LogError($"Material for {layer.mode} not found");
                    continue;
                }
                drawSettings.overrideMaterial = material;
                
                //Create the RenderListParams
                FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.transparent, layer.layerMask);
                var param = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);
                
                // Add the Raster Render Pass to Draw the Renderers
                using (var builder = renderGraph.AddRasterRenderPass<LayerPassData>($"Draw Lighting Layer {layer.layerName}", out var passData))
                {
                    passData.rendererList = renderGraph.CreateRendererList(param);
                    passData.color = layer.color;
                    passData.intensity = layer.intensity;
                    
                    if (passData.rendererList.IsValid())
                    {
                        builder.UseRendererList(passData.rendererList);
                        builder.SetRenderAttachment(lightingData.lightTexture, 0);
                        builder.AllowGlobalStateModification(true);
                        
                        builder.SetRenderFunc((LayerPassData data, RasterGraphContext context) =>
                        {
                            context.cmd.SetGlobalColor(LightingColorID, data.color);
                            context.cmd.SetGlobalFloat(IntensityID, data.intensity);
                            context.cmd.DrawRendererList(data.rendererList);
                        });
                    }
                }
            }
        }
    }
    
    #endregion
    
    #region BLUR_LIGHTING_PASS

    /// <summary>
    /// Applies a Gaussian Blur to the lightTexture either vertically or horizontally
    /// </summary>
    private class BlurLightPass : ScriptableRenderPass
    {
        public enum Direction { Vertical, Horizontal }
        
        public int blurSize;
        public float blurSigma;
        public Direction direction;
        
        private Material gaussianMaterial;
        
        private static readonly int TexelSizeID = Shader.PropertyToID("_TexelSize");
        private static readonly int DirectionID = Shader.PropertyToID("_Direction");
        private static readonly int SizeID = Shader.PropertyToID("_Size");
        private static readonly int SigmaID = Shader.PropertyToID("_Sigma");

        public void UpdateMaterials(Shader gaussianShader)
        {
            if (gaussianMaterial == null) gaussianMaterial = new Material(gaussianShader);
            gaussianMaterial.SetFloat(SigmaID, blurSigma);
            gaussianMaterial.SetFloat(SizeID, blurSize);
            if (direction == Direction.Vertical)
                gaussianMaterial.SetVector(DirectionID, new Vector2(0,1));
            else
                gaussianMaterial.SetVector(DirectionID, new Vector2(1,0));
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Get Data
            LightingData lightingData = frameData.Get<LightingData>();
            gaussianMaterial.SetVector(TexelSizeID, lightingData.texelSize);
            
            // Check if the pass should Render
            if (!lightingData.DisplayPass()) return;
            
            // Create Property Blocks for Gaussian Blur steps
            TextureHandle src, dst;
            if (direction == Direction.Vertical)
            {
                // Vertical Blur
                src = lightingData.lightTexture;
                dst = lightingData.tempTexture;
            }
            else
            {
                // Horizontal Blur
                src = lightingData.tempTexture;
                dst = lightingData.lightTexture;
            }

            // Create Blit Params
            var blitParams = new RenderGraphUtils.BlitMaterialParameters(
                src, dst,
                gaussianMaterial,
                0
            );
            
            // Queue blur pass
            renderGraph.AddBlitPass(blitParams, "Directional Lighting Blur");
        }
    }
    
    #endregion
    
    #region APPLY_LIGHTING_PASS

    private class ApplyLightingPass : ScriptableRenderPass
    {
        public float intensity = 1.0f;
        public float brightDampening = 0.5f;
        
        private Material applyMaterial;
        
        private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
        private static readonly int DampenID = Shader.PropertyToID("_BrightDampen");

        public void UpdateMaterials(Shader applyShader)
        {
            if (applyMaterial == null) applyMaterial = new Material(applyShader);
            applyMaterial.SetFloat(IntensityID, intensity);
            applyMaterial.SetFloat(DampenID, brightDampening);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Get Data
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            LightingData lightingData = frameData.Get<LightingData>();
            
            // Check if the pass should Render
            if (!lightingData.DisplayPass()) return;
            
            // Create Blit Params
            var darkenParams = new RenderGraphUtils.BlitMaterialParameters(
                lightingData.lightTexture, resourceData.activeColorTexture,
                applyMaterial,
                0
            );

            renderGraph.AddBlitPass(darkenParams, "Apply Lighting");
        }
    }
    
    #endregion
}
