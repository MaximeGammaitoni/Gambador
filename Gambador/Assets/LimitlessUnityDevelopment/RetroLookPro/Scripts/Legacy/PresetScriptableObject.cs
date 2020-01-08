using UnityEngine;
namespace LimitlessDev.RetroLookPro
{
[CreateAssetMenu(fileName = "RetroLookProPreset", menuName = "RetroLookPro/New Preset", order = 1)]
public class PresetScriptableObject : ScriptableObject
{
    public Preset currPreset;
}
[System.Serializable]
public class Preset
{
    public Preset ShallowCopy()
    {
        Preset pre = (Preset)this.MemberwiseClone();
        return pre;
    }

    #region Bottom noise
    [Header("Bottom Noise")]
       public Texture2D _BottomNoiseTexture;
    public bool useBottomNoise = true;
    [Range(0.0f, 0.5f)]
    public float bottomHeight = 0.04f;
    [Range(0.0f, 3.0f)]
    public float bottomIntensity = 1.0f;
    public bool useBottomStretch = true;
    [HideInInspector]
    public Material _bottomNoiseMat;
    [Space]
    #endregion

    #region Color Palette
    public ResolutionMode resolutionMode;
    public int resolutionModeIndex;
    public Vector2Int resolution;
    public int pixelSize;
    public float dither;
    public float opacity;
    public int colorPresetIndex;
    public bool f_show_colorPalette;
    public bool enableColorPalette;
    #endregion

    #region TV Mode
    [Header("TV Mode")]
    public bool _enableTVmode;
    [HideInInspector]
    public Material _VHS_Material;
    public Texture _VHSNoise;
    public float _textureIntensity = 0.713f;
    public float _VerticalOffsetFrequency = 0f;
    public float _verticalOffset = 0.076f;
    public float _offsetColor = 0.0108f;
    public float _OffsetDistortion = 1210f;
    public bool _scan;
    public float _adjustLines = 2f;
    public Color _scanLinesColor;
    [HideInInspector]
    public Material _TV_Material;
    public float _hardScan;
    public float _resolution;
    public float maskDark = 0.5f;
    public float maskLight = 1.5f;
    public Vector4 warp;
    [Space]
    #endregion

    #region Main properties
    /// Materials and Shaders assigned automatically 
    [HideInInspector]
    public Shader sh_first;               //first pass  
    [HideInInspector]
    public Shader sh_second;              //second pass  
    [HideInInspector]
    public Shader sh_third;               //third pass  
    [HideInInspector]
    public Shader sh_fourth;               //fourth pass  
    [HideInInspector]
    public Shader sh_clear;             //clear shader
    [HideInInspector]
    public Shader sh_tape;              //shader_tape_noise
    [HideInInspector]
    public Material m_1;                // material for first shader  
    [HideInInspector]
    public Material m_2;                // material for second shader 
    [HideInInspector]
    public Material m_3;                // material for third shader
    [HideInInspector]
    public Material m_4;                // material for fourth shader 
    [HideInInspector]
    public Material m_clear;           // material for clear shader
    [HideInInspector]
    public Material m_tape_noise;       // material for tape noise shader
    #endregion

    #region ByPass mode
    [Header("ByPass mode")]
    public bool enableCustomTexture;
    public Texture bypassTex;
    public Sprite spriteTex;
    [Space]
    #endregion

    #region Foldout group bools
    [HideInInspector]
    public bool f_show_Bleed = false;
    [HideInInspector]
    public bool f_show_Fisheye = false;
    [HideInInspector]
    public bool f_show_Vignette = false;
    [HideInInspector]
    public bool f_show_Noise = false;
    [HideInInspector]
    public bool f_show_Jitter = false;
    [HideInInspector]
    public bool f_show_Signal = false;
    [HideInInspector]
    public bool f_show_Artefacts = false;
    [HideInInspector]
    public bool f_show_Extra = false;
    [HideInInspector]
    public bool f_show_Bypass = false;
    [HideInInspector]
    public bool f_show_TV_mode = false;
    [HideInInspector]
    public bool f_show_Old_TV_mode;
    [HideInInspector]
    public bool f_show_Bottom_Noise_mode = false;
    [HideInInspector]
    public bool f_show_Glitch_Effects = false;
    #endregion

    #region Old Film effect
    [Header("Old Film effect")]
    public bool o_Enable_negative_props = false;
    public bool o_Enable_Old_TV_effects = false;
    public float o_Luminosity = 0.2f ;
    public float o_Negative = 0f;
    public float o_Vignette = 1f;
    public float o_FPS = 12f;
    public float o_contrast = 1f;
    public float o_burn = 0f;
    public float o_sceneCut = 0.5f;
    public float o_Fade = 1f;
    public Material o_negative_mat;
    public Material o_oldfilm_mat;
    #endregion
    
