sampler2D implicitInput : register(s0);
float factor : register(c0);

float gToned = 1.0f;
float3 gLightColor = { 1, 0.9, 0.5 };
float3 gDarkColor = { 0.2, 0.05, 0 };

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(implicitInput, uv);
    float4 outputColor = color;
    outputColor.r = (color.r * 0.393) + (color.g * 0.769) + (color.b * 0.189);
    outputColor.g = (color.r * 0.349) + (color.g * 0.686) + (color.b * 0.168);
    outputColor.b = (color.r * 0.272) + (color.g * 0.534) + (color.b * 0.131);
    outputColor.a = color.a;

    float gray = color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
    
    float4 muted;
    muted.r = (color.r - gray) * factor + gray;
    muted.g = (color.g - gray) * factor + gray;
    muted.b = (color.b - gray) * factor + gray;
    muted.a = color.a;

    float4 result = lerp(muted, outputColor, factor);
    
    return result;
}