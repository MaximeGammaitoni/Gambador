Shader "RetroLookPro/Legacy/Forth_RetroLook" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}		
		_FeedbackTex ("Render Input", 2D) = "white" {}	
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#include "UnityCG.cginc"
				#pragma glsl
				sampler2D _MainTex;
				sampler2D _FeedbackTex;
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
				   float4 texcoord2 : TEXCOORD1;   	
				};
				struct v2f {
			   	float2 uv : TEXCOORD2; 
			   	float4 uvn : TEXCOORD1;			   	
			   	float4 pos : SV_POSITION; 
				};
				v2f vert (appdata i){
			   	v2f o;
			   	o.pos = UnityObjectToClipPos( i.vertex );
			   	o.uv = o.pos.xy/o.pos.w;
			   	o.uvn = float4( i.texcoord.xy, 0, 0);			   	
			   	return o;
				}
				float feedbackAmp = 1.0;
				half3 bm_screen(half3 a, half3 b){ 	return 1.0- (1.0-a)*(1.0-b); }
				half4 frag( v2f i ) : COLOR {
					float2 p = i.uvn;
					half3 col = tex2D( _MainTex, i.uvn).rgb; 		
					half3 fbb = tex2D( _FeedbackTex, i.uvn).rgb; 
				   col = bm_screen(col, fbb*feedbackAmp);
				   return half4(col, 1.0); 
				}
			ENDCG
		}
	}
}