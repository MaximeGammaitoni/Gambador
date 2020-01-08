using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEditor;

namespace LimitlessDev.RetroLookPro
{
    [CustomEditor(typeof(RetroLookPro))]
    [CanEditMultipleObjects]
    public class RetroLookProEditor : Editor
    {
        private AnimBool debug;
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };
        private Texture texture;
        private void OnEnable()
        {
            debug = new AnimBool();
            debug.valueChanged.AddListener(Repaint);
            string[] texturePaths = AssetDatabase.FindAssets("RLProAssetIMG");
            if (texturePaths.Length != 0)
            {
                string texturePath = AssetDatabase.GUIDToAssetPath(texturePaths[0]);
                texture = (Texture)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture));
            }
            else
                texture = null;
        }
        public override void OnInspectorGUI()
        {
            RetroLookPro myTarget = (RetroLookPro)target;
            var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 1.1f, 428f);
            GUILayout.Label(texture, GUILayout.MaxHeight(120), GUILayout.MinHeight(80), GUILayout.MaxWidth(iconWidth));
            myTarget.developmentMode = EditorGUILayout.Toggle("Development Mode", myTarget.developmentMode);
            myTarget.referenceScr = (PresetScriptableObject)EditorGUILayout.ObjectField("Preset", myTarget.referenceScr, typeof(PresetScriptableObject), false);

            if (myTarget.presetss == null)
            {
                string[] efListPaths = AssetDatabase.FindAssets("RetroLookProColorPaletePresetsList");
                if (efListPaths.Length != 0)
                {

                    string efListPath = AssetDatabase.GUIDToAssetPath(efListPaths[0]);
                    myTarget.presetss = (effectPresets)AssetDatabase.LoadAssetAtPath(efListPath, typeof(effectPresets));
                }
                else
                {
                    EditorGUILayout.HelpBox("Please insert Retro Look Pro Color Palete Presets List.", MessageType.Info);
                    myTarget.presetss = (effectPresets)EditorGUILayout.ObjectField("Presets List", myTarget.presetss, typeof(effectPresets), false);
                }
            }
            EditorGUILayout.Space();

            debug.target = EditorGUILayout.Toggle("Debug", debug.target);
            using (var group = new EditorGUILayout.FadeGroupScope(debug.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;
                    serializedObject.Update();
                    EditorGUILayout.HelpBox("Use this variables as reference in your scripts or adjust all values manually from here. Dont forget to uncheck 'Development Mode'. Also very usefull for debug.", MessageType.Info);
                    DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
                    serializedObject.ApplyModifiedProperties();
                    EditorGUI.indentLevel--;

                }
            }
        }
    }
}
