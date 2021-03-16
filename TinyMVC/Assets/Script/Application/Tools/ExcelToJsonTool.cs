
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
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


   



public class UtilTools
{
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
}

public class StringUtils
{
    private static object m_builderLock = new object();
    private static StringBuilder m_builder = new StringBuilder();
    public static string combine(params object[] texts)
    {
        lock (m_builderLock)
        {
            m_builder.Remove(0, m_builder.Length);
            var len = texts.Length;
            for (int i = 0; i < len; i++)
            {
                m_builder.Append(texts[i]);
            }
            return m_builder.ToString(0, m_builder.Length);
        }
    }

    public static string combine(string main, params object[] texts)
    {
        lock (m_builderLock)
        {
            m_builder.Remove(0, m_builder.Length);
            m_builder.Append(main);
            var len = texts.Length;
            for (int i = 0; i < len; i++)
            {
                m_builder.Append(texts[i]);
            }
            return m_builder.ToString(0, m_builder.Length);
        }
    }

    public static string combine(string main, int intParam)
    {
        lock (m_builderLock)
        {
            m_builder.Remove(0, m_builder.Length);
            m_builder.Append(main);
            m_builder.Append(intParam.ToString());
            return m_builder.ToString(0, m_builder.Length);
        }
    }

    public static string combine(string main, float floatParam)
    {
        lock (m_builderLock)
        {
            m_builder.Remove(0, m_builder.Length);
            m_builder.Append(main);
            m_builder.Append(floatParam.ToString());
            return m_builder.ToString(0, m_builder.Length);
        }
    }
}




