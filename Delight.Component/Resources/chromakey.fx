// Uni
sampler2D uni_Texture : register(s0);
float uni_h1 : register(c0);
float uni_h2 : register(c1);
float uni_saturationMin : register(c2);
float uni_saturationMax : register(c3);
float uni_luminanceMin : register(c4);
float uni_luminanceMax : register(c5);
float uni_Smooth : register(c6);


bool inHUEInterval(float pHUE)
// Check HUE interval
{
	return (pHUE >= uni_h1) && (pHUE <= uni_h2);
}

bool inSaturationInterval(float pSat)
// Check saturation interval
{
	return (pSat >= uni_saturationMin) && (pSat <= uni_saturationMax);
}

int inValueInterval(float pVal)
// Check value interval
{
	if (pVal >= uni_luminanceMin && pVal <= uni_luminanceMax)
		return 0;
	else if (pVal < uni_luminanceMin)
		return -1;
	else
		return 1;
}

float4 rgb_to_hsv(float4 color)
{
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(color.bg, K.wz), float4(color.gb, K.xy), step(color.b, color.g));
	float4 q = lerp(float4(p.xyw, color.r), float4(color.r, p.yzx), step(p.x, color.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float4(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x, color[3]);
}

float4 hsv_to_rgb(float4 hsv)
{
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
	return float4(hsv.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), hsv.y), hsv[3]);
}


float4 process(float4 pColor)
{
	float4 lHSV = rgb_to_hsv(pColor);
	float lSH1 = max(0.0f, uni_h1 - uni_Smooth);
	float lSH2 = min(1.0f, uni_h2 + uni_Smooth);

	if (inHUEInterval(lHSV.r))
		// if HUE is between HUE tolerance Min/Max
	{
		if (inSaturationInterval(lHSV.g))
			// if Saturation is between Saturation Min/Max
		{
			if (inValueInterval(lHSV.b) == 0)
				// if Value is between minValue/maxValue => color delete
				pColor = float4(0, 0, 0, 0);
			else if (inValueInterval(lHSV.b) == -1)
				// if Value < minValue => color dark
			{
				lHSV.a = min(1.0f, uni_luminanceMin + 1.0f - (lHSV.b / uni_luminanceMin)); // calcul alpha
				lHSV.b = 0.0f;
				pColor = hsv_to_rgb(lHSV);
			}
			else
				// if Value > maxValue => color lum
			{
				lHSV.a = min(1.0f, ((lHSV.b - uni_luminanceMax) / (1.0f - uni_luminanceMax))); // calcul alpha
				lHSV.b = 1.0f;
				pColor = hsv_to_rgb(lHSV);
			}
		}
	}
	// Smooth test (on HUE index)
	else if (lHSV.r >= lSH1 && lHSV.r < uni_h1)
		pColor.a = smoothstep(uni_h1, lSH1, lHSV.r);
	else if (lHSV.r <= lSH2 && lHSV.r > uni_h2)
		pColor.a = smoothstep(uni_h2, lSH2, lHSV.r);
	return pColor * float4(pColor.aaa, 1.0f);
}


float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 lRealColor = tex2D(uni_Texture, uv);
	return process(lRealColor);
}
