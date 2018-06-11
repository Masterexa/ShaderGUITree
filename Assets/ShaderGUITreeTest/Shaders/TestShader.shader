Shader "Custom/TestShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_GlossMap("GlossMap", 2D)="white"{}
		_Glossiness			("Smoothness", Range(0,1))	= 0.5
		[Gamma]_Metallic	("Metallic", Range(0,1))	= 0.0

		_OcclusionStrength	("Occlusion", Range(0,1))	= 1
		_OcclusionMap		("Occlusion Map", 2D)		= "white"{}

		_EmissionColor	("Emission Color", Color)=(0,0,0)
		_EmissionMap	("Emission Map", 2D) ="white"{}

		[HideInInspector, Enum(ShaderGUITree.RenderingMode)]_Mode	("Rendering Mode", Float)=0

		// Displacement
		[Enum(ShaderGUITree.DisplacementMode)]_DisplacementMode	("Displacement Mode", Float)=0
		_BumpScale			("Normal Factor", Float)=1
		_BumpMap			("Normal Map", 2D)="bump"{}
		_Parallax			("Height", Float)=0.1
		_ParallaxMap		("Height Map", 2D) = "white"{}

		_Tessellation		("Tessellation", Range(1,32))=4
		_TessellationEdge	("Edge", Range(1,50))=0.02
		_TessellationPhong	("Phong", Range(0,1))=0.02

		// Extra Property
		_VertexWaveStrength		("Vertex Wave Strength", Float)=1
		_VertexWaveFrequency	("Vertex Wave Frequency", Float)=1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow fullforwardshadows vertex:vert tessellate:hull_edge tessphong:_TessellationPhong

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D	_MainTex;
		fixed4		_Color;

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D	_GlossMap;
		half		_Metallic;
		half		_Glossiness;

		sampler2D	_OcclusionMap;
		half		_OcclusionStrength;

		sampler2D	_EmissionMap;
		half3		_EmissionColor;

		half		_DisplacementMode;
		sampler2D	_BumpMap;
		half		_BumpScale;
		sampler2D	_ParallaxMap;
		half		_Parallax;

		float		_Tessellation;
		float		_TessellationEdge;
		float		_TessellationPhong;

		float	_VertexWaveStrength;
		float	_VertexWaveFrequency;

		#include "Tessellation.cginc"
		#include "UnityCG.cginc"


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		inline float nearlyEqual(float a, float b)
		{
			return step(abs(a-b), 0.01);
		}

	/* --- Tessellation --- */

		inline float isTessellated()
		{
			return nearlyEqual(2.0, _DisplacementMode);
		}

		inline float getTessellationAmount()
		{
			return lerp(1, _Tessellation, isTessellated());
		}


	/* --- Materials --- */
		inline half2 getMetallicAndSmoothness(float2 coord)
		{
			half2 tex = tex2D(_GlossMap, coord).ra;
			tex.r *= _Metallic;
			tex.g *= _Glossiness;
			return tex;
		}

		inline half3 getEmission(float2 coord)
		{
			return tex2D(_EmissionMap, coord).rgb * _EmissionColor;
		}

		inline float getHeightMap(float2 coord)
		{
			return tex2D(_ParallaxMap, coord) * _Parallax;
		}

		inline float getVertexDisplacement(float2 coord)
		{
			return tex2Dlod(_ParallaxMap, float4(coord.xy, 0, 0)).g * _Parallax * isTessellated();
		}


		inline half3 getNormalMap(float2 coord)
		{
			half scale = _BumpScale * getHeightMap(coord);
			return UnpackScaleNormal(tex2D(_BumpMap, coord.xy), scale);
		}

		inline half getOcclusion(float2 coord)
		{
			return lerp(1.0, tex2D(_OcclusionMap, coord).g, _OcclusionStrength);
		}


		inline float getVertexWave(float3 position)
		{
			return sin((position.y+_Time)/_VertexWaveFrequency) * _VertexWaveStrength;
		}
	/* --- Basis --- */


		float4 hull_fixed()
		{
			return getTessellationAmount();
		}

		float4 hull_distance(appdata_full v0, appdata_full v1, appdata_full v2)
		{
			float minDist	= 10.0;
			float maxDist	= 25.0;

			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, getTessellationAmount());
		}

		float4 hull_edge(appdata_full v0, appdata_full v1, appdata_full v2)
		{
			return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _TessellationEdge);
		}


		void vert(inout appdata_full v)
		{
			v.vertex.xyz += getVertexDisplacement(v.texcoord) * v.normal;
			v.vertex.xyz += getVertexWave(v.texcoord) * v.normal;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4	c	= tex2D (_MainTex, IN.uv_MainTex) * _Color;
			half2	ms	= getMetallicAndSmoothness(IN.uv_MainTex);

			o.Albedo		= c.rgb;
			o.Alpha			= c.a;

			// Metallic and smoothness come from slider variables
			o.Metallic		= ms.r;
			o.Smoothness	= ms.g;
			o.Occlusion		= getOcclusion(IN.uv_MainTex);
			o.Normal		= getNormalMap(IN.uv_MainTex);

			// Emission
			o.Emission	= getEmission(IN.uv_MainTex);
		}
		ENDCG
	}
	FallBack "Diffuse"
	CustomEditor "ShaderGUITree.TestShaderGUI"
}
