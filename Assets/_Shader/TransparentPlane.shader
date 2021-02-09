Shader "Custom/TransparentPlane"
{
	Properties
	{
		_AlphaScale("cutoff",Range(0,1)) = 0.5//透明程度
		_MainTex("main texture",2D) = "white"{}
		_Color("diffuse",Color) = (1,1,1,1)
		_Specular("specular",Color) = (1,1,1,1)
		_Gloss("gloss",Range(1,256)) = 20

		_ShadowColor("Shadow Color", Color) = (1, 1, 1, 1)
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
			LOD 100

			Pass//ForwardBase
			{
				Tags{"LightMode" = "ForwardBase"}
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma multi_compile_fwdbase
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				fixed4 _Color;
				fixed4 _Specular;
				float _Gloss;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _AlphaScale;
				
				fixed4 _ShadowColor;

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal:NORMAL;
					float2 uv:TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float3 worldNormal:TEXCOORD0;
					float3 worldPos:TEXCOORD1;
					float2 uv:TEXCOORD2;
					SHADOW_COORDS(3)//仅仅是阴影
				};


				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					TRANSFER_SHADOW(o);//仅仅是阴影
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

					float3 worldNormal = normalize(i.worldNormal);
					float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
					float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

					fixed4 texColor = tex2D(_MainTex, i.uv);

					fixed3 diffuse = texColor.rgb*_LightColor0.rgb*_Color.rgb*saturate(dot(worldNormal, worldLightDir));

					fixed3 halfDir = normalize(viewDir + worldLightDir);
					fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(saturate(dot(worldNormal, halfDir)), _Gloss);

					//fixed shadow = SHADOW_ATTENUATION(i);
					//return fixed4(ambient + (diffuse + specular)*shadow + i.vertexLight, 1);

					//这个函数计算包含了光照衰减以及阴影,因为ForwardBase逐像素光源一般是方向光，衰减为1，atten在这里实际是阴影值
					UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
					//修改阴影颜色
					fixed3 fixedShadowColor = fixed3(atten < 1 ? _ShadowColor.r : 1, atten < 1 ? _ShadowColor.g : 1, atten < 1 ? _ShadowColor.b : 1);
					return fixed4(ambient + (diffuse + specular) * fixedShadowColor, texColor.a*_AlphaScale);
				}
				ENDCG
			}

			//Pass//ForwardAdd
			//{
			//	Tags{"LightMode" = "ForwardAdd"}
			//	Blend One One

			//	CGPROGRAM
			//	#pragma multi_compile_fwdadd_fullshadows
			//	#pragma vertex vert
			//	#pragma fragment frag

			//	#include"UnityCG.cginc"
			//	#include"Lighting.cginc"
			//	#include"AutoLight.cginc"

			//	float4 _Diffuse;
			//	float4 _Specular;
			//	float _Gloss;

			//	struct a2v
			//	{
			//		float4 vertex:POSITION;
			//		float3 normal:NORMAL;
			//	};

			//	struct v2f
			//	{
			//		float4 pos:SV_POSITION;
			//		float3 worldPos:TEXCOORD0;
			//		float3 worldNormal:TEXCOORD1;
			//		LIGHTING_COORDS(2, 3)//包含光照衰减以及阴影
			//	};

			//	v2f vert(a2v v)
			//	{
			//		v2f o;
			//		o.pos = UnityObjectToClipPos(v.vertex);
			//		o.worldNormal = UnityObjectToWorldNormal(v.normal);
			//		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			//		TRANSFER_VERTEX_TO_FRAGMENT(o);//包含光照衰减以及阴影
			//		return o;
			//	}

			//	fixed4 frag(v2f i) :SV_Target
			//	{
			//		float3 worldNormal = normalize(i.worldNormal);
			//		float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

			//		fixed3 diffuse = _LightColor0.rgb*_Diffuse.rgb*saturate(dot(worldNormal, worldLightDir));

			//		float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
			//		float3 halfDir = normalize(viewDir + worldLightDir);
			//		fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(saturate(dot(worldNormal, halfDir)), _Gloss);
			//		//fixed atten = LIGHT_ATTENUATION(i);
			//		UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);//包含光照衰减以及阴影
			//		return fixed4((diffuse + specular)*atten, 1);
			//	}
			//	ENDCG
			//}

			
		}
		FallBack "Diffuse"
		//FallBack "Transparent/Cutout/VertexLit"
}