Shader "Custom/Hilo"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MetalColor("Metal Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_GlossinessInMetal ("Smoothness in Metal", Range(0,1)) = 0.5
        _MetallicInMetal ("Metallic in Metal", Range(0,1)) = 0.0
		_Twist("Twist",Float) = 45
		_NormalScale("NormalScale",Range(0,1)) = 1
		_NormalMap("NormalMap",2D) = "white" {}
        _NoiseMap("NoiseMap",2D) = "white" {}
		_NoiseMapScale("NoiseMapScale",Range(0,50)) = 1
		_NormalMapScale("NormalMapScale",Range(0,3)) = 1
		_MetalTwist ("Metal Twist",Float) = 0.0
		_MetalRepeat ("Metal Repeat",Float) = 0.0
		// Metallic band
		_MetallicBand("Metallic Band", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _NormalMap;
        sampler2D _NoiseMap;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_NoiseMap;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _MetalColor;
		half _Twist;
		half _NormalScale;
		half _NormalMapScale;
        half _NoiseMapScale;
        half _NoiseMapTiling;
        half _GlossinessInMetal;
        half _MetallicInMetal;
        half _MetalTwist;
        half _MetalRepeat;
        half _MetallicBand;
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half2 uvMetal= half2(IN.uv_MainTex.x+IN.uv_MainTex.y*_MetalTwist,IN.uv_MainTex.y*_MetalRepeat);
			half metalThreshold = uvMetal.x*_MetalRepeat-round(uvMetal.x*_MetalRepeat)+0.5;	
            fixed4 c = lerp( _MetalColor, _Color,step(_MetallicBand,metalThreshold));
			o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = lerp( _MetallicInMetal, _Metallic,step(_MetallicBand,metalThreshold));
            o.Smoothness = lerp( _GlossinessInMetal, _Glossiness,step(_MetallicBand,metalThreshold));
            o.Alpha = c.a;

			// Thread curvature normal
			float twist = _Twist * 6.28 / 360;
			
			//twist += tex2D (_NoiseMap, half2(IN.uv_NoiseMap.y,0)).r*_NoiseMapScale;
			
			float sint = sin(twist);
			float cost = cos(twist);
			
			float ynoise = tex2D (_NoiseMap, half2(IN.uv_NoiseMap.x,IN.uv_NoiseMap.y)).r*_NoiseMapScale;
			half2 tcoords = half2(IN.uv_MainTex.x,IN.uv_MainTex.y + ynoise);
			
			
			float angle = frac(tcoords.x*cost + tcoords.y *sint)*3.14159;
            //Normal                        
			half3 n = half3(
				-cos(angle),
				0,
				sin(angle)
				);
            
            //Normal rotated in
			half3 rn = half3(
				n.x * cost - n.y * sint,
				n.x * sint + n.y * cost,
				n.z);
				
			rn = normalize(lerp(half3(0, 0, 1), rn, _NormalScale));
            
			// Detail normal
			// Rotate texture coordinate
			half2 rtc = half2(
				-tcoords.x * cost - tcoords.y * sint,
				-tcoords.x * sint + tcoords.y * cost
				);
				
			
			half3 dn = UnpackScaleNormal(tex2D(_NormalMap, rtc), _NormalMapScale);
			//dn = normalize(lerp(half3(0, 0, 1), dn, _NormalMapScale));

            //Calculo banda metálica
            
			
        

			//o.Normal = normalize(rn + dn);
			o.Normal = lerp(half3(0,0,1),normalize(rn+dn),step(_MetallicBand,uvMetal.x*_MetalRepeat-round(uvMetal.x*_MetalRepeat)+0.5));
			
				
        }
        ENDCG
    }
    FallBack "Diffuse"
}
