Shader "Custom/SkyboxBlended" {
    Properties{
        _Skybox1("Skybox 1", Cube) = "" {}
        _Skybox2("Skybox 2", Cube) = "" {}
        _BlendValue("Blend Value", Range(0, 1)) = 0.5
        _RotationX("Rotation X", Range(-180, 180)) = 0
        _RotationY("Rotation Y", Range(-180, 180)) = 0
        _RotationZ("Rotation Z", Range(-180, 180)) = 0
    }

        SubShader{
            Pass {
                Tags { "Queue" = "Background" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                };

                struct v2f {
                    float3 pos : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                samplerCUBE _Skybox1;
                samplerCUBE _Skybox2;
                float _BlendValue;
                float _RotationX;
                float _RotationY;
                float _RotationZ;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.pos = v.vertex.xyz;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                half4 frag(v2f i) : SV_Target {
                    // Convert degrees to radians
                    float radX = _RotationX * (3.14159265 / 180.0);
                    float radY = _RotationY * (3.14159265 / 180.0);
                    float radZ = _RotationZ * (3.14159265 / 180.0);

                    // Rotation matrix for Y-axis
                    float3x3 rotY = float3x3(
                        cos(radY), 0, -sin(radY),
                        0, 1, 0,
                        sin(radY), 0, cos(radY)
                    );

                    // Rotation matrix for X-axis
                    float3x3 rotX = float3x3(
                        1, 0, 0,
                        0, cos(radX), sin(radX),
                        0, -sin(radX), cos(radX)
                    );

                    // Rotation matrix for Z-axis
                    float3x3 rotZ = float3x3(
                        cos(radZ), sin(radZ), 0,
                        -sin(radZ), cos(radZ), 0,
                        0, 0, 1
                    );

                    // Apply rotations
                    float3 rotatedPos = mul(rotY, i.pos); // Y-axis rotation
                    rotatedPos = mul(rotX, rotatedPos);   // X-axis rotation
                    rotatedPos = mul(rotZ, rotatedPos);   // Z-axis rotation

                    half3 col1 = texCUBE(_Skybox1, rotatedPos).rgb;
                    half3 col2 = texCUBE(_Skybox2, rotatedPos).rgb;
                    return half4(lerp(col1, col2, _BlendValue), 1.0);
                }
                ENDCG
            }
        }
}