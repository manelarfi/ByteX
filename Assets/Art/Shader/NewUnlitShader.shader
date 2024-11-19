Shader "Custom/Blur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
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
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float blurSize = 0.002; // You can change this value for more or less blur
                float4 col = tex2D(_MainTex, i.uv);
                
                col += tex2D(_MainTex, i.uv + float2(blurSize, blurSize));
                col += tex2D(_MainTex, i.uv + float2(-blurSize, -blurSize));
                col += tex2D(_MainTex, i.uv + float2(blurSize, -blurSize));
                col += tex2D(_MainTex, i.uv + float2(-blurSize, blurSize));
                
                return col * 0.2;
            }
            ENDCG
        }
    }
}
