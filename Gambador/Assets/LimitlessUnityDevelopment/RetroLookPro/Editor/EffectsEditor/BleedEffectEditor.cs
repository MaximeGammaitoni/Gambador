using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(RLProBleed))]
    internal sealed class BleedEffectEditor : PostProcessEffectEditor<RLProBleed>
    {

        SerializedParameterOverride m_BleedModeEnum;
        SerializedParameterOverride m_BleedAmount;
        SerializedParameterOverride m_BleedLength;
        SerializedParameterOverride m_BleedDebug;
        SerializedProperty m_CurveY;
        SerializedProperty m_CurveI;
        SerializedProperty m_CurveQ;
        SerializedProperty jdPR;
        SerializedParameterOverride m_EditCurves;
        SerializedParameterOverride m_SyncYQ;
        SerializedParameterOverride m_SplineCurveY;
        SerializedParameterOverride m_SplineCurveI;
        SerializedParameterOverride m_SplineCurveQ;

        bool laal;

        public override void OnEnable()
        {
            m_BleedModeEnum = FindParameterOverride(x => x.bleedMode);
            m_BleedAmount = FindParameterOverride(x => x.bleedAmount);
            m_BleedLength = FindParameterOverride(x => x.bleedLength);
            m_BleedDebug = FindParameterOverride(x => x.bleedDebug);
m_SplineCurveY = FindParameterOverride(x => x.curveY);
m_SplineCurveI = FindParameterOverride(x => x.curveI);
m_SplineCurveQ = FindParameterOverride(x => x.curveQ);
            m_EditCurves = FindParameterOverride(x => x.editCurves);

            m_SyncYQ = FindParameterOverride(x => x.syncYQ);
        }

        public override void OnInspectorGUI()
        {

            PropertyField(m_BleedModeEnum);
            PropertyField(m_BleedAmount);

            if (m_BleedModeEnum.value.intValue == (int)BleedMode.customBleeding)
            {
                PropertyField(m_SyncYQ);
                PropertyField(m_EditCurves);

                PropertyField(m_SplineCurveY);
                if (!m_SyncYQ.value.boolValue)
                PropertyField(m_SplineCurveI);
                PropertyField(m_SplineCurveQ);

                PropertyField(m_BleedLength);
            }

            PropertyField(m_BleedDebug);

        }
    }
}
