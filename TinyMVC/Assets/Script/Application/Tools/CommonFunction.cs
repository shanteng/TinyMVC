
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class CommonFunction
{
    public static string GetStreamAssetPath(string file)
    {
        //#if UNITY_EDITOR ||UNITY_IOS
#if UNITY_IOS
        return $"file://{Application.streamingAssetsPath}/{file}";
#elif UNITY_ANDROID
        return  $"{Application.streamingAssetsPath}/{file}";
#else
        return $"{Application.streamingAssetsPath}/{file}";
#endif
    }

    public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#endif
        return default(T);
    }
}







