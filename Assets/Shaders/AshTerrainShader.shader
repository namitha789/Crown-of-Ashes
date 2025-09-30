Shader "Custom/AshTerrainShader"
   {
       Properties
       {
           _MainTex ("Base Texture", 2D) = "white" {}
           _AshTex ("Ash Texture", 2D) = "white" {}
           _AshAmount ("Ash Amount", Range(0,1)) = 0.5
           _AshColor ("Ash Color", Color) = (0.5, 0.5, 0.5, 1)
           _Glossiness ("Smoothness", Range(0,1)) = 0.1
           _Metallic ("Metallic", Range(0,1)) = 0.1
       }
       
       SubShader
       {
           Tags { "RenderType"="Opaque" }
           LOD 200
   
           CGPROGRAM
           // Physically based Standard lighting model
           #pragma surface surf Standard fullforwardshadows
   
           // Use shader model 3.0 target for best compatibility
           #pragma target 3.0
   
           sampler2D _MainTex;
           sampler2D _AshTex;
           half _AshAmount;
           fixed4 _AshColor;
           half _Glossiness;
           half _Metallic;
   
           struct Input
           {
               float2 uv_MainTex;
               float2 uv_AshTex;
           };
   
           void surf (Input IN, inout SurfaceOutputStandard o)
           {
               // Base texture
               fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
               
               // Ash texture
               fixed4 ashTex = tex2D(_AshTex, IN.uv_AshTex);
               
               // Blend based on ash amount
               fixed4 finalColor = lerp(baseColor, ashTex * _AshColor, _AshAmount * ashTex.r);
               
               o.Albedo = finalColor.rgb;
               o.Metallic = _Metallic;
               o.Smoothness = _Glossiness;
               o.Alpha = finalColor.a;
           }
           ENDCG
       }
       
       FallBack "Diffuse"
   }