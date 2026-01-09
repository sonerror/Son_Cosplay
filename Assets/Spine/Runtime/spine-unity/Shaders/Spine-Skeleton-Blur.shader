Shader "Spine/Skeleton Blur" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8
		
		// Blur properties
		[Toggle(_BLUR_ON)] _BlurEnabled("Enable Blur", Float) = 0
		_BlurSize("Blur Size", Range(0, 10)) = 1.0
		_BlurIterations("Blur Quality", Range(1, 4)) = 2

		// Outline properties
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector][MaterialToggle(_USE_SCREENSPACE_OUTLINE_WIDTH)] _UseScreenSpaceOutlineWidth("Width in Screen Space", Float) = 0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector][MaterialToggle(_OUTLINE_FILL_INSIDE)]_Fill("Fill", Float) = 0
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineOpaqueAlpha("Opaque Alpha", Range(0,1)) = 1.0
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		Pass {
			Name "Normal"

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma shader_feature _BLUR_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "CGIncludes/Spine-Common.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _BlurSize;
			int _BlurIterations;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColor = PMAGammaToTargetSpace(v.vertexColor);
				return o;
			}

			float4 GaussianBlur(float2 uv, float2 direction) {
				float4 color = float4(0, 0, 0, 0);
				float weights[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};
				
				color += tex2D(_MainTex, uv) * weights[0];
				
				for (int i = 1; i < 5; i++) {
					float2 offset = direction * i * _BlurSize * _MainTex_TexelSize.xy;
					color += tex2D(_MainTex, uv + offset) * weights[i];
					color += tex2D(_MainTex, uv - offset) * weights[i];
				}
				
				return color;
			}

			float4 frag (VertexOutput i) : SV_Target {
				float4 texColor;
				
				#ifdef _BLUR_ON
				// Apply blur
				texColor = float4(0, 0, 0, 0);
				
				for (int iter = 0; iter < _BlurIterations; iter++) {
					// Horizontal blur
					texColor += GaussianBlur(i.uv, float2(1, 0));
					// Vertical blur
					texColor += GaussianBlur(i.uv, float2(0, 1));
				}
				
				texColor /= (_BlurIterations * 2);
				#else
				texColor = tex2D(_MainTex, i.uv);
				#endif

				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif

				return (texColor * i.vertexColor);
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog { Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct VertexOutput {
				V2F_SHADOW_CASTER;
				float4 uvAndAlpha : TEXCOORD1;
			};

			VertexOutput vert (appdata_base v, float4 vertexColor : COLOR) {
				VertexOutput o;
				o.uvAndAlpha = v.texcoord;
				o.uvAndAlpha.a = vertexColor.a;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
				clip(texcol.a * i.uvAndAlpha.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	CustomEditor "SpineShaderWithOutlineGUI"
}