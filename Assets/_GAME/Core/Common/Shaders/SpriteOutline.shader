Shader "Mobile/SpriteOutline" {
	Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 1
		_FadeStrength("Fade Strength", Range(0, 10)) = 1
		_CenterUV ("Center UV", Vector) = (0.5, 0.5, 0, 0)
		_OutlineUVOffset("Outline UV Offset", Range(0, 0.2)) = 0.01
		_OutlineAlphaThreshold("Outline Alpha Cutoff", Range(0, 0.3)) = 0.1
        [KeywordEnum(Three_0, Three_30, Three_60, Four_0, Four_45, Six_0, Six_30, Eight)] _OutlineDir ("Outline Directions", Float) = 0
		[Toggle] _UseClampCheck ("Use Clamp Check", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON

			#pragma shader_feature _OUTLINEDIR_THREE_0 _OUTLINEDIR_THREE_30 _OUTLINEDIR_THREE_60 _OUTLINEDIR_FOUR_0 _OUTLINEDIR_FOUR_45 _OUTLINEDIR_SIX_0 _OUTLINEDIR_SIX_30 _OUTLINEDIR_EIGHT
			#pragma shader_feature _USECLAMPCHECK_ON
            
			#ifndef UNITY_SPRITES_INCLUDED
            #define UNITY_SPRITES_INCLUDED

            #include "UnityCG.cginc"

            #ifdef UNITY_INSTANCING_ENABLED

                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    // SpriteRenderer.Color while Non-Batched/Instanced.
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
                    // this could be smaller but that's how bit each entry is regardless of type
                    UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
                #define _MainTex_ST     UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, _MainTex_ST)

            #endif // instancing

            CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
                fixed4 _RendererColor;
                fixed2 _Flip;
                float4 _MainTex_ST;
            #endif
            CBUFFER_END
            #ifdef _OUTLINEDIR_THREE_0
			#define NUM_SAMPLES 3
			static const float2 OFFSETS[3] = {
				float2(1.0, 0.0),         // 0°
				float2(-0.5, 0.8660254),  // 120°
				float2(-0.5, -0.8660254)  // 240°
			};
			#endif

			#ifdef _OUTLINEDIR_THREE_30
			#define NUM_SAMPLES 3
			static const float2 OFFSETS[3] = {
				float2(0.8660254, 0.5),    // 30°
				float2(-0.8660254, 0.5),   // 150°
				float2(0.0, -1.0)          // 270°
			};
			#endif

			#ifdef _OUTLINEDIR_THREE_60
			#define NUM_SAMPLES 3
			static const float2 OFFSETS[3] = {
				float2(0.5, 0.8660254),    // 60°
				float2(-1.0, 0.0),         // 180°
				float2(0.5, -0.8660254)    // 300°
			};
			#endif

			#ifdef _OUTLINEDIR_FOUR_0
			#define NUM_SAMPLES 4
			static const float2 OFFSETS[NUM_SAMPLES] = {
				float2(-1, 0),
				float2(1, 0),
				float2(0, -1),
				float2(0, 1)
			};
			#endif

			#ifdef _OUTLINEDIR_FOUR_45
			#define NUM_SAMPLES 4
			static const float2 OFFSETS[NUM_SAMPLES] = {
				float2(-0.7071, -0.7071),  // 225°
				float2(-0.7071, 0.7071),   // 135°
				float2(0.7071, -0.7071),   // 315°
				float2(0.7071, 0.7071)     // 45°
			};
			#endif

			#ifdef _OUTLINEDIR_SIX_0
			#define NUM_SAMPLES 6
			static const float2 OFFSETS[NUM_SAMPLES] = {
				float2(1.0, 0.0),            // 0°
				float2(0.5, 0.8660254),      // 60°
				float2(-0.5, 0.8660254),     // 120°
				float2(-1.0, 0.0),           // 180°
				float2(-0.5, -0.8660254),    // 240°
				float2(0.5, -0.8660254)      // 300°
			};
			#endif

			#ifdef _OUTLINEDIR_SIX_30
			#define NUM_SAMPLES 6
			static const float2 OFFSETS[NUM_SAMPLES] = {
				float2(0.8660254, 0.5),      // 30°
				float2(0.0, 1.0),            // 90°
				float2(-0.8660254, 0.5),     // 150°
				float2(-0.8660254, -0.5),    // 210°
				float2(0.0, -1.0),           // 270°
				float2(0.8660254, -0.5)      // 330°
			};
			#endif

			#ifdef _OUTLINEDIR_EIGHT
			#define NUM_SAMPLES 8
			static const float2 OFFSETS[8] = {
				float2(-1, 0),   float2(1, 0),
				float2(0, -1),   float2(0, 1),
				float2(-1, -1),  float2(1, 1),
				float2(-1, 1),   float2(1, -1)
			};
			#endif

			fixed4 _OutlineColor;
            float _OutlineWidth;
			float _FadeStrength;
			float2 _CenterUV;
			float _OutlineUVOffset;
			float _OutlineAlphaThreshold;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
                
				float2 offsetDir = normalize(IN.texcoord - _CenterUV);
				IN.texcoord = IN.texcoord + _OutlineUVOffset * offsetDir;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                return tex2D (_MainTex, uv * _MainTex_ST.xy + _MainTex_ST.zw);
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                float2 texelSize = 1.0 / _ScreenParams.xy;
				half4 col = SampleSpriteTexture(IN.texcoord) * IN.color;
				
				float2 uvMask = step(0.0, IN.texcoord) * step(IN.texcoord, 1.0);
				float mask = uvMask.x * uvMask.y;
				col.a *= mask;

				float alphaSum = 0;
				fixed neighborCount = 0;

				for (int i = 0; i < NUM_SAMPLES; ++i)
				{
					float2 coord = IN.texcoord + OFFSETS[i] * _OutlineWidth * texelSize;
					float alpha = SampleSpriteTexture(coord).a;

					#if defined(_USECLAMPCHECK_ON)
						uvMask = step(0.0, coord) * step(coord, 1.0);
						mask = uvMask.x * uvMask.y;
						alpha *= mask;
					#endif
					alphaSum += alpha;
					neighborCount += step(0.01, alpha);
				}

				float neighborAlpha = alphaSum / NUM_SAMPLES;//max(neighborCount, 1.0);
				float fade = saturate(neighborAlpha * _FadeStrength);
				float outlineFactor = step(0.01, neighborAlpha) * (1.0 - step(_OutlineAlphaThreshold, col.a));

				float outlineAlpha = _OutlineColor.a * fade * outlineFactor;
				col.rgb *= col.a;
				col = lerp(col, float4(_OutlineColor.rgb, outlineAlpha), outlineFactor);

				return col;
            }

            #endif // UNITY_SPRITES_INCLUDED
        ENDCG
        }
    }
}