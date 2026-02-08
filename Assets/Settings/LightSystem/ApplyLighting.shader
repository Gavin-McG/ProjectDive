Shader "LightSystem/ApplyLighting"
{
    Properties
    {
        _Intensity ("Intensity", Float) = 2.0
        _BrightDampen ("Brightness Dampening Factor", Float) = 0.5
    }
    SubShader
    {
        Tags {"RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            Name "Apply Lighting"
            ZWrite Off
            ZTest Always
            Cull Off

            Blend DstColor Zero

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Intensity;
            float _BrightDampen;
            
            half4 Frag (Varyings input) : SV_Target
            {
                half4 lightColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                half3 colorMultiplier = lightColor.rgb * _Intensity;

                return half4(colorMultiplier, 1.0);
            }
            ENDHLSL
        }
    }
}