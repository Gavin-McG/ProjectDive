using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

/// <summary>
/// Renderer Feature for adding lighting using Layers with Illumination Rules / Colors
/// </summary>
public class SpriteLightingFeature : ScriptableRendererFeature
{
    [Serializable]
    public class MaskLayerData
    {
        public enum LightMode { Light, Shadow, Emit }
        
        [SerializeField] public string layerName = "Example Name";
        [SerializeField] public bool active = true;
        [SerializeField] public LayerMask layerMask;
        [SerializeField] public Color color = Color.white;
        [SerializeField] public float intensity = 1;
        [SerializeField] public LightMode mode = LightMode.Light;
    }
    
    [Header("Lighting Details")]
    [SerializeField] bool showInScene = true;
    [SerializeField] Color backgroundColor = Color.white; 
    [SerializeField] float backgroundIntensity = 1.0f;
    [SerializeField] List<MaskLayerData> maskLayers = new List<MaskLayerData>();
    
    [Header("Materials")]
    [SerializeField] Material lightMaterial;
    [SerializeField] Material shadowMaterial;
    [SerializeField] Material emitMaterial;
    [SerializeField] Material gaussianMaterial;
    [SerializeField] Material applyLightMaterial;
    
    [Header("Blur Details")]
    [SerializeField, Min(20)] int verticalResolution = 120;
    [SerializeField] int blurSize = 20;
    [SerializeField] float blurSigma = 10.0f;
    
    private SpriteLightingPass _spriteLightingPass;
    
    private static readonly int LightingColorID = Shader.PropertyToID("_LightingColor");
    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
    private static readonly int TexelSizeID = Shader.PropertyToID("_TexelSize");
    private static readonly int DirectionID = Shader.PropertyToID("_Direction");
    private static readonly int SizeID = Shader.PropertyToID("_Size");
    private static readonly int SigmaID = Shader.PropertyToID("_Sigma");
    
