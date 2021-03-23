
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
public class BuildAssetBundleEditor : Editor 
{
    public static string AssetBundlePath => $"{GetPathPrefix()}/BuildResources";
    public static string DevBundlePath => $"Assets/DevAssetBundles";
    public static string GetPathPrefix()
    {
        return EditorUserBuildSettings.activeBuildTarget.ToString();
    }

    [MenuItem("AssetBundle/BuildAssetBundles")]
    private static void BuildAssetBundles()
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        CollectAssetBundles("Assets/DevResources/atlas", ".atlas", assetBundleBuilds);
        CollectAssetBundles("Assets/DevResources/ui", ".pbab", assetBundleBuilds);
        try
        {
            EditorUtility.DisplayProgressBar("Build", "BuildingAssetBundles", 0.0f);
            if (!Directory.Exists(AssetBundlePath))
            {
                Directory.CreateDirectory(AssetBundlePath);
            }

            var manifest = BuildPipeline.BuildAssetBundles(AssetBundlePath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            if (manifest)
            {
                Debug.Log("Successfully Build!");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //将打包好的Ab包复制到DevAssetBundles用于Editor上加载测试
        CopyDirIntoDestDirectory(AssetBundlePath, DevBundlePath, true);

        EditorUtility.DisplayDialog("", "Success AssetBundleIsCopyTo:"+ DevBundlePath, "ok");
       
    }

    private static void CollectAssetBundles(string path, string abextension, List<AssetBundleBuild> assetBundleBuilds)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            string exten;
            foreach (var file in files)
            {
                exten = file.Extension;
                if (CheckIgnoreFile(exten))
                {
                    continue;
                }
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = file.Name.Replace(exten, abextension);
                build.assetNames = new[] { GetReleativeToAssets(file.FullName) };
                assetBundleBuilds.Add(build);
            }
        }
    }

    private static bool CheckIgnoreFile(string extension)
    {
        if (extension.Equals(".meta") || extension.Equals(".DS_Store") || extension.Equals(".cs"))
        {
            return true;
        }
        return false;
    }

    private static string GetReleativeToAssets(string fullName)
    {
        //获得在文件在Assets下的目录，类似于Assets/path/of/yourfile
        string fileRelativePath = fullName.Substring(Application.dataPath.Length - 6);
        //如果在windows平台下运行，需要替换路径中的\为/;
        fileRelativePath = fileRelativePath.Replace("\\", "/");
        return fileRelativePath;
    }

    private static void CopyDirIntoDestDirectory(string sourceFileName, string destFileName, bool overwrite)
    {
        if (!Directory.Exists(destFileName))
        {
            Directory.CreateDirectory(destFileName);
        }

        foreach (var file in Directory.GetFiles(sourceFileName))
        {
            File.Copy(file, Path.Combine(destFileName, Path.GetFileName(file)), overwrite);
        }

        foreach (var d in Directory.GetDirectories(sourceFileName))
        {
            CopyDirIntoDestDirectory(d, Path.Combine(destFileName, Path.GetFileName(d)), overwrite);
        }
    }

}//end class