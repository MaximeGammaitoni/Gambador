using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RLProVHSScanlinesRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/VHS Scanlines", false)]
public sealed class RLProVHSScanlines : PostProcessEffectSettings
{
    [Tooltip(".")]
    public ColorParameter scanLinesColor = new ColorParameter { };

    public FloatParameter scanLines = new FloatParameter { value = 1.5f };

    public FloatParameter speed = new FloatParameter { value = 0f };

    public FloatParameter fade = new FloatParameter { value = 0f };

    public BoolParameter horizontal = new BoolParameter { value = true };
    [Range(3500f, 1f), Tooltip(".")]
    public FloatParameter distortion = new FloatParameter { value = 0f };
    public FloatParameter distortion1 = new FloatParameter { value = 0f };
    public FloatParameter distortion2 = new FloatParameter { value = 0f };
    public FloatParameter scale = new FloatParameter { value = 0f };
}

public sealed class RLProVHSScanlinesRenderer : PostProcessEffectRenderer<RLProVHSScanlines>
{
    private int pass;
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/VHSScanlines_RLPro"));

        sheet.properties.SetFloat("_ScanLines", settings.scanLines);
        sheet.properties.SetFloat("speed", settings.speed);
        sheet.properties.SetFloat("_OffsetDistortion", settings.distortion);
        sheet.properties.SetFloat("fade", settings.fade);
        sheet.properties.SetFloat("sferical", settings.distortion1);
        sheet.properties.SetFloat("barrel", settings.distortion2);
        sheet.properties.SetFloat("scale", settings.scale);
        sheet.properties.SetColor("_ScanLinesColor", settings.scanLinesColor);
        if (settings.horizontal)
        {
            if (settings.distortion < 3499)
                pass = 1;
            else
                pass = 0;
        }
        else
        {
            if (settings.distortion < 3499)
                pass = 3;
            else
                pass = 2;
        }
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, pass);

    }
}