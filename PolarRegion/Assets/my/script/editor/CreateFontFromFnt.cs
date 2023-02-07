using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class CreateFontFromFnt : EditorWindow
{
    [MenuItem("Tools/创建字体(Fnt)")]
    static void DoIt()
    {
        GetWindow<CreateFontFromFnt>("创建字体");
    }
    private string fontName;
    private string fontPath;
    private Texture2D tex;
    private string fntFilePath;

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("字体名称：");
        fontName = EditorGUILayout.TextField(fontName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("字体图片：");
        tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(string.IsNullOrEmpty(fontPath) ? "选择路径" : fontPath))
        {
            fontPath = EditorUtility.OpenFolderPanel("字体路径", Application.dataPath, "");
            if (string.IsNullOrEmpty(fontPath))
            {
                Debug.Log("取消选择路径");
            }
            else
            {
                fontPath = fontPath.Replace(Application.dataPath, "") + "/";
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(string.IsNullOrEmpty(fntFilePath) ? "选择fnt文件" : fntFilePath))
        {
            fntFilePath = EditorUtility.OpenFilePanelWithFilters("选择fnt文件", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new string[] { "", "fnt" });
            if (string.IsNullOrEmpty(fntFilePath))
            {
                Debug.Log("取消选择路径");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建"))
        {
            Create();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void Create()
    {
        if (string.IsNullOrEmpty(fntFilePath))
        {
            Debug.LogError("fnt为空");
            return;
        }
        if (tex == null)
        {
            Debug.LogError("字体图片为空");
            return;
        }

        string fontSettingPath = fontPath + fontName + ".fontsettings";
        string matPath = fontPath + fontName + ".mat";
        if (File.Exists(Application.dataPath + fontSettingPath))
        {
            Debug.LogErrorFormat("已存在同名字体文件:{0}", fontSettingPath);
            return;
        }
        if (File.Exists(Application.dataPath + matPath))
        {
            Debug.LogErrorFormat("已存在同名字体材质:{0}", matPath);
            return;
        }
        var list = new List<CharacterInfo>();
        XmlDocument xmlDoc = new XmlDocument();
        var content = File.ReadAllText(fntFilePath, System.Text.Encoding.UTF8);
        xmlDoc.LoadXml(content);
        var nodelist = xmlDoc.SelectNodes("font/chars/char");
        foreach (XmlElement item in nodelist)
        {
            CharacterInfo info = new CharacterInfo();
            var id = int.Parse(item.GetAttribute("id"));
            var x = float.Parse(item.GetAttribute("x"));
            var y = float.Parse(item.GetAttribute("y"));
            var width = float.Parse(item.GetAttribute("width"));
            var height = float.Parse(item.GetAttribute("height"));

            info.index = id;
            //纹理映射，上下翻转
            info.uvBottomLeft = new Vector2(x / tex.width, 1 - (y + height) / tex.height);
            info.uvBottomRight = new Vector2((x + width) / tex.width, 1 - (y + height) / tex.height);
            info.uvTopLeft = new Vector2(x / tex.width, 1 - y / tex.height);
            info.uvTopRight = new Vector2((x + width) / tex.width, 1 - y / tex.height);

            info.minX = 0;
            info.maxX = (int)width;
            info.minY = -(int)height / 2;
            info.maxY = (int)height / 2;
            info.advance = (int)width;

            list.Add(info);
        }

        Material mat = new Material(Shader.Find("GUI/Text Shader"));
        mat.SetTexture("_MainTex", tex);
        Font m_myFont = new Font();
        m_myFont.material = mat;
        AssetDatabase.CreateAsset(mat, "Assets" + matPath);
        AssetDatabase.CreateAsset(m_myFont, "Assets" + fontSettingPath);
        m_myFont.characterInfo = list.ToArray();
        EditorUtility.SetDirty(m_myFont);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("创建成功！");
    }
}