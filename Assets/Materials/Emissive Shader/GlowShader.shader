Shader "Custom/GlowShader" {
    Properties{
        _Color("Main Color", Color) = (.5,.5,.5,1)
        _Emission("Emissive Color", Color) = (1,1,1,1)
        _GlowStrength("Glow Strength", Range(0, 10)) = 1
        _MainTex("Base (RGB)", 2D) = "white" { }
    }
    SubShader{
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _Emission;
        float _GlowStrength;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            fixed3 result = tex2D(_MainTex, IN.uv_MainTex).rgb;
            o.Albedo = result * _Color.rgb;
            o.Emission = _Emission.rgb * _GlowStrength;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
