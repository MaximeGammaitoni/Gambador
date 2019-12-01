using UnityEngine;
using UnityEditor;
namespace LimitlessDev.RetroLookPro
{
[CustomEditor(typeof(Readme))]
[InitializeOnLoad]
public class ReadmeEditor : Editor {

	static float kSpace = 16f;

	[MenuItem("Retro Look Pro/Info", priority = 333)]
	static Readme SelectReadme() 
	{
		var ids = AssetDatabase.FindAssets("Readme t:Readme");
		if (ids.Length == 1)
		{
			var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));
			
			Selection.objects = new UnityEngine.Object[]{readmeObject};
			
			return (Readme)readmeObject;
		}
		else
		{
			//Debug.Log("Couldn't find a ReadMe");
			return null;
		}
	}
	
	protected override void OnHeaderGUI()
	{
        var readme = (Readme)target;
		Init();
		if(readme.icon)
        GUILayout.Label(readme.icon, GUILayout.MaxHeight(120), GUILayout.MinHeight(80),GUILayout.MinWidth(480));
        GUILayout.Label(readme.title, TitleStyle);
	}
	
	public override void OnInspectorGUI()
	{
		var readme = (Readme)target;
		Init();
        
        foreach (var section in readme.sections)
		{
			if (!string.IsNullOrEmpty(section.heading))
			{
                GUILayout.BeginHorizontal();        
				if(section.Headericon)        
                GUILayout.Label(section.Headericon, GUILayout.MaxHeight(25), GUILayout.MaxWidth(25));
                GUILayout.Label(section.heading, HeadingStyle);
                GUILayout.EndHorizontal();
			}
			if (!string.IsNullOrEmpty(section.text))
			{
				GUILayout.Label(section.text, BodyStyle);
			}
			if (!string.IsNullOrEmpty(section.linkText))
			{
                if (section.isLink)
                {
                    if (LinkLabel(new GUIContent(section.linkText)))
                    {
                        Application.OpenURL(section.url);
                    }
                }
			}
            if (section.extraImages)
            {
                section.foldout = EditorGUILayout.Toggle("Example", section.foldout);
                if (section.foldout){
if(section.screen)
                    GUILayout.Label(section.screen);
				}
            }
            if (!string.IsNullOrEmpty(section.linkText))
            {
                if (section.isButton)
                {
                    if (GUILayout.Button(new GUIContent(section.linkText)))
                    {
                        Application.OpenURL(section.url);
                    }
                }
            }
            if (section.isTextField)
            {
                ReadOnlyTextField(section.linkText, section.copyText);
            }

			GUILayout.Space(kSpace);
		}
	}	
	
	bool m_Initialized;
	
	GUIStyle LinkStyle { get { return m_LinkStyle; } }
	[SerializeField] GUIStyle m_LinkStyle;
	
	GUIStyle TitleStyle { get { return m_TitleStyle; } }
	[SerializeField] GUIStyle m_TitleStyle;
	
	GUIStyle HeadingStyle { get { return m_HeadingStyle; } }
	[SerializeField] GUIStyle m_HeadingStyle;
	
	GUIStyle BodyStyle { get { return m_BodyStyle; } }
	[SerializeField] GUIStyle m_BodyStyle;
	
	void Init()
	{
		if (m_Initialized)
			return;
		m_BodyStyle = new GUIStyle(EditorStyles.label);
		m_BodyStyle.wordWrap = true;
		m_BodyStyle.fontSize = 14;
		
		m_TitleStyle = new GUIStyle(m_BodyStyle);
		m_TitleStyle.fontSize = 26;
		
		m_HeadingStyle = new GUIStyle(m_BodyStyle);
		m_HeadingStyle.fontSize = 18 ;
		
		m_LinkStyle = new GUIStyle(m_BodyStyle);
		m_LinkStyle.wordWrap = false;
		m_LinkStyle.normal.textColor = new Color (0x00/255f, 0x78/255f, 0xDA/255f, 1f);
		m_LinkStyle.stretchWidth = false;
		
		m_Initialized = true;
	}
	
	bool LinkLabel (GUIContent label, params GUILayoutOption[] options)
	{
		var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

		Handles.BeginGUI ();
		Handles.color = LinkStyle.normal.textColor;
		Handles.DrawLine (new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
		Handles.color = Color.white;
		Handles.EndGUI ();

		EditorGUIUtility.AddCursorRect (position, MouseCursor.Link);

		return GUI.Button (position, label, LinkStyle);
	}
    void ReadOnlyTextField(string label, string text)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }
        EditorGUILayout.EndHorizontal();
    }
}
}

