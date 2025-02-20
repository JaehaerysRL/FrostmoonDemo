using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ForceLangVersion
{
    static ForceLangVersion()
    {
        EditorApplication.update += OnFirstUpdate;
    }

    private static void OnFirstUpdate()
    {
        EditorApplication.update -= OnFirstUpdate;
        PatchCsprojFiles();
    }

    [MenuItem("Tools/强制修复LangVersion")]
    public static void PatchCsprojFiles()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        // 需要修改的.csproj文件列表
        var csprojFiles = new[] { "Assembly-CSharp.csproj", "Assembly-CSharp-Editor.csproj" };

        foreach (var file in csprojFiles)
        {
            string csprojPath = Path.Combine(projectRoot, file);
            if (File.Exists(csprojPath))
            {
                string content = File.ReadAllText(file);
                if (Regex.IsMatch(content, @"<LangVersion>[^<]+</LangVersion>"))
                {
                    // 直接替换现有LangVersion
                    content = Regex.Replace(content,
                        @"<LangVersion>[^<]+</LangVersion>",
                        "<LangVersion>13.0</LangVersion>");
                }
                else
                {
                    // 插入到第一个PropertyGroup中
                    content = Regex.Replace(content,
                        @"(<PropertyGroup>)",
                        "$1\n    <LangVersion>13.0</LangVersion>");
                }

                File.WriteAllText(file, content);
            }

        }

        // AssetDatabase.Refresh();
    }
}

public class CsProjectLangVersionPostprocessor : AssetPostprocessor
{
    // 当生成C#项目文件时调用此方法
    private static void OnGeneratedCSProjectFiles()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        // 需要修改的.csproj文件列表
        var csprojFiles = new[] { "Assembly-CSharp.csproj", "Assembly-CSharp-Editor.csproj" };

        foreach (var file in csprojFiles)
        {
            string csprojPath = Path.Combine(projectRoot, file);
            if (File.Exists(csprojPath))
            {
                string content = File.ReadAllText(file);
                if (Regex.IsMatch(content, @"<LangVersion>[^<]+</LangVersion>"))
                {
                    // 直接替换现有LangVersion
                    content = Regex.Replace(content,
                        @"<LangVersion>[^<]+</LangVersion>",
                        "<LangVersion>13.0</LangVersion>");
                }
                else
                {
                    // 插入到第一个PropertyGroup中
                    content = Regex.Replace(content,
                        @"(<PropertyGroup>)",
                        "$1\n    <LangVersion>13.0</LangVersion>");
                }

                File.WriteAllText(file, content);
            }

        }
    }

}