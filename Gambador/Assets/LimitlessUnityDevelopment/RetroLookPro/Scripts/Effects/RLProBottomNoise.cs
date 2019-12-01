using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(RLPRO_SRP_BottomNoise_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Bottom Noise Effect", false)]
public sealed class RLProBottomNoise : PostProcessEffectSettings
{
    [Range(0.01f, 0.5f), Tooltip("Height of Noise.")]
    public FloatParameter height = new FloatParameter { value = 0.2f };
    [Range(0f, 3f), Tooltip("Noise intensity.")]
    public FloatParameter intencity = new FloatParameter { value = 1.5f };

    public TextureParameter noiseTexture = new TextureParameter { value = null};
}

public sealed class RLPRO_SRP_BottomNoise_Renderer : PostProcessEffectRenderer<RLProBottomNoise>
{
     private float T;
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/BottomNoiseEffect"));
        sheet.properties.SetFloat("_OffsetNoiseX",UnityEngine.Random.Range(0f, 1.0f));
        float offsetNoise1 = sheet.properties.GetFloat("_OffsetNoiseY");
        sheet.properties.SetFloat("_OffsetNoiseY", offsetNoise1 + UnityEngine.Random.Range(-0.05f, 0.05f));
        sheet.properties.SetFloat("_NoiseBottomHeight", settings.height);
        
        sheet.properties.SetFloat("_NoiseBottomIntensity", settings.intencity);
        if(settings.noiseTexture.value!= null)
        sheet.properties.SetTexture("_SecondaryTex", settings.noiseTexture);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
