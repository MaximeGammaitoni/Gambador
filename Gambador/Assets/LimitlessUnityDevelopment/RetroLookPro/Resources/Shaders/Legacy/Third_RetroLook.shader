Shader "RetroLookPro/Legacy/Third_RetroLook" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {} 		
		_LastTex ("Render Input", 2D) = "white" {} 		
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
				sampler2D _LastTex;
				sampler2D _FeedbackTex;				
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
				   float4 texcoord2 : TEXCOORD1;   	
				   float4 texcoord3 : TEXCOORD2;   					   
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
				
				
				float feedbackAmount = 0.0;
				float feedbackFade = 0.0;
				float feedbackThresh = 5.0;
				half3 feedbackColor = half3(1.0,0.5,0.0); //


				half3 bm_screen(half3 a, half3 b){ 	return 1.0- (1.0-a)*(1.0-b); }

				half4 frag( v2f i ) : COLOR {
					
					float2 p = i.uvn;
					float one_x = 1.0/_ScreenParams.x;
					
					half3 fc =  tex2D( _MainTex, i.uvn).rgb; 		
					half3 fl =  tex2D( _LastTex, i.uvn).rgb; 		
					float diff = abs(fl.x-fc.x + fl.y-fc.y + fl.z-fc.z)/3.0; 
					if(diff<feedbackThresh) diff = 0.0;

					half3 fbn = fc*diff*feedbackAmount; 			
					

					
					half3 fbb = half3(0.0, 0.0, 0.0); 
								
					fbb = ( 
						tex2D( _FeedbackTex, i.uvn).rgb + 
						tex2D( _FeedbackTex, i.uvn + float2(one_x, 0.0)).rgb + 
						tex2D( _FeedbackTex, i.uvn - float2(one_x, 0.0)).rgb
					) / 3.0;
					fbb *= feedbackFade;
					fbn = bm_screen(fbn, fbb); 
				   return half4(fbn*feedbackColor, 1.0); 
				}
			ENDCG
		}
	}
}
