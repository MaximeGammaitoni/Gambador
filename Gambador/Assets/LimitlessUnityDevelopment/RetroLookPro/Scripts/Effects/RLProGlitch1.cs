using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(Glitch1Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Glitch1", false)]
public sealed class RLProGlitch1 : PostProcessEffectSettings
{
    [Range(0f, 2f), Tooltip("")]
    public FloatParameter stretch = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip(".")]
    public FloatParameter speed = new FloatParameter { value = 0.5f };
    [Range(0f, 1f), Tooltip(".")]
    public FloatParameter fade = new FloatParameter { value = 0.5f };
}

public sealed class Glitch1Renderer : PostProcessEffectRenderer<RLProGlitch1>
{
     private float T;
    public override void Render(PostProcessRenderContext context)
    {
                    T += Time.deltaTime;
            if (T > 100) T = 0;
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Glitch1RetroLook"));
        sheet.properties.SetFloat("Strength", settings.stretch);
        sheet.properties.SetFloat("Speed", settings.speed);
        sheet.properties.SetFloat("Fade", settings.fade);
        sheet.properties.SetFloat("T", T);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}