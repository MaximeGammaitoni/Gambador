Shader "RetroLookPro/Legacy/NegativeFilterRetroLook" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _MainTex2 ("Base (RGB)", 2D) = "white" {}
    }
    SubShader 
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma target 3.0
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _MainTex2;
            uniform float T;
            uniform float Luminosity;
            uniform float Vignette;
            uniform float Negative;
            uniform float2 _MainTex_TexelSize;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
            };   

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            float3 linearLight( float3 s, float3 d )
            {
                return 2.0 * s + d - 1.0 * Luminosity;
            }

            half4 _MainTex_ST;

            float4 frag(v2f i) : COLOR
            {
                float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
                float2 uv = uvst.xy;
                float t = float(int(T * 15.0));
                float2 suv = uv ;
                float3 col = tex2D(_MainTex,suv).rgb;
                col=lerp(col,1-col,Negative);
                #if UNITY_UV_STARTS_AT_TOP
                    if (_MainTex_TexelSize.y < 0)
                        uv = 1-uv;
                #endif
                suv = uv ;
                uv.y=suv.y;
                float3 oldfilm = tex2D(_MainTex2,uv).rgb;
                uv = uvst.xy;
                col*=pow(16.0 * uv.x * (1.0-uv.x) * uv.y * (1.0-uv.y), 0.4)*1+Vignette;
                col = dot( float3(0.2126, 0.7152, 0.0722), col);
                col=linearLight(oldfilm,col);
                return float4(col, 1.0);
            }
        ENDCG
        }
    }
}