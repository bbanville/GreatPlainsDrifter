/*******************************************************************************
File: Outline.shader
Author: Jack Makarov
DP Email: jack.m@digipen.edu
Date: 9/27/2020
Course: CS176
Section: A
Description:
 This is an outline shader. Made following the tutorial on
 https://roystan.net/articles/outline-shader.html
*******************************************************************************/

Shader "Hidden/Jack/Outline Post Process"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

			TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
			TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture);
			TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

			float4 _MainTex_TexelSize;
			
			float _Scale;
			float4 _Color;

			float _DepthThreshold;
			float _DepthNormalThreshold;
			float _DepthNormalThresholdScale;

			float _NormalThreshold;

			float4x4 _ClipToView;


			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

			//we copy over the vertex shader from the StdLib.hlsl to calculate the
			//view direction in view space. We also make some modifications
			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
				float3 viewSpaceDir : TEXCOORD2;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
			};

			Varyings Vert(AttributesDefault v)
			{
				Varyings o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
				//clip space position in o.vertex can be interpreted as a camera's view direction
				//to each pixel. So we gonnna multiply it by our ClipToView matrix to transform
				//the direction to view space
				o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;

			#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
			#endif

				o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

				return o;
			}

			float4 Frag(Varyings i) : SV_Target
			{
				//try to detect edges using depth buffer
				//we'll sample pixels from the depth buffer in an X shape
				//that will be roughly centered on the current pixel we are rendering
				float halfScaleFloor = floor(_Scale * 0.5);
				float halfScaleCeil = ceil(_Scale * 0.5);

				//scaling UVs this way we should be able to increment our edge with exaclty one pixel at a time
				//achieveing max possible granularity, while still being centered around i.texcoord
				float2 bottomLeftUV = i.texcoord - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
				float2 topRightUV = i.texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;
				float2 bottomRightUV = i.texcoord + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
				float2 topLeftUV = i.texcoord + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

				//sample the normals buffer using our UV coordinates
				//SAMPLE_DEPTH_TEXTURE is a macro that we use on camera's normals buffer
				//to get the texture we have the script RRSTT.cs attached to our camera
				//it generates a camera to render the view-space normals of the scene into _CameraNormalsTexture
				float3 normal0 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomLeftUV).rgb;
				float3 normal1 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topRightUV).rgb;
				float3 normal2 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomRightUV).rgb;
				float3 normal3 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topLeftUV).rgb;

				//sample the deapth texture using our UV coordinates
				//SAMPLE_DEPTH_TEXTURE is a macro that we use on camera's depth texture
				//we only take r channel, as depth is a scalar value with a 0-1 range
				//depth is non-linear, further from the camera smaller depth values represent greater distances
				float depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomLeftUV).r;
				float depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topRightUV).r;
				float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomRightUV).r;
				float depth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topLeftUV).r;

				//modulate depthThreshold based on the difference between the camera's viewing normal and the normal of the surface
				//using dot product. View normal is sampled with the range 0..1, while viewSpaceDir is in the -1..1 range
				//so we transform the viewNormal to be in the same range and then take the dot product of both
				float3 viewNormal = normal0 * 2 - 1;
				float NdotV = 1 - dot(viewNormal, -i.viewSpaceDir);

				//cutoff lower bound since we don't need to modify the threshold of surfaces that are mostly facing the camera
				//and rescale it to be 0..1. This way we only apply the new threshold when the surfaces are above certain angle
				//from the camera
				float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
				//now re-range it to be from 1 to an upper bound _DepthNormalThresholdScale instead of 0..1
				float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;
				//we jump through all these loops to get rid of the surface artifacting that happens when they face
				//the camera at certain angles

				//compare the depth of pixels across from each other via substraction
				//it doesn't matter if it's positive or negative, we will be using the absolute value
				//or have it squared. Each of 2 represent 1 half of the detected edges.
				float depthFiniteDifference0 = depth1 - depth0;
				float depthFiniteDifference1 = depth3 - depth2;
				//combine the above 2 using part of an edge detection operator called the Roberts cross
				float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
				//modulate the _DepthThreshold based on the existing depth of the surfaces
				//and get rid of the dark greys
				float depthThreshold = _DepthThreshold * depth0 * normalThreshold;
				edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

				//same as before for depth we do for normals
				float3 normalFiniteDifference0 = normal1 - normal0;
				float3 normalFiniteDifference1 = normal3 - normal2;
				//then combine them in a similarish fashion. Since normals are vectors and not scalars like depth
				//we need to transform them from a 3d value to a single dimensional value before calculating the
				//edge intensity. The dot product is what we want, it returns a scalar, but also by doing it for each
				//of the FiniteDifferences on itself we are also squaring the value
				float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
				edgeNormal = edgeNormal > _NormalThreshold ? 1 : 0;

				//combine the results of our depth and normal buffer shenanigans
				float edge = max(edgeDepth, edgeNormal);

				//control the color of the outlines
				float4 edgeColor = float4(_Color.rgb, _Color.a * edge);

				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
				
				//finally apply the outlines
				return alphaBlend(edgeColor, color);
			}
			ENDHLSL
		}
	}
}