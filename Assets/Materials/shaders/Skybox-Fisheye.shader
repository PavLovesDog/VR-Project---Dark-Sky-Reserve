Shader "Custom/Skybox-Fisheye"
{
    Properties
    {
        _MainTex("Fisheye Texture", 2D) = "white" {}
        _Tint("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _Exposure("Exposure", Range(0, 8)) = 1.0
        _RotationY("Rotation Y", Range(0, 360)) = 0
        _RotationX("Rotation X", Range(-90, 90)) = 0
    }
        SubShader
        {
            Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
            Cull Off ZWrite Off
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                half4 _Tint;
                half _Exposure;
                float _RotationY;
                float _RotationX;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float3 texcoord : TEXCOORD0;
                };

                float3 RotateAroundYInDegrees(float3 vertex, float degrees)
                {
                    float alpha = degrees * UNITY_PI / 180.0;
                    float sina, cosa;
                    sincos(alpha, sina, cosa);
                    float2x2 m = float2x2(cosa, -sina, sina, cosa);
                    return float3(mul(m, vertex.xz), vertex.y).xzy;
                }

                float3 RotateAroundXInDegrees(float3 vertex, float degrees)
                {
                    float alpha = degrees * UNITY_PI / 180.0;
                    float sina, cosa;
                    sincos(alpha, sina, cosa);
                    float2x2 m = float2x2(cosa, sina, -sina, cosa);
                    return float3(vertex.x, mul(m, vertex.yz));
                }

                v2f vert(appdata_t v)
                {
                    v2f o;
                    float3 rotatedY = RotateAroundYInDegrees(v.vertex, _RotationY);
                    float3 rotatedX = RotateAroundXInDegrees(rotatedY, _RotationX);
                    o.vertex = UnityObjectToClipPos(rotatedX);
                    o.texcoord = v.vertex.xyz;
                    return o;
                }

                inline float2 ToFisheyeCoords(float3 coords)
                {
                    float3 normalizedCoords = normalize(coords);
                    float r = 2.0 * atan2(length(normalizedCoords.xy), abs(normalizedCoords.z)) / UNITY_PI;
                    float theta = atan2(normalizedCoords.y, normalizedCoords.x);
                    float2 uv = float2(cos(theta), sin(theta)) * r * 0.5 + 0.5;
                    return float2(uv.x * 0.5, uv.y);
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 tc = ToFisheyeCoords(i.texcoord);
                    fixed4 tex = tex2D(_MainTex, tc);
                    fixed3 c = tex.rgb * _Tint.rgb; // Use the texture color directly
                    c *= _Exposure;
                    return fixed4(c, 1);
                }
                ENDCG
            }
        }
            FallBack "Skybox/Procedural"
}
