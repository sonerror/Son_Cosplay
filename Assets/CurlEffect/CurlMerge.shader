Shader "Custom/CurlMerge"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _CurlRadius("Curl Radius", Range(0.01, 1)) = 0.05
        _CurlProgress("Curl Progress", Range(0, 1)) = 0
        _Aspect("Aspect Ratio", Float) = 1
        _CurlAngle("Curl Angle (degrees)", Range(0, 360)) = 45
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        // Pass 1: Mask
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragMask
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CurlRadius;
            float _CurlProgress;
            float _Aspect;
            float _CurlAngle;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 fragMask(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float rad = radians(_CurlAngle);
                float2 dir = normalize(float2(cos(rad), sin(rad)));

                float2 originUV = float2(dir.x < 0 ? 1.0 : 0.0, dir.y < 0 ? 1.0 : 0.0);
                float2 uvAspect = float2(uv.x * _Aspect, uv.y);
                float2 originAspect = float2(originUV.x * _Aspect, originUV.y);

                float2 farUV = float2(dir.x > 0 ? 1.0 : 0.0, dir.y > 0 ? 1.0 : 0.0);
                float2 farAspect = float2(farUV.x * _Aspect, farUV.y);

                float maxDist = dot(farAspect - originAspect, dir);
                float proj = dot(uvAspect - originAspect, dir);
                float dist = proj - _CurlProgress * maxDist;

                if (dist > _CurlRadius)
                    discard;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }

        // Pass 2: Curl
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CurlRadius;
            float _CurlProgress;
            float _Aspect;
            float _CurlAngle;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float rad = radians(_CurlAngle);
                float2 dir = normalize(float2(cos(rad), sin(rad)));

                float2 originUV = float2(dir.x < 0 ? 1.0 : 0.0, dir.y < 0 ? 1.0 : 0.0);
                float2 uvAspect = float2(uv.x * _Aspect, uv.y);
                float2 originAspect = float2(originUV.x * _Aspect, originUV.y);

                float2 farUV = float2(dir.x > 0 ? 1.0 : 0.0, dir.y > 0 ? 1.0 : 0.0);
                float2 farAspect = float2(farUV.x * _Aspect, farUV.y);

                float maxDist = dot(farAspect - originAspect, dir);
                float proj = dot(uvAspect - originAspect, dir);
                float dist = proj - _CurlProgress * maxDist;

                float2 linePoint = uvAspect - dist * dir;
                fixed4 color;

                if (dist > _CurlRadius)
                {
                    discard;
                }
                else if (dist >= 0.0)
                {
                    float theta = asin(dist / _CurlRadius);
                    float2 p2 = linePoint + dir * (3.1415 - theta) * _CurlRadius;
                    float2 p1 = linePoint + dir * theta * _CurlRadius;
                    float2 uvTry = (all(p2 > 0.) && all(p2 < float2(_Aspect, 1.))) ? p2 / float2(_Aspect, 1.) : p1 / float2(_Aspect, 1.);
                    color = tex2D(_MainTex, uvTry);
                    float shadow = pow(saturate((_CurlRadius - dist) / _CurlRadius), 2.0);
                    color.rgb *= lerp(1.0, 0.9, shadow);
                }
                else
                {
                    float2 p = linePoint + dir * (abs(dist) + 3.1415 * _CurlRadius);
                    float2 uvTry = (all(p > 0.) && all(p < float2(_Aspect, 1.))) ? p / float2(_Aspect, 1.) : uv;
                    color = tex2D(_MainTex, uvTry);
                }

                return color;
            }
            ENDCG
        }

        // Pass 3: Fold Mirror (Reflected back face)
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragFoldMirror
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CurlRadius;
            float _CurlProgress;
            float _Aspect;
            float _CurlAngle;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 fragFoldMirror(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float rad = radians(_CurlAngle);
                float2 dir = normalize(float2(cos(rad), sin(rad)));

                float2 originUV = float2(dir.x < 0 ? 1.0 : 0.0, dir.y < 0 ? 1.0 : 0.0);
                float2 uvAspect = float2(uv.x * _Aspect, uv.y);
                float2 originAspect = float2(originUV.x * _Aspect, originUV.y);

                float2 farUV = float2(dir.x > 0 ? 1.0 : 0.0, dir.y > 0 ? 1.0 : 0.0);
                float2 farAspect = float2(farUV.x * _Aspect, farUV.y);

                float maxDist = dot(farAspect - originAspect, dir);
                float proj = dot(uvAspect - originAspect, dir);
                float dist = proj - _CurlProgress * maxDist;

                if (dist > _CurlRadius)
                    discard;

                float2 linePoint = uvAspect - dist * dir;
                float2 p = linePoint + dir * (abs(dist) + 3.1415 * _CurlRadius);

                float2 delta = p - linePoint;
                float2 perp = float2(-dir.y, dir.x);
                float2 reflected = p - 2.0 * dot(delta, perp) * perp;

                float2 reflectedUV = reflected / float2(_Aspect, 1.0);
                if (any(reflectedUV < 0.0) || any(reflectedUV > 1.0))
                    discard;

                fixed4 color = tex2D(_MainTex, reflectedUV);
                color.rgb = lerp(color.rgb, float3(1,1,1), 0.5);
                return color;
            }
            ENDCG
        }
    }
}
