Shader "RetroLookPro/Legacy/ResetRetroLook" {

	Properties {
		_MainTex ("Render Input", 2D) = "white" {} 		
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
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
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
				half4 frag( v2f i ) : COLOR {
				   return half4(0.0, 0.0, 0.0, 1.0); 
				}
			ENDCG
		}
	}
}
