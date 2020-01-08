Shader "RetroLookPro/BleedEffect" 
{
    HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		
        sampler2D _MainTex;
		sampler2D _CurvesTex;		
		float time_ = 0.0;
		#pragma shader_feature VHS_BLEED_ON
		#pragma shader_feature VHS_OLD_THREE_PHASE
		#pragma shader_feature VHS_THREE_PHASE
		#pragma shader_feature VHS_TWO_PHASE
		float screenLinesNum = 240.0;
		#pragma shader_feature VHS_CUSTOM_BLEED_ON
		#pragma shader_feature VHS_DEBUG_BLEEDING_ON
		int bleedLength = 21;				
		float4 curvesOffest = float4(0.0, 0.0, 0.0, 0.0); 
		float bleedAmount = 1.0; 
		#define PI 3.14159265359
		half3 rgb2yiq(half3 c)
		{   
			return half3(
						(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
						(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
						(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
						);
		};
		half3 yiq2rgb(half3 c)
		{				
			return half3(
						(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
						( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
						(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
						);
		};
		half3 t2d(float2 p)
		{
			half3 col = tex2D (_MainTex, p ).rgb;

			return rgb2yiq( col );
		}
		#define fixCoord (p - float2( 0.5 * ONE_X, 0.0))
		#define fetch_offset(offset, one_x) t2d(fixCoord + float2( (offset) * (ONE_X), 0.0));
		#define get_t2d(offset, one_x) tex2D(_MainTex, p + (offset)*(one_x)).rgb;
		
		float4 FragNTSCOld3Phase(VaryingsDefault i) : COLOR
        {	
			float2 p = i.texcoordStereo;
			float ONE_X = 1.0 / _ScreenParams.x;
    		ONE_X *= bleedAmount; 			     			      
			bleedLength = 25;
			float maxTexLength = 50.0;
			float luma_filter[25]; luma_filter[0]=-0.000071070; luma_filter[1]=-0.000032816; luma_filter[2]=0.000128784; luma_filter[3]=0.000134711; luma_filter[4]=-0.000226705; luma_filter[5]=-0.000777988; luma_filter[6]=-0.000997809; luma_filter[7]=-0.000522802; luma_filter[8]=0.000344691; luma_filter[9]=0.000768930; luma_filter[10]=0.000275591; luma_filter[11]=-0.000373434; luma_filter[12]=0.000522796; luma_filter[13]=0.003813817; luma_filter[14]=0.007502825; luma_filter[15]=0.006786001; luma_filter[16]=-0.002636726; luma_filter[17]=-0.019461182; luma_filter[18]=-0.033792479; luma_filter[19]=-0.029921972; luma_filter[20]=0.005032552; luma_filter[21]=0.071226466; luma_filter[22]=0.151755921; luma_filter[23]=0.218166470; luma_filter[24]=0.243902439;
			float chroma_filter[24+1]; chroma_filter[0]=0.001845562; chroma_filter[1]=0.002381606; chroma_filter[2]=0.003040177; chroma_filter[3]=0.003838976; chroma_filter[4]=0.004795341; chroma_filter[5]=0.005925312; chroma_filter[6]=0.007242534; chroma_filter[7]=0.008757043; chroma_filter[8]=0.010473987; chroma_filter[9]=0.012392365; chroma_filter[10]=0.014503872; chroma_filter[11]=0.016791957; chroma_filter[12]=0.019231195; chroma_filter[13]=0.021787070; chroma_filter[14]=0.024416251; chroma_filter[15]=0.027067414; chroma_filter[16]=0.029682613; chroma_filter[17]=0.032199202; chroma_filter[18]=0.034552198; chroma_filter[19]=0.036677005; chroma_filter[20]=0.038512317; chroma_filter[21]=0.040003044; chroma_filter[22]=0.041103048; chroma_filter[23]=0.041777517; chroma_filter[24]=0.042004791;
	    	half3 signal = half3(0.0,0.0,0.0);						
			half3 norm = 	half3(0.0,0.0,0.0);
		    half3 adj = 	half3(0.0,0.0,0.0);
		    int taps = bleedLength-4;
			for (int ii = 0; ii < taps % 1023; ii++)
			{
				float offset = float(ii);
				half3 sums = fetch_offset(offset - float(taps), ONE_X) + fetch_offset(float(taps) - offset, ONE_X) ;
				adj = half3(luma_filter[ii+3], chroma_filter[ii], chroma_filter[ii]);
				signal += sums * adj;
				norm += adj;					        
			}
			adj = half3(luma_filter[taps], chroma_filter[taps], chroma_filter[taps]);
			half3 col23 = tex2D (_MainTex, fixCoord ).rgb;
			signal += rgb2yiq( col23 ) * adj;
			norm += adj;
		    signal = signal / norm;			
			float3 rgb = yiq2rgb(signal);
							
				#if VHS_DEBUG_BLEEDING_ON
					float one_x = 1.0/float(bleedLength);
					float one_y = 1./_ScreenParams.y;
					float d = one_y*0.5; 
					p.y = p.y*2.0 - 1.0 ;
					p.y /= 4.0;
					p.x = 1.0 - p.x;
					float px = i.texcoordStereo.x;
					px = px*(bleedLength/maxTexLength)+( (maxTexLength-bleedLength)/maxTexLength);					  	
					half3 colDebug = half3(0.0, 0.0, 0.0);
			        int i_ =  floor( (1.0-px)*maxTexLength);
			        if(p.y<0.0+d && p.y>0.0-d) colDebug.rgb += half3(0.3, 0.3, 0.3); 				      	
				    if(i_<25 && i_>=0)
					{ 
					    if(p.y<luma_filter[i_]+d && p.y>luma_filter[i_]-d) colDebug += half3(0.5, 0.0, 0.0);
					    if(p.y<luma_filter[i_]+d*4.0 && p.y>luma_filter[i_]-d*4.0) rgb /=4.0;
					    if(p.y<chroma_filter[i_]+d && p.y>chroma_filter[i_]-d) colDebug += half3(0.5, 0.0, 0.5);                    
					    if(p.y<chroma_filter[i_]+d*4.0 && p.y>chroma_filter[i_]-d*4.0) rgb /=4.0;                   
				    }
				    return half4(rgb+colDebug, 1.0); 
			    #endif
			return half4(rgb, 1.0); 
        }

		float4 FragNTSC3Phase(VaryingsDefault i) : COLOR
        {	
			float2 p = i.texcoordStereo;
			float ONE_X = 1.0 / _ScreenParams.x;
    		ONE_X *= bleedAmount; 			    
			bleedLength = 25;
			float maxTexLength = 50.0;
			float luma_filter[25]; luma_filter[0]=-0.000012020; luma_filter[1]=-0.000022146; luma_filter[2]=-0.000013155; luma_filter[3]=-0.000012020; luma_filter[4]=-0.000049979; luma_filter[5]=-0.000113940; luma_filter[6]=-0.000122150; luma_filter[7]=-0.000005612; luma_filter[8]=0.000170516; luma_filter[9]=0.000237199; luma_filter[10]=0.000169640; luma_filter[11]=0.000285688; luma_filter[12]=0.000984574; luma_filter[13]=0.002018683; luma_filter[14]=0.002002275; luma_filter[15]=-0.000909882; luma_filter[16]=-0.007049081; luma_filter[17]=-0.013222860; luma_filter[18]=-0.012606931; luma_filter[19]=0.002460860; luma_filter[20]=0.035868225; luma_filter[21]=0.084016453; luma_filter[22]=0.135563500; luma_filter[23]=0.175261268; luma_filter[24]=0.190176552;
			float chroma_filter[25]; chroma_filter[0]=-0.000118847; chroma_filter[1]=-0.000271306; chroma_filter[2]=-0.000502642; chroma_filter[3]=-0.000930833; chroma_filter[4]=-0.001451013; chroma_filter[5]=-0.002064744; chroma_filter[6]=-0.002700432; chroma_filter[7]=-0.003241276; chroma_filter[8]=-0.003524948; chroma_filter[9]=-0.003350284; chroma_filter[10]=-0.002491729; chroma_filter[11]=-0.000721149; chroma_filter[12]=0.002164659; chroma_filter[13]=0.006313635; chroma_filter[14]=0.011789103; chroma_filter[15]=0.018545660; chroma_filter[16]=0.026414396; chroma_filter[17]=0.035100710; chroma_filter[18]=0.044196567; chroma_filter[19]=0.053207202; chroma_filter[20]=0.061590275; chroma_filter[21]=0.068803602; chroma_filter[22]=0.074356193; chroma_filter[23]=0.077856564; chroma_filter[24]=0.079052396;
			half3 signal = half3(0.0,0.0,0.0);						
			half3 norm = 	half3(0.0,0.0,0.0);
		    half3 adj = 	half3(0.0,0.0,0.0);
		    int taps = bleedLength-4;
			for (int ii = 0; ii < taps % 1023; ii++)
			{
				float offset = float(ii);
				half3 sums = 	fetch_offset(offset - float(taps), ONE_X) +	fetch_offset(float(taps) - offset, ONE_X) ;
				adj = half3(luma_filter[ii+3], chroma_filter[ii], chroma_filter[ii]);
				signal += sums * adj;
				norm += adj;					        
			}
			adj = half3(luma_filter[taps], chroma_filter[taps], chroma_filter[taps]);
			half3 col23 = tex2D (_MainTex, fixCoord ).rgb;
			signal += rgb2yiq( col23 ) * adj;
			norm += adj;
		    signal = signal / norm;				    
			float3 rgb = yiq2rgb(signal);

				#if VHS_DEBUG_BLEEDING_ON
					float one_x = 1.0/float(bleedLength);
					float one_y = 1./_ScreenParams.y;
					float d = one_y*0.5; 
					p.y = p.y*2.0 - 1.0 ;
					p.y /= 4.0;
					p.x = 1.0 - p.x;
					float px = i.texcoordStereo.x;
					px = px*(bleedLength/maxTexLength)+( (maxTexLength-bleedLength)/maxTexLength);					  	
					half3 colDebug = half3(0.0, 0.0, 0.0);
			        int i_ =  floor( (1.0-px)*maxTexLength);
			        if(p.y<0.0+d && p.y>0.0-d) colDebug.rgb += half3(0.3, 0.3, 0.3); 				      	
				    if(i_<25 && i_>=0)
					{ 
					    if(p.y<luma_filter[i_]+d && p.y>luma_filter[i_]-d) colDebug += half3(0.5, 0.0, 0.0);
					    if(p.y<luma_filter[i_]+d*4.0 && p.y>luma_filter[i_]-d*4.0) rgb /=4.0;
					    if(p.y<chroma_filter[i_]+d && p.y>chroma_filter[i_]-d) colDebug += half3(0.5, 0.0, 0.5);                    
					    if(p.y<chroma_filter[i_]+d*4.0 && p.y>chroma_filter[i_]-d*4.0) rgb /=4.0;                   
				    }
				    return half4(rgb+colDebug, 1.0); 
			    #endif					
			return half4(rgb, 1.0); 
        }

		float4 FragNTSC2Phase(VaryingsDefault i) : COLOR
        {	
			float2 p = i.texcoordStereo;
			float ONE_X = 1.0 / _ScreenParams.x;
    		ONE_X *= bleedAmount; 			    
			bleedLength = 33;
			float maxTexLength = 50.0;
			float luma_filter[33]; luma_filter[0]=-0.000174844; luma_filter[1]=-0.000205844; luma_filter[2]=-0.000149453; luma_filter[3]=-0.000051693; luma_filter[4]=0.000000000; luma_filter[5]=-0.000066171; luma_filter[6]=-0.000245058; luma_filter[7]=-0.000432928; luma_filter[8]=-0.000472644; luma_filter[9]=-0.000252236; luma_filter[10]=0.000198929; luma_filter[11]=0.000687058; luma_filter[12]=0.000944112; luma_filter[13]=0.000803467; luma_filter[14]=0.000363199; luma_filter[15]=0.000013422; luma_filter[16]=0.000253402; luma_filter[17]=0.001339461; luma_filter[18]=0.002932972; luma_filter[19]=0.003983485; luma_filter[20]=0.003026683; luma_filter[21]=-0.001102056; luma_filter[22]=-0.008373026; luma_filter[23]=-0.016897700; luma_filter[24]=-0.022914480; luma_filter[25]=-0.021642347; luma_filter[26]=-0.008863273; luma_filter[27]=0.017271957; luma_filter[28]=0.054921920; luma_filter[29]=0.098342579; luma_filter[30]=0.139044281; luma_filter[31]=0.168055832; luma_filter[32]=0.178571429;
	  		float chroma_filter[33]; chroma_filter[0]=0.001384762; chroma_filter[1]=0.001678312; chroma_filter[2]=0.002021715; chroma_filter[3]=0.002420562; chroma_filter[4]=0.002880460; chroma_filter[5]=0.003406879; chroma_filter[6]=0.004004985; chroma_filter[7]=0.004679445; chroma_filter[8]=0.005434218; chroma_filter[9]=0.006272332; chroma_filter[10]=0.007195654; chroma_filter[11]=0.008204665; chroma_filter[12]=0.009298238; chroma_filter[13]=0.010473450; chroma_filter[14]=0.011725413; chroma_filter[15]=0.013047155; chroma_filter[16]=0.014429548; chroma_filter[17]=0.015861306; chroma_filter[18]=0.017329037; chroma_filter[19]=0.018817382; chroma_filter[20]=0.020309220; chroma_filter[21]=0.021785952; chroma_filter[22]=0.023227857; chroma_filter[23]=0.024614500; chroma_filter[24]=0.025925203; chroma_filter[25]=0.027139546; chroma_filter[26]=0.028237893; chroma_filter[27]=0.029201910; chroma_filter[28]=0.030015081; chroma_filter[29]=0.030663170; chroma_filter[30]=0.031134640; chroma_filter[31]=0.031420995; chroma_filter[32]=0.031517031;
			half3 signal = half3(0.0,0.0,0.0);						
			half3 norm = 	half3(0.0,0.0,0.0);
		    half3 adj = 	half3(0.0,0.0,0.0);
		    int taps = bleedLength-4;
			for (int ii = 0; ii < taps % 1023; ii++)
			{
			    float offset = float(ii);
			    half3 sums = fetch_offset(offset - float(taps), ONE_X) + fetch_offset(float(taps) - offset, ONE_X) ;
				adj = half3(luma_filter[ii+3], chroma_filter[ii], chroma_filter[ii]);
				signal += sums * adj;
				norm += adj;	        
			}
			adj = half3(luma_filter[taps], chroma_filter[taps], chroma_filter[taps]);
			half3 col23 = tex2D (_MainTex, fixCoord ).rgb;
			signal += rgb2yiq( col23 ) * adj;
			norm += adj;
		    signal = signal / norm;				    
			float3 rgb = yiq2rgb(signal);	
							   
				#if VHS_DEBUG_BLEEDING_ON

					float one_x = 1.0/float(bleedLength);
					float one_y = 1./_ScreenParams.y;
					float d = one_y*0.5; 
					p.y = p.y*2.0 - 1.0 ;
					p.y /= 4.0;
					p.x = 1.0 - p.x;
					float px = i.texcoordStereo.x;
					px = px*(bleedLength/maxTexLength)+( (maxTexLength-bleedLength)/maxTexLength);					  	
					half3 colDebug = half3(0.0, 0.0, 0.0);
			        int i_ =  floor( (1.0-px)*maxTexLength);
			        if(p.y<0.0+d && p.y>0.0-d) colDebug.rgb += half3(0.3, 0.3, 0.3); 				      	
				    if(i_<25 && i_>=0)
					{ 
					    if(p.y<luma_filter[i_]+d && p.y>luma_filter[i_]-d) colDebug += half3(0.5, 0.0, 0.0);
					    if(p.y<luma_filter[i_]+d*4.0 && p.y>luma_filter[i_]-d*4.0) rgb /=4.0;
					    if(p.y<chroma_filter[i_]+d && p.y>chroma_filter[i_]-d) colDebug += half3(0.5, 0.0, 0.5);                    
					    if(p.y<chroma_filter[i_]+d*4.0 && p.y>chroma_filter[i_]-d*4.0) rgb /=4.0;                   
				    }
				    return half4(rgb+colDebug, 1.0); 
			    #endif

			return half4(rgb, 1.0); 
        }
		
		float4 FragCustomBleed(VaryingsDefault i) : COLOR
        {	
			float2 p = i.texcoordStereo;
			float ONE_X = 1.0 / _ScreenParams.x;
    		ONE_X *= bleedAmount; 			    
			float maxTexLength = 50.0;
	    	half3 signal = half3(0.0,0.0,0.0);					
			half3 norm = half3(0.0,0.0,0.0);
		    half3 adj = half3(0.0,0.0,0.0);
		    int taps = bleedLength-4;
			for (int ii = 0; ii < taps % 1023; ii++)
			{
			    float offset = float(ii);
				half3 sums = fetch_offset(offset - float(taps), ONE_X) + fetch_offset(float(taps) - offset, ONE_X) ;
				half3 val = tex2D(_CurvesTex, float2(offset/maxTexLength, 0.0) ).xyz - curvesOffest.xyz;			         
				half3 val3 = tex2D(_CurvesTex, float2( (offset+3.0)/maxTexLength, 0.0) ).xyz - curvesOffest.xyz;
				adj = half3(val3.x, val.y, val.z);
				signal += sums * adj;
				norm += adj;					        
			}
			adj = tex2D(_CurvesTex, float2(taps/maxTexLength, 0.0) ).xyz - curvesOffest.xyz;
			half3 col23 = tex2D (_MainTex, fixCoord ).rgb;
			signal += rgb2yiq( col23 ) * adj;
			norm += adj;
		    signal = signal / norm;				    
			float3 rgb = yiq2rgb(signal);	

			#if VHS_DEBUG_BLEEDING_ON

				float one_x = 1.0/float(bleedLength);
				float one_y = 1./_ScreenParams.y;
				float d = one_y*0.5; 
				p.y = p.y*2.0 - 1.0 ;
				p.y /= 4.0;
				p.x = 1.0 - p.x;
				float px = i.texcoordStereo.x;
				px = px*(bleedLength/maxTexLength)+( (maxTexLength-bleedLength)/maxTexLength);					  	
				half3 colDebug = half3(0.0, 0.0, 0.0);
			    int i_ =  floor( (1.0-px)*maxTexLength);
			    if(p.y<0.0+d && p.y>0.0-d) colDebug.rgb += half3(0.3, 0.3, 0.3); 
			      	
						if(i_<=bleedLength && i_>=0)
						{
				        	float3 crv = tex2D (_CurvesTex, float2(float(i_)/maxTexLength, 0.0) ).xyz;
					    	crv -= curvesOffest;
							if(p.y<crv.x+d && p.y>crv.x-d) colDebug += half3(0.5,  0.0, 0.0);                  
							if(p.y<crv.x+d*4.0 && p.y>crv.x-d*4.0) rgb /=4.0;                  
							if(p.y<crv.y+d && p.y>crv.y-d) colDebug += half3(0.5,  0.0, 0.5);                    
							if(p.y<crv.y+d*4.0 && p.y>crv.y-d*4.0) rgb /=4.0;                  
							if(p.y<crv.z+d && p.y>crv.z-d) colDebug += half3(0.5,  1.0, 0.5);                    
							if(p.y<crv.z+d*4.0 && p.y>crv.z-d*4.0) rgb /=4.0;                  
				      	}

				return half4(rgb+colDebug, 1.0); 
			#endif
			return half4(rgb, 1.0); 
        }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragNTSCOld3Phase

            ENDHLSL
        }
		        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragNTSC3Phase

            ENDHLSL
        }
		        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragNTSC2Phase

            ENDHLSL
        }
		        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragCustomBleed

            ENDHLSL
        }
    }
}