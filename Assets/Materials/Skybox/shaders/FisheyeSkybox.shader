Shader "Custom/FisheyeSkybox"
{
    Properties
    {
        _MainTex("Fisheye Texture", 2D) = "white" {}
        _FisheyeRadius("Fisheye Radius", Float) = 1.0
        _FisheyeFOV("Fisheye FOV", Float) = 180.0
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

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _FisheyeRadius;
                float _FisheyeFOV;
                float PI = 3.14159265;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    // Map the vertex position to UV coordinates
                    float2 norm = normalize(v.vertex.xy) * 0.5 + 0.5;
                    o.uv = norm;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Map the normalized UV coordinates to fisheye coordinates
                    float2 uv = i.uv;
                    uv = uv * 2 - 1; // Convert from [0, 1] to [-1, 1]
                    float r = length(uv);
                    if (r > 1.0) discard; // Discard pixels outside the fisheye circle

                    float theta = r * (_FisheyeFOV / 360.0 * PI);
                    float phi = atan2(uv.y, uv.x);

                    // Convert fisheye polar coordinates to spherical coordinates
                    float s = cos(theta);
                    float t = sin(theta);

                    // The final UV coordinates
                    float2 finalUV = float2((phi / (2 * PI)) + 0.5, 1 - (s + 1) * 0.5);
                    finalUV.y *= _FisheyeRadius;

                    // Sample the texture with the final UV coordinates
                    return tex2D(_MainTex, finalUV);
                }
                ENDCG
            }
        }
            FallBack "Skybox/Procedural"
}