    public override void Create()
    {
        _spriteLightingPass = new SpriteLightingPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,

            //Light Details
            showInScene = showInScene,
            backgroundColor = backgroundColor,
            backgroundIntensity = backgroundIntensity,
            maskLayers = maskLayers,
            
            //Materials
            lightMaterial = lightMaterial,
            shadowMaterial = shadowMaterial,
            emitMaterial = emitMaterial,
            gaussianMaterial = gaussianMaterial,
            applyLightMaterial = applyLightMaterial,
            
            //Blur Settings
            verticalResolution = verticalResolution,
            blurSize = blurSize,
            blurSigma = blurSigma,
        };
    }
    
    private class SpriteLightingPass : ScriptableRenderPass
    {
        public bool showInScene = true;
        public Color backgroundColor;
        public float backgroundIntensity;
        public List<MaskLayerData> maskLayers;
        public Material lightMaterial;
        public Material shadowMaterial;
        public Material emitMaterial;
        public Material gaussianMaterial;
        public Material applyLightMaterial;
        public int verticalResolution;
        public int blurSize;
        public float blurSigma;
        
        static readonly ShaderTagId[] ShaderTags =
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("Universal2D"),
            new ShaderTagId("SRPDefaultUnlit")
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

        Material GetMaterial(MaskLayerData.LightMode mode) => mode switch
        {
            MaskLayerData.LightMode.Light => lightMaterial,
            MaskLayerData.LightMode.Shadow => shadowMaterial,
            MaskLayerData.LightMode.Emit => emitMaterial,
            _ => null
        };

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle mainTex = resourceData.activeColorTexture;

            // Create temp texture
            TextureDesc desc = mainTex.GetDescriptor(renderGraph);
            desc.name = "Shadow Temp Texture";
            float imageAspect = (float)desc.width / desc.height;
            desc.format = GraphicsFormat.R16G16B16A16_SFloat;
            desc.width = (int)(verticalResolution * imageAspect);
            desc.height = verticalResolution;
            desc.depthBufferBits = 0;

            TextureHandle lightMaskTex = renderGraph.CreateTexture(desc);
            TextureHandle blurTempTex = renderGraph.CreateTexture(desc);
            
            //----------------------------------------------------------------------
            //          STEP 0 - Clear texture with background brightness color
            //----------------------------------------------------------------------
            
            using (var builder = renderGraph.AddRasterRenderPass<ClearPassData>("Background Lighting", out var passData))
            {
                passData.color = backgroundColor;
                
                builder.SetRenderAttachment(lightMaskTex, 0);
                builder.SetRenderFunc((ClearPassData data, RasterGraphContext context) =>
                {
                    context.cmd.ClearRenderTarget(RTClearFlags.Color, data.color * backgroundIntensity, 1,0);
                });
            }
            
            //----------------------------------------------------------------------
            //                  STEP 1 - Render Sprites for Light Mask
            //----------------------------------------------------------------------
            
            // Get Rendering Data
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            var sortFlags = cameraData.defaultOpaqueSortFlags;

            // Only show effect in Game view
            CameraType type = cameraData.camera.cameraType;
            if (type != CameraType.Game && (type != CameraType.SceneView || !showInScene))
                return;
            
            // Create the Drawing Settings
            shaderTagIdList.Clear();
            foreach (ShaderTagId sid in ShaderTags)
                shaderTagIdList.Add(sid);
            DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIdList, renderingData, cameraData, lightData, sortFlags);
            
            // Draw each of the layers for the mask
            foreach (var maskLayer in maskLayers)
            {
                if (!maskLayer.active) continue;
                
                // Choose the correct material to use
                Material material = GetMaterial(maskLayer.mode);
                if (material == null)
                {
                    Debug.LogError($"Material for {maskLayer.mode} not found");
                    continue;
                }
                drawSettings.overrideMaterial = material;
                
                //Create the RenderListParams
                FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.transparent, maskLayer.layerMask);
                var param = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);
                
                // Add the Raster Render Pass to Draw the Renderers
                using (var builder = renderGraph.AddRasterRenderPass<LayerPassData>("Lighting Layer - " + maskLayer.layerName, out var passData))
                {
                    passData.rendererList = renderGraph.CreateRendererList(param);
                    passData.color = maskLayer.color;
                    passData.intensity = maskLayer.intensity;
                    
                    if (passData.rendererList.IsValid())
                    {
                        builder.UseRendererList(passData.rendererList);
                        builder.SetRenderAttachment(lightMaskTex, 0);
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
            
            //----------------------------------------------------------------------
            //                  STEP 2 - Perform 2-step Gaussian Blur
            //----------------------------------------------------------------------
            
            // Calculate texel size for Blits
            Vector2 textureSize = new Vector2(desc.width, desc.height);
            Vector2 texelSize = Vector2.one / textureSize;

            // Create Property Blocks for Gaussian Blur steps
            MaterialPropertyBlock verticalMPB = new MaterialPropertyBlock();
            verticalMPB.SetVector(TexelSizeID, texelSize);
            verticalMPB.SetVector(DirectionID, new Vector2(0, 1));
            verticalMPB.SetInt(SizeID, blurSize);
            verticalMPB.SetFloat(SigmaID, blurSigma);

            MaterialPropertyBlock horizontalMPB = new MaterialPropertyBlock();
            horizontalMPB.SetVector(TexelSizeID, texelSize);
            horizontalMPB.SetVector(DirectionID, new Vector2(1, 0));
            horizontalMPB.SetInt(SizeID, blurSize);
            horizontalMPB.SetFloat(SigmaID, blurSigma);

            // Create Blit Params
            var verticalParams = new RenderGraphUtils.BlitMaterialParameters(
                lightMaskTex, blurTempTex,
                gaussianMaterial,
                0,
                verticalMPB
            );

            var horizontalParams = new RenderGraphUtils.BlitMaterialParameters(
                blurTempTex, lightMaskTex,
                gaussianMaterial,
                0,
                horizontalMPB
            );
            
            // Vertical blur pass
            renderGraph.AddBlitPass(verticalParams, "Lighting - Vertical Blur");

            // Horizontal blur pass (writes back to camera color)
            renderGraph.AddBlitPass(horizontalParams, "Lighting - Horizontal Blur");
            
            //----------------------------------------------------------------------
            //    STEP 3 - Use Blurred LightMask to Shadow camera Texture
            //----------------------------------------------------------------------
            
            // Create Blit Params
            var darkenParams = new RenderGraphUtils.BlitMaterialParameters(
                lightMaskTex, mainTex,
                applyLightMaterial,
                0
            );

            renderGraph.AddBlitPass(darkenParams, "Apply Lighting");
        }
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_spriteLightingPass);
    }
}
