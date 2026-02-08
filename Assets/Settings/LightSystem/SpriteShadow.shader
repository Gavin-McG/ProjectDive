Shader "LightSystem/SpriteShadow"
{
   Properties
    {
        [MainTexture][NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            Name "Sprite Shadow"
            ZWrite Off
            ZTest Always
            Cull Off

            Blend Zero SrcColor

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            half4 _LightingColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.texcoord = v.uv;
                return o;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 sampleColor = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.texcoord);

                if (sampleColor.a == 0) discard;
                
                return half4(_LightingColor.rgb, sampleColor.a);
            }
            ENDHLSL
        }
    }
}