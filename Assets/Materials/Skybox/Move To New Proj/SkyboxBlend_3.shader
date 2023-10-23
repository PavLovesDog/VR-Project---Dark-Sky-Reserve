Shader "Custom/SkyboxBlend_3" {
    Properties{
        _Skybox1("Skybox 1", Cube) = "" {}
        _Skybox2("Skybox 2", Cube) = "" {}
        _Skybox3("Skybox 3", Cube) = "" {}
        _BlendValue("Blend Value", Range(0, 2)) = 1

        _TransitionTo2("Transition to Skybox 2", Range(0, 2)) = 1
        _TransitionTo3("Transition to Skybox 3", Range(0, 2)) = 2

        _RotationX("Rotation X", Range(-360, 360)) = 0
        _RotationY("Rotation Y", Range(-360, 360)) = 0
        _RotationZ("Rotation Z", Range(-360, 360)) = 0
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
                samplerCUBE _Skybox3;
                float _BlendValue;
                float _TransitionTo2;
                float _TransitionTo3;
                float _RotationX;
                float _RotationY;
                float _RotationZ;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.pos = v.vertex.xyz;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    return o;
                }

                half4 frag(v2f i) : SV_Target {
                    float3 samplePos = i.pos; // Default sample position

                    float radX = _RotationX * (3.14159265 / 180.0);
                    float radY = _RotationY * (3.14159265 / 180.0);
                    float radZ = _RotationZ * (3.14159265 / 180.0);

                    float3x3 rotY = float3x3(
                        cos(radY), 0, -sin(radY),
                        0, 1, 0,
                        sin(radY), 0, cos(radY)
                    );

                    float3x3 rotX = float3x3(
                        1, 0, 0,
                        0, cos(radX), sin(radX),
                        0, -sin(radX), cos(radX)
                    );

                    float3x3 rotZ = float3x3(
                        cos(radZ), sin(radZ), 0,
                        -sin(radZ), cos(radZ), 0,
                        0, 0, 1
                    );

                    samplePos = mul(rotY, samplePos);
                    samplePos = mul(rotX, samplePos);
                    samplePos = mul(rotZ, samplePos);

                    half3 col1 = texCUBE(_Skybox1, samplePos).rgb;
                    half3 col2 = texCUBE(_Skybox2, samplePos).rgb;
                    half3 col3 = texCUBE(_Skybox3, samplePos).rgb;

                    half3 finalColor;
                    if (_BlendValue <= _TransitionTo2) {
                        finalColor = lerp(col1, col2, _BlendValue / _TransitionTo2);
                    }
                    else if (_BlendValue <= _TransitionTo3) {
                        finalColor = lerp(col2, col3, (_BlendValue - _TransitionTo2) / (_TransitionTo3 - _TransitionTo2));
                    }
                    else {
                        finalColor = col3;
                    }

                    return half4(finalColor, 1.0);
                }
                ENDCG
            }
        }
}
