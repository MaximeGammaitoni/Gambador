using System;
using UnityEngine;
using UnityEditor;
namespace LimitlessDev.RetroLookPro
{
[CreateAssetMenu(fileName = "Readme", menuName = "Info/ReadMe", order = 1)]
public class Readme : ScriptableObject {
	public Texture2D icon;
	public string title;
	public Section[] sections;
	public bool loadedLayout;

	
	[Serializable]
	public class Section {
		public string heading, text, linkText, url;
        public bool extraImages;
        public bool foldout;
        public bool isButton;
        public bool isLink;
        public Texture2D Headericon;
        public bool isTextField;
        public string copyText;
        public Texture2D screen;
    }
    public void InsertText()
    {
        sections = new Section[1];
        sections[0] = new Section
        {
            heading = "asd",
            linkText = "asd link",
            text = "asd text",
            url = "www.google.com"
        };
        title = "ReadMe please";
        //var ids = AssetDatabase.FindAssets("");
        string[] results1 = AssetDatabase.FindAssets("editorRLImage");
        string textutrePath = AssetDatabase.GUIDToAssetPath(results1[0]);
        icon = (Texture2D)AssetDatabase.LoadAssetAtPath(textutrePath, typeof(Texture2D));
    }
}
}
