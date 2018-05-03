Shader "Hidden/FacePart" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        [MaterialToggle] _FlipHorizontal ("Flip Horizontally", Float) = 0
    }
    SubShader {
        Tags { "Queue" = "Transparent" }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed4 _Color;
            Float _FlipHorizontal;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = i.uv;
                if(_FlipHorizontal > 0)
                {
                    uv.x = 1.0 - uv.x;
                }
                fixed4 col = tex2D(_MainTex, uv);
                col.rgb *= _Color;
                col.a *= _Color.a;
                return col;
            }
            ENDCG
        }


    }
}