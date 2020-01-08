using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum BleedMode
{
    NTSCOld3Phase,
    NTSC3Phase,
    NTSC2Phase,
    customBleeding
}
[Serializable]
public sealed class bleedModeParameter : ParameterOverride<BleedMode> { };
[Serializable]
[PostProcess(typeof(RLPRO_SRP_BleedRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Bleed Effect", false)]
public sealed class RLProBleed : PostProcessEffectSettings
{
    public bleedModeParameter bleedMode = new bleedModeParameter { };
    public int bleedModeIndex;
    [Range(0f, 15f), Tooltip("Bleed Stretch amount.")]
    public FloatParameter bleedAmount = new FloatParameter { value = 0 };

    [Range(0, 50), Tooltip("Bleed Length.")]
    public IntParameter bleedLength = new IntParameter { value = 0 };
    public BoolParameter bleedDebug = new BoolParameter { value = false };
    public BoolParameter editCurves = new BoolParameter { value = false };
    public BoolParameter syncYQ = new BoolParameter { value = false };
    public SplineParameter curveY = new SplineParameter { value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f)) };
    public SplineParameter curveI = new SplineParameter { value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f)) };
    public SplineParameter curveQ = new SplineParameter { value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f)) };
}

public sealed class RLPRO_SRP_BleedRenderer : PostProcessEffectRenderer<RLProBleed>
{
    public AnimationCurve cu = new AnimationCurve();
    int max_curve_length = 50;
    Texture2D texCurves = null;
    Vector4 curvesOffest = new Vector4(0, 0, 0, 0);
    float[,] curvesData = new float[50, 3];

    public override void Init()
    {
        if (settings.bleedMode.value == BleedMode.customBleeding)
        {
            Curves();
        }
    }
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/BleedEffect"));
        if ((int)settings.bleedMode.value == 3) { if (settings.editCurves) Curves(); }
        if ((int)settings.bleedMode.value == 3)
        {
            if (texCurves == null)
                Curves();
            sheet.properties.SetTexture("_CurvesTex", texCurves);
        }

        sheet.properties.SetVector("curvesOffest", curvesOffest);
        sheet.properties.SetInt("bleedLength", settings.bleedLength);
        ParamSwitch(sheet, settings.bleedDebug, "VHS_DEBUG_BLEEDING_ON");

        sheet.properties.SetFloat("bleedAmount", settings.bleedAmount);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.bleedMode.value);

    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }
    private void Curves()
    {
        if (texCurves == null) texCurves = new Texture2D(max_curve_length, 1, TextureFormat.RGBA32, false);
        curvesOffest[0] = 0.0f;
        curvesOffest[1] = 0.0f;
        curvesOffest[2] = 0.0f;
        float t = 0.0f;
        for (int i = 0; i < settings.bleedLength; i++)
        {
            t = ((float)i) / ((float)settings.bleedLength);
            t = (int)(t * 100);
            curvesData[i, 0] = settings.curveY.value.cachedData[i];
            curvesData[i, 1] = settings.curveI.value.cachedData[i];
            curvesData[i, 2] = settings.curveQ.value.cachedData[i];
            if (settings.syncYQ) curvesData[i, 2] = curvesData[i, 1];

            if (curvesOffest[0] > curvesData[i, 0]) curvesOffest[0] = curvesData[i, 0];
            if (curvesOffest[1] > curvesData[i, 1]) curvesOffest[1] = curvesData[i, 1];
            if (curvesOffest[2] > curvesData[i, 2]) curvesOffest[2] = curvesData[i, 2];
        };
        curvesOffest[0] = Mathf.Abs(curvesOffest[0]);
        curvesOffest[1] = Mathf.Abs(curvesOffest[1]);
        curvesOffest[2] = Mathf.Abs(curvesOffest[2]);

        for (int i = 0; i < settings.bleedLength; i++)
        {
            curvesData[i, 0] += curvesOffest[0];
            curvesData[i, 1] += curvesOffest[1];
            curvesData[i, 2] += curvesOffest[2];
            texCurves.SetPixel(-2 + settings.bleedLength - i, 0, new Color(curvesData[i, 0], curvesData[i, 1], curvesData[i, 2]));
        };

        texCurves.Apply();

    }
}