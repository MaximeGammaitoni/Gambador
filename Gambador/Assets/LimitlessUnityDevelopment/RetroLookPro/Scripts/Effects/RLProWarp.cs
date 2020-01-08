using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_Warp_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Warp", false)]
public sealed class RLProWarp : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Dark areas adjustment.")]
    public FloatParameter fade = new FloatParameter { value = 1f };
    [Tooltip("Warp mode.")]
    public WarpModeParameter warpMode = new WarpModeParameter {};
    [Tooltip("Warp picture.")]
    public Vector2Parameter warp = new Vector2Parameter {value = new Vector2(0.03125f,0.04166f)};
public FloatParameter scale = new FloatParameter { value = 1f };
}

public sealed class RLPRO_SRP_Warp_Renderer : PostProcessEffectRenderer<RLProWarp>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Warp_RLPro"));
        sheet.properties.SetFloat("fade", settings.fade);
        sheet.properties.SetFloat("scale", settings.scale);
        sheet.properties.SetVector("warp", settings.warp);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.warpMode == WarpMode.SimpleWarp?0:1);
    }
}
