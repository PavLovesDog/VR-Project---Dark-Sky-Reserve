Shader "Custom/PortalShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _EdgeFeather("Edge Feather", Range(0.01, 1)) = 0.1
        _DistanceMultiplier("Distance Multiplier", Float) = 1.0
        _UVOffset("UV Offset", Vector) = (0, 0, 0, 0)
        _UVScale("UV Scale", Vector) = (1, 1, 0, 0)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
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
                    float3 worldPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _EdgeFeather;
                float _DistanceMultiplier;
                float2 _UVOffset;
                float2 _UVScale;

                // Camera and plane properties
                float3 _CameraWorldPos;
                float3 _PlaneNormal;
                float3 _PlaneWorldPos;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.uv = v.uv * _UVScale.xy + _UVOffset.xy;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Calculate angle between plane normal and view direction for edge feathering
                    float ndotv = dot(normalize(_PlaneNormal), normalize(i.worldPos - _CameraWorldPos));
                    float edge = smoothstep(0.0, _EdgeFeather, ndotv);

                    // Calculate distance from the camera to the plane
                    // Add a minimum distance to avoid division by zero or too large scaling factors
                    float distance = max(length(i.worldPos - _CameraWorldPos), 1.0);
                    float distanceFactor = distance * _DistanceMultiplier;

                    // Adjust UVs based on camera position for shifting effect
                    float2 uvShift = i.uv - 0.5;

                    // Get the camera position relative to the plane
                    // We will only adjust the UVs based on the camera's X and Z position, not Y (height)
                    float2 cameraRelPos = _CameraWorldPos.xz - _PlaneWorldPos.xz;

                    // Adjust UVs based on camera relative X and Z position only
                    uvShift -= cameraRelPos / distanceFactor;

                    uvShift += 0.5; // Re-center the UVs after adjustment

                    // Clamp the UVs to avoid tiling and ensure the image covers the entire plane
                    uvShift = clamp(uvShift, 0.0, 1.0);

                    // Sample the texture with the clamped UVs
                    fixed4 col = tex2D(_MainTex, uvShift) * edge;

                    // Apply edge feathering
                    col.a *= edge;

                    return col;

                    //// Calculate angle between plane normal and view direction for edge feathering
                    //float ndotv = dot(normalize(_PlaneNormal), normalize(i.worldPos - _CameraWorldPos));
                    //float edge = smoothstep(0.0, _EdgeFeather, ndotv);
                    //
                    //// Calculate distance from the camera to the plane for scaling UVs
                    //float distanceFactor = length(i.worldPos - _CameraWorldPos) * _DistanceMultiplier;
                    //
                    //// Adjust UVs based on camera position for shifting effect
                    //// Make sure to center the UVs by subtracting half the distance vector
                    //float2 uvShift = i.uv - 0.5;
                    //float3 cameraRelPos = _CameraWorldPos - _PlaneWorldPos; // Get the camera position relative to the plane
                    //
                    //// Adjust UVs based on camera relative position and camera height
                    //uvShift.x -= cameraRelPos.x / distanceFactor; // Adjust UVs based on camera relative position on X
                    //uvShift.y -= cameraRelPos.y / distanceFactor; // Adjust UVs based on camera height
                    //
                    //uvShift += 0.5; // Re-center the UVs after adjustment
                    //
                    //// Clamp the UVs to avoid tiling and ensure the image covers the entire plane
                    //uvShift = clamp(uvShift, 0.0, 1.0);
                    //
                    //// Sample the texture with the clamped UVs
                    //fixed4 col = tex2D(_MainTex, uvShift) * edge;
                    //
                    //// Apply edge feathering
                    //col.a *= edge;
                    //
                    //return col;
                }
                ENDCG
            }
        }
}
