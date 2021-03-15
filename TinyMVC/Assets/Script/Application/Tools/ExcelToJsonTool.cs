
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ExcelConfig
{
    public string name;
    public List<string> sheets;
}

[Serializable]
public class ExcelToJsonTool : MonoBehaviour
{
    public List<ExcelConfig> _Excels;


    public static int ParseInt(string data)
    {
        if (data.Length == 0)
            return 0;
        int val = 0;
        if (int.TryParse(data, out val) == false)
        {
            return 0;
        }
        return val;
    }

    public static float ParseFloat(string data)
    {
        if (data.Length == 0)
            return 0;

        float val = 0;
        if (float.TryParse(data, out val) == false)
        {
            return 0;
        }
        return val;
    }
}





