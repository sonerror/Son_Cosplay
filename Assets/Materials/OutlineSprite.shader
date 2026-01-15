Shader "Sprites/Default With Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        // Outline Properties
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 100)) = 1
        [MaterialToggle] _OutlineEnabled ("Enable Outline", Float) = 0
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
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            // Outline variables
            uniform float _OutlineEnabled;
            uniform fixed4 _OutlineColor;
            uniform float _OutlineWidth;

            struct v2f_custom
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f_custom vert(appdata_t IN)
            {
                v2f_custom OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f_custom IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb *= c.a;
                
                // Apply outline if enabled
                if (_OutlineEnabled > 0.5)
                {
                    float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y) * _OutlineWidth;
                    
                    // Sample surrounding pixels
                    float outlineAlpha = 0.0;
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(texelSize.x, 0)).a);
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(-texelSize.x, 0)).a);
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(0, texelSize.y)).a);
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(0, -texelSize.y)).a);
                    
                    // Diagonal samples for better outline
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(texelSize.x, texelSize.y)).a);
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(-texelSize.x, texelSize.y)).a);
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(texelSize.x, -texelSize.y)).a);
                    outlineAlpha = max(outlineAlpha, SampleSpriteTexture(IN.texcoord + float2(-texelSize.x, -texelSize.y)).a);
                    
                    // If current pixel is transparent but outline is detected
                    if (c.a < 0.01 && outlineAlpha > 0.01)
                    {
                        c = _OutlineColor;
                        c.rgb *= c.a;
                    }
                }
                
                return c;
            }
            ENDCG
        }
    }
}