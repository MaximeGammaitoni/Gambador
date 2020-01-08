Shader "RetroLookPro/VHSScanlines_RLPro" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        float4 _ScanLinesColor; 
        float _ScanLines;
        sampler2D _MainTex;
        float speed;
        float fade;
        float _OffsetDistortion;
        float sferical;
        float barrel;
        float scale;
        float _OffsetColor;
        float2 _OffsetColorAngle;

        float2 FisheyeDistortion(float2 coord, float spherical, float barrel, float scale) 
        {
            float2 h = coord.xy - float2(0.5, 0.5);
            float r2 = dot(h, h);
            float f = 1.0 + r2 * (spherical + barrel * sqrt(r2));
            return f * scale * h + 0.5;
        }

        float4 FragH(VaryingsDefault i) : SV_Target
        {
            float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
            half4 color = tex2D(_MainTex, i.texcoord);
	        float lineSize = _ScreenParams.y * 0.005;
	        float displacement = ((_Time.y * 1000) * speed) % _ScreenParams.y;
	        float ps;
            ps = displacement + (coord.y * _ScreenParams.y / i.vertex.w);
            float sc = i.texcoord.y;
            float4 result;
            result = ((int)(ps / floor(_ScanLines*lineSize)) % 2 == 0) ? color :  _ScanLinesColor ;
            result+= color * sc;
            return lerp(color,result,fade);
        }

        float4 FragHD(VaryingsDefault i) : SV_Target
        {
            float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
            half4 color = tex2D(_MainTex, i.texcoord);
	        float lineSize = _ScreenParams.y * 0.005;
	        float displacement = ((_Time.y * 1000) * speed) % _ScreenParams.y;
	        float ps;
            i.texcoord.y = frac(i.texcoord.y + cos((coord.x + _CosTime.y) * 100) / _OffsetDistortion);
            ps = displacement + (i.texcoord.y * _ScreenParams.y / i.vertex.w);
            float sc = i.texcoord.y;
            float4 result;
            result = ((int)(ps / floor(_ScanLines*lineSize)) % 2 == 0) ? color :  _ScanLinesColor ;
            result+= color * sc;
            return lerp(color,result,fade);
        }

        float4 FragV(VaryingsDefault i) : SV_Target
        {
            float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
            half4 color = tex2D(_MainTex, i.texcoord);
	        float lineSize = _ScreenParams.y * 0.005;
	        float displacement = ((_Time.y * 1000) * speed) % _ScreenParams.y;
	        float ps;
            ps = displacement + (coord.x * _ScreenParams.x / i.vertex.w);
            float sc = i.texcoord.y;
            float4 result;
            result = ((int)(ps / floor(_ScanLines*lineSize)) % 2 == 0) ? color :  _ScanLinesColor ;
            result+= color * sc;
            return lerp(color,result,fade);
        }

        float4 FragVD(VaryingsDefault i) : SV_Target
        {
            float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
            half4 color = tex2D(_MainTex, i.texcoord);
	        float lineSize = _ScreenParams.y * 0.005;
	        float displacement = ((_Time.y * 1000) * speed) % _ScreenParams.y;
	        float ps;
            i.texcoord.x = frac(i.texcoord.x + cos((coord.y + _CosTime.y) * 100) / _OffsetDistortion);
            ps = displacement + (i.texcoord.x * _ScreenParams.x / i.vertex.w);
            float sc = i.texcoord.y;
            float4 result;
            result = ((int)(ps / floor(_ScanLines*lineSize)) % 2 == 0) ? color :  _ScanLinesColor ;
            result+= color * sc;
            return lerp(color,result,fade);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragH

            ENDHLSL
        }
                Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragHD

            ENDHLSL
        }
                Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragV

            ENDHLSL
        }
                Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragVD

            ENDHLSL
        }
    }
}