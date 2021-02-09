Shader "UnityChan/Hair - Double-sided"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_ShadowColor ("Shadow Color", Color) = (0.8, 0.8, 1, 1)
		_SpecularPower ("Specular Power", Float) = 20
		_EdgeThickness ("Outline Thickness", Float) = 1
		
		_MainTex ("Diffuse", 2D) = "white" {}
		_FalloffSampler ("Falloff Control", 2D) = "white" {}
		_RimLightSampler ("RimLight Control", 2D) = "white" {}
		_SpecularReflectionSampler ("Specular / Reflection Mask", 2D) = "white" {}
		_EnvMapSampler ("Environment Map", 2D) = "" {} 
		_NormalMapSampler ("Normal Map", 2D) = "" {} 

		//�������
		_Diffuse("Diffuse", Color) = (1,1,1,1)
		_OutlineCol("OutlineCol", Color) = (1,0,0,1)
		_OutlineFactor("OutlineFactor", Range(0,1)) = 0.1
	}

	SubShader
	{
		//���ʹ������Pass����һ��pass�ط��߼���һ�㣬ֻ�����ߵ���ɫ
		Pass
		{
			//�޳����棬ֻ��Ⱦ���棬���ڴ����ģ�����ã����������Ҫ����ģ�����������
			Cull Front
			
			CGPROGRAM
			#include "UnityCG.cginc"
			fixed4 _OutlineCol;
			float _OutlineFactor;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			
			v2f vert(appdata_full v)
			{
				v2f o;
				//��vertex�׶Σ�ÿ�����㰴�շ��ߵķ���ƫ��һ���֣��������ֻ���ɽ���ԶС��͸������
				//v.vertex.xyz += v.normal * _OutlineFactor;
				o.pos = UnityObjectToClipPos(v.vertex);
				//�����߷���ת�����ӿռ�
				float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				//���ӿռ䷨��xy����ת����ͶӰ�ռ䣬ֻ��xy��Ҫ��z��Ȳ���Ҫ��
				float2 offset = TransformViewToProjection(vnormal.xy);
				//������ͶӰ�׶��������ƫ�Ʋ���
				o.pos.xy += offset * _OutlineFactor;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				//���Passֱ����������ɫ
				return _OutlineCol;
			}
			
			//ʹ��vert������frag����
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
		
		//������ɫ��Pass
		Pass
		{
			CGPROGRAM	
		
			//����ͷ�ļ�
			#include "Lighting.cginc"
			//����Properties�еı���
			fixed4 _Diffuse;
			sampler2D _MainTex;
			//ʹ����TRANSFROM_TEX�����Ҫ����XXX_ST
			float4 _MainTex_ST;
 
			//����ṹ�壺vertex shader�׶����������
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};
 
			//���嶥��shader,����ֱ��ʹ��appdata_base������position, noramal, texcoord��
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//ͨ��TRANSFORM_TEX��ת���������꣬��Ҫ������Offset��Tiling�ĸı�,Ĭ��ʱ��ͬ��o.uv = v.texcoord.xy;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				return o;
			}
 
			//����ƬԪshader
			fixed4 frag(v2f i) : SV_Target
			{
				//unity�����diffuseҲ�Ǵ��˻����⣬��������Ҳ����һ�»�����
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Diffuse.xyz;
				//��һ�����ߣ���ʹ��vert��һ��Ҳ���У���vert��frag�׶��в�ֵ��������ķ��߷��򲢲���vertex shaderֱ�Ӵ�����
				fixed3 worldNormal = normalize(i.worldNormal);
				//�ѹ��շ����һ��
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				//���ݰ�������ģ�ͼ������صĹ�����Ϣ
				fixed3 lambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
				//���������ɫΪlambert��ǿ*����diffuse��ɫ*����ɫ
				fixed3 diffuse = lambert * _Diffuse.xyz * _LightColor0.xyz + ambient;
				//�����������
				fixed4 color = tex2D(_MainTex, i.uv);
				color.rgb = color.rgb* diffuse;
				return fixed4(color);
			}
 
			//ʹ��vert������frag����
			#pragma vertex vert
			#pragma fragment frag	
 
			ENDCG
		}
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
			"LightMode"="ForwardBase"
		}		

		Pass
		{
			Cull Off
			ZTest LEqual
CGPROGRAM
#pragma multi_compile_fwdbase
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#define ENABLE_NORMAL_MAP
#include "CharaMain.cg"
ENDCG
		}

		Pass
		{
			Cull Front
			ZTest Less
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "CharaOutline.cg"
ENDCG
		}

	}

	FallBack "Transparent/Cutout/Diffuse"
}
