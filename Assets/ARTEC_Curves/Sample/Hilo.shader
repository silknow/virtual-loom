Shader "Custom/Hilo"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap 1", 2D) = "bump" {}
        _BumpMap2 ("Bumpmap 2", 2D) = "bump" {}
        _BumpMult1 ("Bumpmap multiplier 1",Range(-1,1)) = 0.0
        _BumpMult2 ("Bumpmap multiplier 2",Range(-1,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Twist ("Twist",Float) = 0.0
        _Threads ("Number of threads",Int) = 3
        _Repeat ("Repeat",Float) = 0.0
        _NoisePower ("Noise Power",Float) = 0.0
        
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

        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _BumpMap2;
        half _BumpMult1;
        half _BumpMult2;
        half _Glossiness;
        half _Metallic;
        half _Twist;
        half _Repeat;
        int _Threads;
        half _NoisePower;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_BumpMap2;
        };
        
        


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }
            
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            
            half2 uvMainTex= half2(IN.uv_MainTex.x*_Threads+IN.uv_MainTex.y*_Twist+random(IN.uv_MainTex)*_NoisePower,IN.uv_MainTex.y*_Repeat*_Threads);
            half2 uvBumpMap= half2(IN.uv_BumpMap.x*_Threads+IN.uv_BumpMap.y*_Twist+random(IN.uv_BumpMap)*_NoisePower,IN.uv_BumpMap.y*_Repeat*_Threads);
            half2 uvBumpMap2= half2(IN.uv_BumpMap2.x*_Threads+IN.uv_BumpMap2.y*_Twist,IN.uv_BumpMap2.y*_Repeat*_Threads);
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, uvMainTex) * _Color;
            o.Albedo = c.rgb;
            o.Normal = normalize(UnpackNormal (tex2D (_BumpMap, uvBumpMap)*_BumpMult1)+UnpackNormal(tex2D (_BumpMap2, uvBumpMap2)*_BumpMult2));
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
