using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CreateFolders : EditorWindow
{
    private static string projectName = "PROJECT_NAME";

    [MenuItem("Assets/Create Default Folders")]
    private static void SetUpFolders()
    {
        var window = CreateInstance<CreateFolders>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 150);
        window.ShowPopup();
    }

    private static void CreateAllFolders()
    {
        var folders = new List<string>
        {
            "Animations",
            "Audio",
            "Editor",
            "Materials",
            "Meshes",
            "Prefabs",
            "Scripts",
            "Scenes",
            "Shaders",
            "Textures",
            "UI"
        };

        foreach (string folder in folders)
        {
            string path = Path.Combine("Assets", projectName, folder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        var uiFolders = new List<string>
        {
            "Assets",
            "Fonts",
            "Icon"
        };

        foreach (string subfolder in uiFolders)
        {
            string path = Path.Combine("Assets", projectName, "UI", subfolder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        AssetDatabase.Refresh();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Insert the Project name used as the root folder");
        projectName = EditorGUILayout.TextField("Project Name: ", projectName);
        Repaint();
        GUILayout.Space(70);
        if (GUILayout.Button("Generate!"))
        {
            CreateAllFolders();
            Close();
        }
    }
}