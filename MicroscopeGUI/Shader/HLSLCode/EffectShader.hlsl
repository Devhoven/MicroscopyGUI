sampler2D Frame: register(s0);
float Brightness: register(c0);
float Contrast: register(c1);
float AmountR: register(c2);
float AmountG: register(c3);
float AmountB: register(c4);

float4 main(float2 uv:TEXCOORD) : COLOR
{
	float4 pixelColor = tex2D(Frame, uv);

	pixelColor.r *= AmountR;
	pixelColor.g *= AmountG;
	pixelColor.b *= AmountB;

	// Apply contrast.
	pixelColor.rgb = ((pixelColor.xyz - 0.5f) * max(Contrast, 0)) + 0.5f;

	// Apply brightness.
	pixelColor.rgb += Brightness;

	return pixelColor;
}