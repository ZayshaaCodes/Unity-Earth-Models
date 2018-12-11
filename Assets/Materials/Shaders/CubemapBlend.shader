// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "CookieShades/CubemapBlend" {
Properties {
    _Latitude("Latitude", Range(-90, 90)) = 0
    _Longitude ("Longitude", Range(-180, 180)) = 0
	[Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
    [NoScaleOffset] _Tex ("Star Map", Cube) = "black" {}
    [NoScaleOffset] _Tex2 ("Overlay 1", Cube) = "black" {}
	_Tint("Tint Color", Color) = (.5, .5, .5, .5)
	_value("Value", Range(0, .2)) = 0.0
    [NoScaleOffset] _Tex3 ("Overlay 2", Cube) = "black" {}
	_Tint2("Tint Color2", Color) = (.5, .5, .5, .5)
	_value2("Value2", Range(0, .2)) = 0.0
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0

        #include "UnityCG.cginc"

        samplerCUBE _Tex;
        samplerCUBE _Tex2;
        samplerCUBE _Tex3;
        half4 _Tex_HDR;
        half4 _Tex2_HDR;
        half4 _Tex3_HDR;
        half4 _Tint;
        half4 _Tint2;
        half _Exposure;
        float _Latitude;
        float _Longitude;
		float _value;
		float _value2;

        float3 RotateAroundYInDegrees (float3 vertex, float Latitude, float Longitude)
        {
            float alpha = -(Latitude-90) * UNITY_PI / 180.0;
            float beta = Longitude * UNITY_PI / 180.0;
            float sina, cosa;
            float sinb, cosb;
            sincos(alpha, sina, cosa);
            sincos(beta, sinb, cosb);
            float3x3 mtx = 
				float3x3(
					1, 0,    0, 
					0, cosa,-sina,
					0, sina, cosa);

            float3x3 mtx2 = 
				float3x3(
					cosb, 0, sinb, 
					0,    1, 0,
				   -sinb, 0, cosb);

            return mul(mtx,mul(mtx2, vertex.xyz).xyz).xyz;
        }

        struct appdata_t {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert (appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            float3 rotated = RotateAroundYInDegrees(v.vertex, _Latitude, _Longitude);
            o.vertex = UnityObjectToClipPos(rotated);
            o.texcoord = v.vertex.xyz;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            half4 tex = texCUBE (_Tex, i.texcoord);
            half4 tex2 = texCUBE (_Tex2, i.texcoord);
            half4 tex3 = texCUBE (_Tex3, i.texcoord);
            half3 c = DecodeHDR (tex, _Tex_HDR);
            half3 c2 = DecodeHDR (tex2, _Tex2_HDR) * _value * _Tint.rgb;
            half3 c3 = DecodeHDR (tex3, _Tex3_HDR) * _value2 * _Tint2.rgb;
            c = (c + c2 + c3) * unity_ColorSpaceDouble.rgb;
            c *= _Exposure;
            return half4(c, 1);
        }
        ENDCG
    }
}


Fallback Off

}
