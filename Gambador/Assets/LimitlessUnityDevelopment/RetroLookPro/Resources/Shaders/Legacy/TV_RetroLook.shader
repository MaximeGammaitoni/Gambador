Shader "RetroLookPro/Legacy/TV_RetroLook"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        hardScan("HardScan", Range(-8, -16)) = -8
        hardPix("HardPix", Range(-2, -4)) = -3
        maskDark("maskDark", Range(0, 2)) = 0.5
        maskLight("maskLight", Range(0, 2)) = 1.5
        warp("Warp", Vector) = (0.03125, 0.04166, 0, 0) 
        resScale("ResolutionScale", Range(1, 16)) = 4
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert1
            #pragma fragment frag1
 
            #pragma target 2.0
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
            v2f vert1(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            sampler2D _MainTex;
            float maskDark = 0.5;
            float maskLight = 1.5;
            float hardScan = -8.0;
            float hardPix = -3.0;
            float2 warp = float2(1.0 / 32.0, 1.0 / 24.0);
            float2 res;
            float resScale;
            float3 Fetch(float2 pos, float2 off)
            {
                pos = floor(pos * res + off) / res;
                return tex2Dlod(_MainTex, float4(pos.xy, 0, -16.0));
            }
         
            float2 Dist(float2 pos) { pos = pos*res; return -((pos - floor(pos)) - float2(0.5, 0.5)); }
            float Gaus(float pos, float scale) { return exp2(scale*pos*pos); }
            float3 Horz3(float2 pos, float off)
            {
                float3 b = Fetch(pos, float2(-1.0, off));
                float3 c = Fetch(pos, float2(0.0, off));
                float3 d = Fetch(pos, float2(1.0, off));
                float dst = Dist(pos).x;
                float scale = hardPix;
                float wb = Gaus(dst - 1.0, scale);
                float wc = Gaus(dst + 0.0, scale);
                float wd = Gaus(dst + 1.0, scale);
                return (b*wb + c*wc + d*wd) / (wb + wc + wd);
            }
            float3 Horz5(float2 pos, float off)
            {
                float3 a = Fetch(pos, float2(-2.0, off));
                float3 b = Fetch(pos, float2(-1.0, off));
                float3 c = Fetch(pos, float2(0.0, off));
                float3 d = Fetch(pos, float2(1.0, off));
                float3 e = Fetch(pos, float2(2.0, off));
                float dst = Dist(pos).x;
                float scale = hardPix;
                float wa = Gaus(dst - 2.0, scale);
                float wb = Gaus(dst - 1.0, scale);
                float wc = Gaus(dst + 0.0, scale);
                float wd = Gaus(dst + 1.0, scale);
                float we = Gaus(dst + 2.0, scale);
                return (a*wa + b*wb + c*wc + d*wd + e*we) / (wa + wb + wc + wd + we);
            }
            float Scan(float2 pos, float off)
            {
                float dst = Dist(pos).y;
                return Gaus(dst + off, hardScan);
            }
            
            float3 Tri(float2 pos)
            {
                float3 a = Horz3(pos, -1.0);
                float3 b = Horz5(pos, 0.0);
                float3 c = Horz3(pos, 1.0);
                float wa = Scan(pos, -1.0);
                float wb = Scan(pos, 0.0);
                float wc = Scan(pos, 1.0);
                return a*wa + b*wb + c*wc;
            }

            float2 Warp(float2 pos)
            {
                pos = pos*2.0 - 1.0;
                pos *= float2(1.0 + (pos.y*pos.y)*warp.x, 1.0 + (pos.x*pos.x)*warp.y);
                return pos*0.5 + 0.5;
            }

            float3 Mask(float2 pos)
            {
                pos.x += pos.y*3.0;
                float3 mask = float3(maskDark, maskDark, maskDark);
                pos.x = frac(pos.x / 6.0);
                if (pos.x < 0.333)mask.r = maskLight;
                else if (pos.x < 0.666)mask.g = maskLight;
                else mask.b = maskLight;
                return mask;
            }

            fixed4 frag1(v2f i) : SV_Target
            {
                res = _ScreenParams.xy / resScale;
                float2 fragCoord = i.screenPos.xy * _ScreenParams.xy;
                float4 fragColor = 0;              
#ifdef UNITY_UV_STARTS_AT_TOP
#endif
                float2 pos = Warp(fragCoord.xy / _ScreenParams.xy);
                fragColor.rgb = Tri(pos) *Mask(fragCoord);
                return fragColor;
            }
            ENDCG
        }
    }
}