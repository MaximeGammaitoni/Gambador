using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(Glitch3Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Glitch3", false)]
public sealed class RLProGlitch3 : PostProcessEffectSettings
{
    [ Tooltip("")]
    public FloatParameter speed = new FloatParameter { value = 1f };
    [ Tooltip(".")]
    public FloatParameter density = new FloatParameter { value = 1f };

    [Tooltip(".")]
    public FloatParameter maxDisplace = new FloatParameter { value = 1f };
}

public sealed class Glitch3Renderer : PostProcessEffectRenderer<RLProGlitch3>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Glitch3"));
        sheet.properties.SetFloat("speed", settings.speed);
        sheet.properties.SetFloat("density", settings.density);
        sheet.properties.SetFloat("maxDisplace", settings.maxDisplace);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}