Shader "RetroLookPro/Glitch2RetroLook" 
{
        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        sampler2D _TrashTex;
        float _Intensity;

       float4 Frag(VaryingsDefault i) : SV_Target
        {
        float4 glitch = tex2D(_NoiseTex, i.texcoord);
        float thresh = 1.001 - _Intensity * 1.001;
        float w_d = step(thresh, pow(glitch.z, 2.5)); // displacement glitch
        float w_f = step(thresh, pow(glitch.w, 2.5)); // frame glitch
        float w_c = step(thresh, pow(glitch.z, 3.5)); // color glitch
        float2 uv = frac(i.texcoord + glitch.xy * w_d);
        float4 source = tex2D(_MainTex, uv);
        float3 color = lerp(source, tex2D(_TrashTex, uv), w_f).rgb;
        float3 neg = saturate(color.grb + (1 - dot(color, 1)) * 0.1);
        color = lerp(color, neg, w_c);

        return float4(color, source.a);
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