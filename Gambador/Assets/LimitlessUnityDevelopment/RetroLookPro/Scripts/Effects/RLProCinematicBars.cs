using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(CinematicBars_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Cinematic Bars", false)]
public sealed class RLProCinematicBars : PostProcessEffectSettings
{
    [Range(0.5f, 0.01f), Tooltip("")]
    public FloatParameter amount = new FloatParameter { value = 0.5f };
     [Range(0f, 1f), Tooltip("Intensity of noise texture.")]
    public FloatParameter fade = new FloatParameter { value = 1f };

}
public sealed class CinematicBars_Renderer : PostProcessEffectRenderer<RLProCinematicBars>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/CinematicBars"));
      
        sheet.properties.SetFloat("_Stripes", settings.amount);
        sheet.properties.SetFloat("_Fade", settings.fade);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}