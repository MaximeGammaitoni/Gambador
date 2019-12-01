using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(Glitch2Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Glitch2", false)]
public sealed class RLProGlitch2 : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("")]
    public FloatParameter intencity = new FloatParameter { value = 1f };
    [Range(1f, 2f), Tooltip(".")]
    public FloatParameter resolutionMultiplier = new FloatParameter { value = 1f };

    [Range(0f, 1f), Tooltip(".")]
    public FloatParameter stretchMultiplier = new FloatParameter { value = 0.88f };
}

public sealed class Glitch2Renderer : PostProcessEffectRenderer<RLProGlitch2>
{
    private float T;
    RenderTexture _trashFrame1;
    RenderTexture _trashFrame2;
    Texture2D _noiseTexture;
    RenderTexture trashFrame;

    public override void Render(PostProcessRenderContext context)
    {
        if (_trashFrame1 != null || _trashFrame2 != null)
        {
            SetUpResources(settings.resolutionMultiplier);

        }
        if (UnityEngine.Random.value > Mathf.Lerp(0.9f, 0.5f, settings.intencity))
        {
            SetUpResources(settings.resolutionMultiplier);
            UpdateNoiseTexture(settings.resolutionMultiplier);
        }

        // Update trash frames.
        int fcount = Time.frameCount;

        if (fcount % 13 == 0) context.command.BlitFullscreenTriangle(context.source, _trashFrame1);
        if (fcount % 73 == 0) context.command.BlitFullscreenTriangle(context.source, _trashFrame2);

        trashFrame = UnityEngine.Random.value > 0.5f ? _trashFrame1 : _trashFrame2;

        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Glitch2RetroLook"));
        sheet.properties.SetFloat("_Intensity", settings.intencity);

        if (_noiseTexture == null)
        {
            UpdateNoiseTexture(settings.resolutionMultiplier);
        }

        sheet.properties.SetTexture("_NoiseTex", _noiseTexture);
        if(trashFrame != null)
        sheet.properties.SetTexture("_TrashTex", trashFrame);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    void SetUpResources(float g_2Res)
    {
        if (_trashFrame1 != null || _trashFrame2 != null)
        {
            return;
        }
        Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 32));
        _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false)
        {

            hideFlags = HideFlags.DontSave,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        _trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0)
        {
            hideFlags = HideFlags.DontSave
        };
        _trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0)
        {
            hideFlags = HideFlags.DontSave
        };

        UpdateNoiseTexture(g_2Res);
    }
    void UpdateNoiseTexture(float g_2Res)
    {
        Color color = RandomColor();
        if (_noiseTexture == null)
        {
            Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 32));
            _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false);
        }
        for (var y = 0; y < _noiseTexture.height; y++)
        {
            for (var x = 0; x < _noiseTexture.width; x++)
            {
                if (UnityEngine.Random.value > settings.stretchMultiplier) color = RandomColor();
                _noiseTexture.SetPixel(x, y, color);
            }
        }

        _noiseTexture.Apply();
    }
    static Color RandomColor()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
}