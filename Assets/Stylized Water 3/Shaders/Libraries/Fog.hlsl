// Stylized Water 3 by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//    • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//    • Uploading this file to a public repository will subject it to an automated DMCA takedown request.

float _WaterFogDisabled;

//Authors of third-party fog solutions can reach out to have their method integrated here

#ifdef SCPostEffects
//Macros normally used for cross-RP compatibility
#define LINEAR_DEPTH(depth) Linear01Depth(depth, _ZBufferParams)

//Legacy (pre v2.2.1)
#define DECLARE_TEX(textureName) TEXTURE2D(textureName);
#define DECLARE_RT(textureName) TEXTURE2D_X(textureName);
#define SAMPLE_TEX(textureName, samplerName, uv) SAMPLE_TEXTURE2D_LOD(textureName, samplerName, uv, 0)
#define SAMPLE_RT_LOD(textureName, samplerName, uv, mip) SAMPLE_TEXTURE2D_X_LOD(textureName, samplerName, uv, mip)
#endif

#ifdef AtmosphericHeightFog
//For versions older than 3.2.0, uncomment this
//bool AHF_Enabled;
#endif

//Fragment stage. Note: Screen position passed here is not normalized (divided by w-component)
void ApplyFog(inout float3 color, float fogFactor, float4 screenPos, float3 positionWS, float vFace) 
{
	float3 foggedColor = color;

	float2 normalizedUV = screenPos.xy / screenPos.w;
#ifdef UnityFog
	foggedColor = MixFog(color.rgb, fogFactor);
#endif

#ifdef Colorful
	if(_DensityParams.x > 0) foggedColor.rgb = ApplyFog(color.rgb, fogFactor, positionWS, normalizedUV);
#endif
	
#ifdef Enviro
	//Distance/height fog enabled?
	if (_EnviroParams.y > 0 || _EnviroParams.z > 0)
	{
		foggedColor.rgb = TransparentFog(float4(color.rgb, 1.0), positionWS, normalizedUV, fogFactor).rgb;
	}
#endif

#ifdef Enviro3
	if(_EnviroFogParameters.z > 0) //Fog density 1
	{
		foggedColor.rgb = ApplyFogAndVolumetricLights(color.rgb, normalizedUV, positionWS, 0);
		foggedColor.rgb = ApplyClouds(foggedColor.rgb, normalizedUV, positionWS);
	}
#endif
	
#ifdef Azure
	foggedColor.rgb = ApplyAzureFog(float4(color.rgb, 1.0), positionWS).rgb;
#endif

#ifdef AtmosphericHeightFog
	if (AHF_Enabled)
	{
		float4 fogParams = GetAtmosphericHeightFog(positionWS.xyz);
		foggedColor.rgb = lerp(color.rgb, fogParams.rgb, fogParams.a);
	}
#endif

#ifdef SCPostEffects
	//Distance or height fog enabled
	if(_DistanceParams.z == 1 || _DistanceParams.w == 1)
	{
		ApplyTransparencyFog(positionWS, normalizedUV, foggedColor.rgb);
	}
#endif

#ifdef COZY
	foggedColor = BlendStylizedFog(positionWS, float4(color.rgb, 1.0)).rgb;
#endif

#ifdef Buto
	#if defined(BUTO_API_VERSION_2) //Buto 2022
	float3 positionVS = TransformWorldToView(positionWS);
	foggedColor = ButoFogBlend(normalizedUV, -positionVS.z, color.rgb);
	#else //Buto 2021
	foggedColor = ButoFogBlend(normalizedUV, color.rgb);
	#endif
#endif

	#ifndef UnityFog
	//Allow fog to be disabled for water globally by setting the value through script
	foggedColor = lerp(foggedColor, color, _WaterFogDisabled);
	#endif
	
	//Fog only applies to the front faces, otherwise affects underwater rendering
	color.rgb = lerp(color.rgb, foggedColor.rgb, vFace);
}
