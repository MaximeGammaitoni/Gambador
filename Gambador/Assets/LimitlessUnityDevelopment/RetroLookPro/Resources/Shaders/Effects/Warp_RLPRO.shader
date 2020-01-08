Shader "RetroLookPro/Warp_RLPro" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        sampler2D _MainTex;
        float2 warp = float2(1.0 / 32.0, 1.0 / 24.0);
        float scale;
        float fade;

            float2 Warp(float2 pos)
            {
                float2 h = pos - float2(0.5, 0.5);
                float r2 = dot(h, h);
                float f = 1.0 + r2 * (warp.x + warp.y * sqrt(r2));
                return f * scale * h + 0.5;
            }
                float2 Warp1(float2 pos)
            {
                pos = pos*2.0 - 1.0;
                pos *= float2(1.0 + (pos.y*pos.y)*warp.x, 1.0 + (pos.x*pos.x)*warp.y);
                return pos*scale + 0.5;
            }

        float4 Frag0(VaryingsDefault i) : SV_Target
        {
            float4 col = tex2D(_MainTex,i.texcoord);
             float2 fragCoord = i.texcoord.xy * _ScreenParams.xy;
                       
            float2 pos = Warp1(fragCoord.xy / _ScreenParams.xy);
            float4 col2 = tex2D(_MainTex,pos);
            return lerp(col,col2,fade);
        }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 col = tex2D(_MainTex,i.texcoord);
            float2 fragCoord = i.texcoord.xy * _ScreenParams.xy;
            float2 pos = Warp(fragCoord.xy / _ScreenParams.xy);
            float4 col2 = tex2D(_MainTex,pos);
            return lerp(col,col2,fade);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag0

            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}