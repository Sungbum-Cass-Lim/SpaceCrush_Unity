using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public static class Builder
{
    public enum ReleaseType
    {
        LocalCass,
        Dev,
        Qa,
        Stage,
        Production
    }

    public static readonly string appName = "SpaceCrush";

    public static string ReleaseFilePath
    {
        get { return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) +"/"+ "Release"; }
    }

    public static string[] Scenes
    {
        get { return GetEnabledEditorScenes(); }
    }

    private static string[] GetEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    [MenuItem("Build/WebGL/Build_WebGL_Local_Cass")]
    public static void Build_WebGL_Local_Cass()
    {
        BuildGenerator.Generate_WebGL_Local_Cass();
        AssetDatabase.Refresh();
        BuildGenerator.Generate_WebGL_Local_Cass();
        AssetDatabase.Refresh();
        //EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.development = false;
        BuildWebGL_Local();
    }
    
    [MenuItem("Build/WebGL/Build_WebGL_Dev")]
    public static void Build_WebGL_Dev()
    {
        BuildGenerator.Generate_WebGL_Dev();
        AssetDatabase.Refresh();
        BuildGenerator.Generate_WebGL_Dev();
        AssetDatabase.Refresh();
        //EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.development = false;
        IncreaseRevision();
        BuildWebGL();
    }

    [MenuItem("Build/WebGL/Build_WebGL_Qa")]
    public static void Build_WebGL_Qa()
    {
        BuildGenerator.Generate_WebGL_Qa();
        AssetDatabase.Refresh();
        BuildGenerator.Generate_WebGL_Qa();
        AssetDatabase.Refresh();
        //EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.development = false;
        BuildWebGL();
    }
    [MenuItem("Build/WebGL/Build_WebGL_Stage")]
    public static void Build_WebGL_Stage()
    {
        BuildGenerator.Generate_WebGL_Stage();
        AssetDatabase.Refresh();
        BuildGenerator.Generate_WebGL_Stage();
        AssetDatabase.Refresh();
        EditorUserBuildSettings.development = false;
        BuildWebGL();
    }
    [MenuItem("Build/WebGL/Build_WebGL_Prod")]
    public static void Build_WebGL_Prod()
    {
        BuildGenerator.Generate_WebGL_Production();
        AssetDatabase.Refresh();
        BuildGenerator.Generate_WebGL_Production();
        AssetDatabase.Refresh();
        EditorUserBuildSettings.development = false;
        BuildWebGL();
    }
    
    
    private static void CheckCurrentVersion()
    {
        Debug.Log("Build v" + PlayerSettings.bundleVersion +
                  " (" + PlayerSettings.Android.bundleVersionCode + ")"); //현재 버전 표시
    }
    
    static void EditVersion(int majorIncr, int minorIncr, int buildIncr)
    {
        string[] lines = PlayerSettings.bundleVersion.Split('.');

        int MajorVersion = int.Parse(lines[0]) + majorIncr;
        int MinorVersion = int.Parse(lines[1]) + minorIncr;
        int Build = buildIncr+1;

        PlayerSettings.bundleVersion = MajorVersion.ToString("0") + "." +
                                       MinorVersion.ToString("0") + "." +
                                       Build.ToString("0");
        PlayerSettings.Android.bundleVersionCode =
            MajorVersion * 10000 + MinorVersion * 1000 + Build;
        CheckCurrentVersion();
    }


    private static void IncreaseRevision()
    {
        string[] lines = PlayerSettings.bundleVersion.Split('.');
        EditVersion(0, 0, int.Parse(lines[2]));
    }

/*
    private static void IncreaseMinor()
    {
        string[] lines = PlayerSettings.bundleVersion.Split('.');
        EditVersion(0, int.Parse(lines[1]), int.Parse(lines[2]));
    }
    
    private static void IncreaseMajor()
    {
        string[] lines = PlayerSettings.bundleVersion.Split('.');
        EditVersion(int.Parse(lines[0]), int.Parse(lines[1]), int.Parse(lines[2]));
    }
    */
    

    private static void BuildWebGL_Local()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.WebGL, ScriptingImplementation.IL2CPP);
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        //EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSpeed;
        PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSpeed);
        BuildStart(BuildInfo.buildTarget, BuildOptions.None);
    }
    
    private static void BuildWebGL()
    {
        //IncreaseRevision();
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.WebGL, ScriptingImplementation.IL2CPP);
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        //EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
        PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSpeed);
        BuildStart(BuildInfo.buildTarget, BuildOptions.None);
    }

    private static void BuildStart(BuildTarget build_target, BuildOptions build_options)
    {
        //AddressableSetting();

        Directory.CreateDirectory(ReleaseFilePath);
        string _appName = appName + "_" + BuildInfo.releaseType + "_" + "v" + PlayerSettings.bundleVersion + "_" + build_target.ToString();
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = Scenes;
        buildPlayerOptions.locationPathName = ReleaseFilePath + "/" + _appName;
        buildPlayerOptions.target = build_target;
        buildPlayerOptions.options = build_options;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    //어드레서블 빌드할때 빌드모드 설정해야 함
    //private static void AddressableSetting()
    //{
    //    var settings = new AddressableAssetSettings();
    //    settings.ActivePlayModeDataBuilderIndex = 2;
    //}
}
