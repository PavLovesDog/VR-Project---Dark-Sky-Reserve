Shader "Custom/Skybox180Degree"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "Queue" = "Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float PI = 3.14159265;

            v2f vert(appdata_t appdata)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(appdata.vertex);
                // Convert the vertex position to spherical coordinates
                float3 dir = normalize(mul(_WorldSpaceCameraPos - appdata.vertex.xyz, unity_WorldToObject).xyz);
                float u = atan2(dir.x, dir.z) / (2.0 * PI) + 0.5;
                float v = asin(clamp(dir.y, -1.0, 1.0)) / PI + 0.5;
                o.uv = float3(u, 1.0 - v * 0.5, 0.0);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // The y-coordinate is scaled to use only the top half of the texture
                // and the x-coordinate is wrapped around
                float2 uv = float2(i.uv.x, i.uv.y);
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
        FallBack "Skybox/Procedural"
}