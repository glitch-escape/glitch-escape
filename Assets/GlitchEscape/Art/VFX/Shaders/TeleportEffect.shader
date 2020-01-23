Shader "Custom/TeleportEffect"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Noise ("Noise Texture", 2D) = "white" {}
		_AlphaThreshold ("AlphaThreshold", Range(0,1)) = 0
		[HDR]
		_DissolveColor ("Dissolve Color", Color) = (1, 0, .8, 1)
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent"}
        LOD 200

		Zwrite Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _Noise;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv2_Noise;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		half _AlphaThreshold;
		fixed4 _DissolveColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			fixed4 alphaTex = tex2D (_Noise, IN.uv2_Noise);
            o.Alpha = alphaTex.r > _AlphaThreshold ? 0 : 1;
			o.Emission = _DissolveColor * (o.Alpha == 0 ? smoothstep(alphaTex.r, _AlphaThreshold + .05, 1) : 0);
			o.Specular = 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
