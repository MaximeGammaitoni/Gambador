Shader "RetroLookPro/Glitch3" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        sampler2D _MainTex;

        float speed;
        float density; 
        float maxDisplace; 

        inline float rand(float2 seed)
        {
            return frac(sin(dot(seed * floor(_Time.y * speed), float2(127.1, 311.7))) * 43758.5453123);
        }

         inline float rand(float seed)
         {
            return rand(float2(seed, 1.0));
         }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float2 rblock = rand(floor(i.texcoord * density));
            float displaceNoise = pow(rblock.x, 8.0) * pow(rblock.x, 3.0) - pow(rand(7.2341), 17.0) * maxDisplace;

            float r = tex2D(_MainTex, i.texcoord).r;
            float g = tex2D(_MainTex, i.texcoord + half2(displaceNoise * 0.05 * rand(7.0), 0.0)).g;
            float b = tex2D(_MainTex, i.texcoord - half2(displaceNoise * 0.05 * rand(13.0), 0.0)).b;

            return half4(r, g, b, 1.0);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}