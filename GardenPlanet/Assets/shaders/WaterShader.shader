// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterShader"
{
	Properties
	{
		_Depth("Depth", Range( 0 , 1)) = 0.5
		_WaterColor("Water Color", Color) = (0.01013041,0,1,0)
		_FoamColor("Foam Color", Color) = (0.8867924,0,0,0)
		_FoamMin("Foam Min", Range( 0 , 1)) = 0
		_FoamMax("Foam Max", Range( 0 , 1)) = 1
		_DepthMin("Depth Min", Range( 0 , 1)) = 0
		_DepthMax("Depth Max", Range( 0 , 1)) = 0
		_FoamPower("Foam Power", Range( 0 , 20)) = 10
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Tesselation("Tesselation", Range( 0 , 10)) = 1
		_Refractian("Refractian", Range( 0 , 10)) = 0
		_Emission("Emission", Color) = (0,0,0,0)
		_EmissionAmount("Emission Amount", Range( 0 , 1)) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_OverlayColour("OverlayColour", Color) = (0,0,0,0)
		_OverlayAmount("OverlayAmount", Range( 0 , 1)) = 0
		_OverlayPan("OverlayPan", Range( 0 , 1)) = 0
		_LightEmissionAmount("LightEmissionAmount", Range( 0 , 5)) = 0
		_WaveFrequency("WaveFrequency", Range( 0 , 1)) = 0
		_WaveAmplification("WaveAmplification", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 viewDir;
			float3 worldNormal;
		};

		uniform float _WaveFrequency;
		uniform float _WaveAmplification;
		uniform sampler2D _TextureSample0;
		uniform float _OverlayPan;
		uniform float4 _OverlayColour;
		uniform float _OverlayAmount;
		uniform float4 _FoamColor;
		uniform float4 _WaterColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth;
		uniform float _LightEmissionAmount;
		uniform float _Refractian;
		uniform float4 _Emission;
		uniform float _EmissionAmount;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _FoamMin;
		uniform float _FoamMax;
		uniform float _FoamPower;
		uniform float _DepthMin;
		uniform float _DepthMax;
		uniform float _Tesselation;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_0 = (_Tesselation).xxxx;
			return temp_cast_0;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 break3_g1 = ase_vertex3Pos;
			float mulTime4_g1 = _Time.y * 2.0;
			float3 appendResult11_g1 = (float3(break3_g1.x , ( break3_g1.y + ( sin( ( ( break3_g1.x * _WaveFrequency ) + mulTime4_g1 ) ) * _WaveAmplification ) ) , break3_g1.z));
			v.vertex.xyz += appendResult11_g1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_OverlayPan).xx;
			float2 panner83 = ( _Time.y * temp_cast_0 + i.uv_texcoord);
			float4 tex2DNode77 = tex2D( _TextureSample0, panner83 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth9 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth9 = abs( ( screenDepth9 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float dotResult12 = dot( i.viewDir , ase_vertexNormal );
			float clampResult14 = clamp( ( distanceDepth9 / dotResult12 ) , 0.0 , 1.0 );
			float4 lerpResult3 = lerp( _FoamColor , _WaterColor , clampResult14);
			o.Albedo = ( ( ( tex2DNode77 * _OverlayColour ) * _OverlayAmount ) + ( lerpResult3 + float4( 0,0,0,0 ) ) + ( ( 1.0 - tex2DNode77 ) * 0.5 ) ).rgb;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			o.Emission = ( ( _LightEmissionAmount * ase_lightColor ) + ( ( float4( refract( i.viewDir , i.viewDir , _Refractian ) , 0.0 ) * _Emission ) * _EmissionAmount ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float clampResult25 = clamp( (0.0 + (clampResult14 - _FoamMin) * (1.0 - 0.0) / (_FoamMax - _FoamMin)) , 0.0 , 1.0 );
			float clampResult31 = clamp( ( ( 1.0 - clampResult25 ) * _FoamPower ) , 0.0 , 1.0 );
			float clampResult29 = clamp( ( ( clampResult31 * 0.5 ) + (0.0 + (clampResult14 - _DepthMin) * (1.0 - 0.0) / (_DepthMax - _DepthMin)) ) , 0.0 , 1.0 );
			o.Alpha = clampResult29;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
180;914;1599;791;826.8677;690.4982;1.359998;True;True
Node;AmplifyShaderEditor.NormalVertexDataNode;11;-1846.555,1336.329;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-1952.934,1029.117;Float;False;Property;_Depth;Depth;0;0;Create;True;0;0;False;0;0.5;0.586;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;10;-1853.555,1158.329;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DepthFade;9;-1624.17,1034.479;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;12;-1595.554,1202.329;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;13;-1432.555,1109.328;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;14;-1121.563,1102.587;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1286.502,1324.885;Float;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1363.945,996.5127;Float;False;Property;_FoamMax;Foam Max;4;0;Create;True;0;0;False;0;1;0.844;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1362.886,904.3569;Float;False;Property;_FoamMin;Foam Min;3;0;Create;True;0;0;False;0;0;0.484;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1270.731,1245.44;Float;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;15;-854.4013,960.7957;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;25;-644.8923,963.8994;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-660.5604,1095.352;Float;False;Property;_FoamPower;Foam Power;7;0;Create;True;0;0;False;0;10;9.4;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;41;-469.5353,981.9612;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-283.6494,972.9731;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-1037.269,-301.8243;Float;False;Property;_OverlayPan;OverlayPan;17;0;Create;True;0;0;False;0;0;0.015;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;86;-905.3425,-486.9223;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;84;-814.1055,-411.4547;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;83;-639.5164,-526.3454;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;71;-166.7177,410.0117;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;70;-168.9228,553.2101;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;72;-250.1001,704.0209;Float;False;Property;_Refractian;Refractian;11;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;31;-132.6491,967.9731;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-488.2731,21.99411;Float;False;Property;_WaterColor;Water Color;1;0;Create;True;0;0;False;0;0.01013041,0,1,0;0.2382966,0.4811321,0.4027474,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;79;-338.9193,-358.112;Float;False;Property;_OverlayColour;OverlayColour;15;0;Create;True;0;0;False;0;0,0,0,0;0.01415092,1,0.06544802,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;105.6818,1010.194;Float;False;Constant;_Float5;Float 5;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-37.51842,1212.72;Float;False;Property;_DepthMax;Depth Max;6;0;Create;True;0;0;False;0;0;0.414;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;96.69294,1291.199;Float;False;Constant;_Float4;Float 4;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;77;-439.0198,-546.6121;Float;True;Property;_TextureSample0;Texture Sample 0;14;0;Create;True;0;0;False;0;None;93927dc8e0735004d87cd70836332b14;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-34.10515,1144.476;Float;False;Property;_DepthMin;Depth Min;5;0;Create;True;0;0;False;0;0;0.18;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;26;39.44898,933.1128;Float;False;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;73;54.49058,613.6505;Float;False;Property;_Emission;Emission;12;0;Create;True;0;0;False;0;0,0,0,0;0.1581969,0.3995927,0.4245283,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;-38.65532,1290.062;Float;False;Constant;_Float3;Float 3;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-419.6724,-164.8058;Float;False;Property;_FoamColor;Foam Color;2;0;Create;True;0;0;False;0;0.8867924,0,0,0;0.3089177,0.735849,0.6452379,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RefractOpVec;69;31.87588,494.2101;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-32.11988,-489.412;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;228.8327,509.9562;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;3;-82.4426,-2.605786;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;280.6819,955.1941;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-90.61987,-356.812;Float;False;Property;_OverlayAmount;OverlayAmount;16;0;Create;True;0;0;False;0;0;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;20;231.9498,1193.66;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;8.72284,786.0114;Float;False;Property;_EmissionAmount;Emission Amount;13;0;Create;True;0;0;False;0;0;0.46;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;13.68877,245.4523;Float;False;Property;_LightEmissionAmount;LightEmissionAmount;18;0;Create;True;0;0;False;0;0;0.02;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;92;95.52487,336.5114;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;100;331.8504,-561.2979;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;99;418.89,-270.2586;Float;False;Constant;_Float0;Float 0;21;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;238.011,1685.418;Float;False;Property;_WaveFrequency;WaveFrequency;19;0;Create;True;0;0;False;0;0;0.788;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;116.0801,-490.7121;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;96;227.1307,1824.137;Float;False;Property;_WaveAmplification;WaveAmplification;20;0;Create;True;0;0;False;0;0;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;359.5595,509.5495;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;94;316.891,1546.698;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;35;450.4306,940.0886;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;148.9192,43.41312;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;524.9697,-496.0181;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;340.0241,406.4524;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;42;297.2651,1467.303;Float;False;Property;_Tesselation;Tesselation;10;0;Create;True;0;0;False;0;1;4.25;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;82;372.4526,-13.05983;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;29;581.882,958.594;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;345.1804,830.942;Float;False;Property;_Smoothness;Smoothness;8;0;Create;True;0;0;False;0;0;0.227;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;90;484.0241,426.4524;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;39;425.3138,651.9857;Float;False;Property;_Metallic;Metallic;9;0;Create;True;0;0;False;0;0;0.336;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;93;565.2891,1516.779;Float;False;Waving Vertex;-1;;1;872b3757863bb794c96291ceeebfb188;0;3;1;FLOAT3;0,0,0;False;12;FLOAT;1;False;13;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;942.3766,466.0579;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;WaterShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;0;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;7;0
WireConnection;12;0;10;0
WireConnection;12;1;11;0
WireConnection;13;0;9;0
WireConnection;13;1;12;0
WireConnection;14;0;13;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;15;2;17;0
WireConnection;15;3;18;0
WireConnection;15;4;19;0
WireConnection;25;0;15;0
WireConnection;41;0;25;0
WireConnection;30;0;41;0
WireConnection;30;1;32;0
WireConnection;83;0;86;0
WireConnection;83;2;85;0
WireConnection;83;1;84;0
WireConnection;31;0;30;0
WireConnection;77;1;83;0
WireConnection;26;0;31;0
WireConnection;69;0;71;0
WireConnection;69;1;70;0
WireConnection;69;2;72;0
WireConnection;78;0;77;0
WireConnection;78;1;79;0
WireConnection;74;0;69;0
WireConnection;74;1;73;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;14;0
WireConnection;27;0;26;0
WireConnection;27;1;28;0
WireConnection;20;0;14;0
WireConnection;20;1;21;0
WireConnection;20;2;22;0
WireConnection;20;3;23;0
WireConnection;20;4;24;0
WireConnection;100;0;77;0
WireConnection;80;0;78;0
WireConnection;80;1;81;0
WireConnection;75;0;74;0
WireConnection;75;1;76;0
WireConnection;35;0;27;0
WireConnection;35;1;20;0
WireConnection;38;0;3;0
WireConnection;98;0;100;0
WireConnection;98;1;99;0
WireConnection;91;0;89;0
WireConnection;91;1;92;0
WireConnection;82;0;80;0
WireConnection;82;1;38;0
WireConnection;82;2;98;0
WireConnection;29;0;35;0
WireConnection;90;0;91;0
WireConnection;90;1;75;0
WireConnection;93;1;94;0
WireConnection;93;12;95;0
WireConnection;93;13;96;0
WireConnection;0;0;82;0
WireConnection;0;2;90;0
WireConnection;0;3;39;0
WireConnection;0;4;40;0
WireConnection;0;9;29;0
WireConnection;0;11;93;0
WireConnection;0;14;42;0
ASEEND*/
//CHKSM=14CD4AE3A9D5F177CA2A8AA6F09498877656FFDF