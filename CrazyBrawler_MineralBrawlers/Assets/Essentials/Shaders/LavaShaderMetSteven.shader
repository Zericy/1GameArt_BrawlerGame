Shader "Unlit/LavaShader"
{
    Properties
    {
		[Header(Textures)]
		_NoiseMap1 ("Noise Map 1", 2D) = "white" {}
        _NoiseMap2 ("Noise Map 2", 2D) = "white" {}
		_CausticsMap("Caustics", 2D) = "white" {}

		[Space(20)]
        [Header(Main)]
        _Color1("Main Tint Start", Color) = (0.5, 0.1,0.1, 1)
        _Color2("Main Tint End", Color) = (0.6, 0.4,0.1, 1)
        _Color3("Top Layer Tint", Color) = (1, 1,0, 1)
        _Offset("Start/End Tint Offset", Range(0,10)) = 1
        _SpeedMainX("Speed Main X", Range(-10,10)) = 0.4
        _SpeedMainY("Speed Main Y", Range(-10,10)) = 0.4       
        _Strength("Brightness Under Lava", Range(0,10)) = 2
        _StrengthTop("Brightness Top Lava", Range(0,10)) = 3
        _Cutoff("Cutoff Top", Range(0,1)) = 0.9
        _TopBlur("Top Blur", Range(0,1)) = 0.1
 
        [Space(20)]
        [Header(Edge)]
        _EdgeC("Edge Color", Color) = (1, 0.5, 0.2, 1)
        _EdgeBlur("Edge Blur", Range(0,1)) = 0.5
        _Edge("Edge Thickness", Range(0,20)) = 8
		_EdgeStrength("Edge Strength", Range(-10,20)) = 1.0
 
        [Space(20)]
        [Header(Distortion)]
        _ScaleDist("Scale Distortion", Range(0,1)) = 0.5
        _SpeedDistortX("Speed Distort X", Range(-10,10)) = 0.2
        _SpeedDistortY("Speed Distort Y", Range(-10,10)) = 0.2
        _Distortion("Distort Strength", Range(0,1)) = 0.2
        _VertexDistortion("Extra Vertex Color Distortion", Range(0,1)) = 0.3
       
        //[Space(10)]
        //[Header(Vertex Movement)]
        //_Speed("Wave Speed", Range(0,1)) = 0.5
        //_Amount("Wave Amount", Range(0,1)) = 0.6
        //_Height("Wave Height", Range(0,1)) = 0.1





    }
    SubShader
    {
        Tags { 
		"RenderType"="Opaque"
		"RenderPipeline"="UniversalPipeline"
		"IgnoreProjector" = "true"
		"Queue"="Transparent"
		}
        LOD 100

        Pass
        {
		 Tags {
		 "LightMode" = "UniversalForward"

		}
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma excluse_renderers d3d11_9x

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

  
            #pragma vertex vert
			#pragma fragment frag

			CBUFFER_START(unityPerMaterial)
			//Noise Maps
			float4 _CausticsMap_ST;
			float4 _NoiseMap1_ST;
			float _SpeedDistortX;
			float _SpeedDistortY;

			//Caustics map
			float _VertexDistortion;
			float _SpeedMainY;
			float _SpeedMainX;
			float _Distortion;

			//Color Lerp
			float4 _Color1;
			float4 _Color2;
			float _Offset;
			float _Strength;

			//Edge detection
			float _Edge;
			float _EdgeBlur;
			float4 _EdgeC;
			float _EdgeStrength;
			float _StrengthTop;

			float _TopBlur;
			float _Cutoff;
			float4 _Color3;


			CBUFFER_END

			TEXTURE2D(_NoiseMap1);
			TEXTURE2D(_NoiseMap2);
			TEXTURE2D(_CausticsMap);


			SAMPLER(sampler_NoiseMap1);

            struct Attributes
            {
                float4 positionOS : POSITION0;
				float4 color : COLOR0;
          
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
				float3 positionWS : POSITION1;
				float4 positionSS : POSITION2;

				float2 noiseUV : TEXCOORD0;
				float4 color : COLOR0;
            };

            Varyings vert (Attributes i)
            {
                Varyings o;
				o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
				o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
				o.color = i.color;
				o.positionSS = ComputeScreenPos(o.positionCS);

                o.noiseUV = TRANSFORM_TEX(o.positionWS.xz, _NoiseMap1);

                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
		
				
				float speedDistortX = _Time.x * _SpeedDistortX;
				float speedDistortY = _Time.x * _SpeedDistortY;
				float2 speedDistortCombined = float2(speedDistortX, speedDistortY);


				float d = SAMPLE_TEXTURE2D(_NoiseMap1, sampler_NoiseMap1, i.noiseUV + speedDistortCombined).x;
				float d2 = SAMPLE_TEXTURE2D(_NoiseMap1, sampler_NoiseMap1, i.noiseUV * 0.5 + speedDistortCombined).x;


                // sample the texture

				float layerreddist = saturate((d+d2)*0.5);

				float2 uvMain = i.positionWS.xz * _CausticsMap_ST;
				uvMain += layerreddist * _Distortion;

				float speedMainX = _Time.x * _SpeedMainX;
				float speedMainY = _Time.x * _SpeedMainY;
				float2 speedMainCombined = float2(speedMainX, speedMainY);

				uvMain += speedMainCombined + (i.color.x * _VertexDistortion);

				float4 col = SAMPLE_TEXTURE2D(_CausticsMap, sampler_NoiseMap1, uvMain) * i.color.x;

				col += layerreddist;
				float4 color = lerp(_Color1, _Color2, col* _Offset) * _Strength;

				float2 uv = i.positionSS.xy/i.positionSS.w;	
				float rawDepth = SampleSceneDepth(uv);

					
				float depth = LinearEyeDepth(rawDepth, _ZBufferParams);
				float edgeLine = 1 - saturate(_Edge* (depth - i.positionSS.w));

				float edge = smoothstep(1- col, 1- col+ _EdgeBlur, edgeLine);

				color *= (1-edge);
				color += (edge *_EdgeC) * _EdgeStrength;

				float top = smoothstep(_Cutoff, _Cutoff + _TopBlur, col) * i.color.x;

				color *= (1-top);
				color *= i.color.x;

				// add edge back in colored, multiply for brightness
 
				// add top back in colored, multiply for brightness
				color += top * _Color3 *_StrengthTop;


				return (color);
            }
            ENDHLSL
        }
    }
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