    #region Bleed effect
    [Header("Bleed effect")]
    public int b_Mode = 0;
    public bool b_Bleed = true;
    public int b_LinesMode = 0;
    public float b_ScreenLinesNum = 240f;
    public float b_BleedAmount = 1f; //default 1.-2.
    public bool b_BleedDebug = false;
    public int b_BleedLength = 21;
    public bool b_BleedCurveEditMode = false;
    public bool b_BleedCurveSync = false;
    public AnimationCurve b_BleedCurve1 = AnimationCurve.Linear(0, 1, 1, 0);
    public AnimationCurve b_BleedCurve2 = AnimationCurve.Linear(0, 0.5f, 1, 0);
    public AnimationCurve b_BleedCurve3 = AnimationCurve.Linear(0, 0.5f, 1, 0);
    [Space]
    #endregion

    #region Glitch effect
    [Header("Glitch Effect")]
    public bool g_enable_glitch = false;
    public float g_strength = 1;
    public float g_fade = 1;
    public float g_speed = 1;
    public Material g_GlitchMat1;

            [Header("Glitch Effect2")]
        public bool g_enable_glitch2 = false;
        public float g_2Intensity = 1;
        public float g_2Res ;
        public float g_2speed;
        public Material g_GlitchMat2;
    #endregion

    #region Fisheye effect
    [Header("Fisheye Effect")]
    public bool f_Fisheye = true;
    public float f_FisheyeBend = 1.0f;
    public int f_FisheyeType = 0;
    public float f_FisheyeSize = 1.5f;
    public float f_CutoffX = 1.0f;
    public float f_CutoffY = 1.0f;
    public float f_FadeX = 20.0f;
    public float f_FadeY = 20.0f;
    [Space]
    #endregion

    #region Vignette effect
    [Header("Vignette Effect")]
    public bool v_Vignette = false;
    public float v_VignetteAmount = 1.0f;
    public float v_VignetteSpeed = 1.0f;
    [Space]
    #endregion

    #region Noise
    [Header("Noise")]
    public bool n_enableNoise;
    public int n_NoiseMode = 1;
    public float n_NoiseLinesAmountY = 220f;
    public float n_NoiseSignalProcessing = 0.1f;
    public bool f_Granularity = false;
    public float f_GranularityAmount = 0.09f;
    /// Noise modes
    // Signal
    [Header("Signal Noise")]
    public bool f_SignalNoise = true;
    public float f_SignalNoiseAmount = 0.2f;
    public float f_SignalNoisePower = 0.68f;
    // Tape 
    [Header("Tape Noise")]
    public bool f_TapeNoise = true;
    public float f_TapeNoiseTH = 0.68f;
    public float f_TapeNoiseAmount = 1.0f;
    public float f_TapeNoiseSpeed = 1.0f;
    // Line
    [Header("Line Noise")]
    public bool f_LineNoise = true;
    public float f_LineNoiseAmount = 1.0f;
    public float f_LineNoiseSpeed = 3.0f;
    [Space]
    ///
    #endregion

    #region Video Shake filter
    // Jitter
    [Header("Video Shake filter")]
    public bool j_enableJitter;
    public bool j_ScanLines = false;
    public float j_ScanLinesWidth = 15.0f;
    public bool j_LinesFloat = false;
    public float j_LinesSpeed = 1.0f;
    public bool j_Stretch = true;
    public bool j_JitterHorizontal = true;
    public float j_JitterHorizAmount = 0.2f;
    public bool j_JitterVertical = false;
    public float j_VertAmount = 1.0f;
    public float j_VertSpeed = 1.0f;
    // Twitch
    public bool j_TwitchHorizontal = false;
    public float j_TwitchHorizFreq = 1.0f;
    public bool j_TwitchVertical = false;
    public float j_TwitchVertFreq = 1.0f;
    [Space]
    #endregion

    #region Picture correction
    [Header("Picture correction")]
    public bool p_PictureCorrection = false;
    public float p_PictureCorr1 = 0f;
    public float p_PictureCorr2 = 0f;
    public float p_PictureCorr3 = 0f;
    public float p_PictureShift1 = 1f;
    public float p_PictureShift2 = 1f;
    public float p_PictureShift3 = 1f;
    public float p_Gamma = 1f;
    #endregion

    #region Artefacts
    [Header("Artefacts")]
    public bool a_Artefacts = false;
    public int a_ArtefactsMode = 0;
    public float a_ArtefactsThreshold = 0.1f;
    public float a_ArtefactsAmount = 2.0f;
    public float a_ArtefactsFadeAmount = 0.82f;
    public Color a_ArtefactsColor = new Color(1.0f, 0.8f, 0.2f);
    public bool a_ArtefactsDebug = false;
    [Space]
    #endregion

    #region Time Setting
    public bool independentTimeOn = false;
    #endregion
}
}
