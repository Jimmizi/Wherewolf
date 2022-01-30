Shader "Unlit/ArtificialLitSpriteShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _TimeOfDay ("Time of Day", Range(0.0, 1.0)) = 0
        _DayColor ("Day Color", Color) = (1,1,1,1)
        _AfternoonColor ("Afternoon Color", Color) = (1,1,1,1)
        _NightColor ("Night Color", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
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
            "DisableBatching" = "True" 
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
            #include "UnityCG.cginc"

            #ifdef UNITY_INSTANCING_ENABLED
            
                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    // SpriteRenderer.Color while Non-Batched/Instanced.
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                    // this could be smaller but that's how bit each entry is regardless of type
                    UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)
            
                #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
            
            #endif // instancing
            
            CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
                fixed4 _RendererColor;
                fixed2 _Flip;
            #endif
                float _EnableExternalAlpha;
            CBUFFER_END
            
            // Material Color.
            fixed4 _Color;
            fixed4 _DayColor;
            fixed4 _AfternoonColor;
            fixed4 _NightColor;
            float _TimeOfDay;
            
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
            
            half4 ObjectPosition() {
                return half4(
                    unity_ObjectToWorld._m03_m13_m23,
                    0.0
                );
            }
            
            half4 ObjectScale() {
                return half4(
                    length(unity_ObjectToWorld._m00_m10_m20),
                    length(unity_ObjectToWorld._m01_m11_m21),
                    length(unity_ObjectToWorld._m02_m12_m22),
                    1.0
                );
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
            
                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            
                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                //OUT.vertex = mul(UNITY_MATRIX_P, 
                //    mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) + float4(OUT.vertex.x, OUT.vertex.y, 0.0, 0.0) * ObjectScale() //+ ObjectPosition()
                //);
                
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor; 
            
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }
            
            sampler2D _MainTex;
            sampler2D _AlphaTex;
            
            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);
            
            #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D (_AlphaTex, uv);
                color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
            #endif
            
                return color;
            }
            
            fixed3 darken(fixed3 a, fixed3 b)
        {
            return min(a, b);
        }

        fixed3 lighten(fixed3 a, fixed3 b)
        {
            return max(a, b);
        }

        fixed3 pinLight(fixed3 a, fixed3 b)
        {
            return (b < 0.5) ? darken(a, (2.0 * b)) : lighten(a, (2.0 * (b - 0.5)));
        }
        
            fixed4 Overlay (fixed4 a, fixed4 b)
            {
                fixed4 r = a < .5 ? 2.0 * a * b : 1.0 - 2.0 * (1.0 - a) * (1.0 - b);
                r.a = b.a;
                return r;
            }
            
            fixed4 ColorGradient(float position) {
                float3 color;
                float intensity;
                
                if (position < 0.4) {
                    color = lerp(_DayColor.rgb, _AfternoonColor.rgb, position / 0.4);
                    intensity = 1.0;
                } else {
                    color = lerp(_AfternoonColor.rgb, _NightColor.rgb, (position - 0.4) / 0.6);
                    intensity = lerp(1.0, 0.5, (position - 0.4) / 0.6);
                }
                
                return fixed4(color, intensity);
            }
            
            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                c.rgb *= c.a;
                
                fixed4 gradient = ColorGradient(_TimeOfDay);
                c.rgb = darken(c.rgb, gradient.rgb) * gradient.a;
                
                return c;
            }

        ENDCG
        }
    }
}