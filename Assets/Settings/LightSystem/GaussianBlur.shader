Shader "LightSystem/GaussianBlur"
{
    SubShader
    {
        Tags {"RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            Name "Gaussian Blur"
            ZWrite Off
            ZTest Always
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float2 _TexelSize;
            float2 _Direction;
            float _Sigma;
            int _Size;

            float Gaussian(float x, float sigma)
            {
                return exp(-(x * x) / (2.0 * sigma * sigma));
            }

            half4 Frag (Varyings input) : SV_Target0
            {
                float2 offset = _Direction * _TexelSize;
                float sigma = max(_Sigma, 0.0001);

                half4 color = 0;
                float weightSum = 0;

                for (int k = -_Size; k <= _Size; k++)
                {
                    float w = Gaussian(k, sigma);
                    weightSum += w;

                    float2 uv = input.texcoord + offset * k;
                    if (all(saturate(uv) == uv))
                    {
                        color += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv) * w;
                    }
                }

                return color / weightSum;
            }
            ENDHLSL
        }
    }
}