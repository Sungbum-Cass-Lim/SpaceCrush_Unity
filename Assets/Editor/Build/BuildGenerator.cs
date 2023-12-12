using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;

/// <summary>
/// Modify ReleaseType, DefineSymbol
/// </summary>
public class BuildGenerator : MonoBehaviour
{
    private static readonly string buildSettingRoot = Application.dataPath + "/Editor";
    private static readonly string buildSettingPath = buildSettingRoot + "/" + "BuildInfo.cs";

    //private static string bundleVersion = "1.0.40";

    private const string USE_WEBGL_LOCAL_CASS = "USE_WEBGL_LOCAL_CASS";
    private const string USE_WEBGL_DEV = "USE_WEBGL_DEV";
    private const string USE_WEBGL_QA = "USE_WEBGL_QA";
    private const string USE_WEBGL_STAGE = "USE_WEBGL_STAGE";
    private const string USE_WEBGL_PROD = "USE_WEBGL_PROD";

    [MenuItem("Build/build generate/WebGL/local_c")] public static void Generate_WebGL_Local_Cass() { Generate(BuildTarget.WebGL, Builder.ReleaseType.LocalCass); }
    [MenuItem("Build/build generate/WebGL/dev")] public static void Generate_WebGL_Dev() { Generate(BuildTarget.WebGL, Builder.ReleaseType.Dev); }
    [MenuItem("Build/build generate/WebGL/Qa")] public static void Generate_WebGL_Qa() { Generate(BuildTarget.WebGL, Builder.ReleaseType.Qa); }
    [MenuItem("Build/build generate/WebGL/Stage")] public static void Generate_WebGL_Stage() { Generate(BuildTarget.WebGL, Builder.ReleaseType.Stage); }
    [MenuItem("Build/build generate/WebGL/Production")] public static void Generate_WebGL_Production() { Generate(BuildTarget.WebGL, Builder.ReleaseType.Production); }

    private static void Generate(BuildTarget _buildTarget, Builder.ReleaseType _releaseType)
    {
        string ver = PlayerSettings.bundleVersion;
        
        string[] split = ver.Split('.');
        int minorVer = int.Parse(split[^1]);

        string newVer = split[0] + "." + split[1] + "." + minorVer;

        GenerateBuildSetting(_buildTarget, _releaseType, newVer);
        SetDefineSymbol(_buildTarget, _releaseType);
    }

    private static void SetDefineSymbol(BuildTarget _buildTarget, Builder.ReleaseType _releaseType)
    {
        string defineSymbol = string.Empty;

        switch (_releaseType)
        {
            case Builder.ReleaseType.LocalCass:
                defineSymbol = USE_WEBGL_LOCAL_CASS;
                break;
            case Builder.ReleaseType.Dev:
                defineSymbol = USE_WEBGL_DEV;
                break;
            case Builder.ReleaseType.Qa:
                defineSymbol = USE_WEBGL_QA;
                break;
            case Builder.ReleaseType.Stage:
                defineSymbol = USE_WEBGL_STAGE;
                break;
            case Builder.ReleaseType.Production:
                defineSymbol = USE_WEBGL_PROD;
                break;
        }

        switch (_buildTarget)
        {
            case BuildTarget.WebGL:
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, defineSymbol);
                }
                break;
        }
        
    }

    private static void GenerateBuildSetting(BuildTarget _buildTarget, Builder.ReleaseType _releaseType, string bundleVersion)
    {
        Debug.LogFormat("GenerateBuildSetting() {0}", buildSettingPath);

        if (WriteFile(GetBuildSettingCode(_buildTarget, _releaseType, bundleVersion), buildSettingRoot, buildSettingPath))
        {
            AssetDatabase.Refresh();
            Debug.LogFormat("Success GenerateBuildSetting() path='{0}'", buildSettingPath);
        }
        else
        {
            Debug.LogErrorFormat("Failed GenerateBuildSetting() path='{0}'", buildSettingPath);
        }
    }

    private static string GetBuildSettingCode(BuildTarget _buildTarget, Builder.ReleaseType _releaseType, string bundleVersion)
    {
        string code = Line(0, "using UnityEditor;", 2);
        code += Line(0, "public class BuildInfo");
        code += Line(0, "{");
        code += Line(1, "public const BuildTarget buildTarget = BuildTarget." + _buildTarget + ";");
        code += Line(1, "public const Builder.ReleaseType releaseType = Builder.ReleaseType." + _releaseType + ";");
        code += Line(1, "public const string BundleVersion = \"" + bundleVersion + "\";");
        code += Line(1, "public const string BuildDate = \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\";");
        code += Line(0, "}");
        return code;
    }


    #region implement
    private static string Line(int tabs, string code, int returnCount = 1)
    {
        string indent = "";
        for (int i = 0; i < tabs; i++)
        {
            indent += "\t";
        }
        string CRs = "";
        for (int i = 0; i < returnCount; i++)
        {
            CRs += "\n";
        }
        return indent + code + CRs;
    }

    private static bool WriteFile(string code, string root, string path)
    {
        bool success = false;

        if (false == string.IsNullOrEmpty(code))
        {
            CheckOrCreateDirectory(root);

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                try
                {
                    writer.WriteLine("{0}", code);
                    success = true;
                }
                catch (System.Exception ex)
                {
                    string msg = " \n" + ex.ToString();
                    Debug.LogError(msg);
                    EditorUtility.DisplayDialog("Error when trying to regenerate file " + path, msg, "OK");
                }
            }
        }

        return success;
    }

    private static void CheckOrCreateDirectory(string dir)
    {
        if (File.Exists(dir))
        {
            Debug.LogWarning(dir + " is a file instead of a directory !");
            return;
        }
        else if (!Directory.Exists(dir))
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
                throw ex;
            }
        }
    }
    #endregion implement
}