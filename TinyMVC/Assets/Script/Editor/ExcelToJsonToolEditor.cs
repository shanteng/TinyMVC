
using Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExcelToJsonTool))]
public class ExcelToJsonToolEditor : Editor
{
    private string _curFinishText = "";
    private string _excelName = "";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _excelName = EditorGUILayout.TextField("Excel名(不填全转):", _excelName);
        if (GUILayout.Button("Excel表转Json"))
        {
            this._curFinishText = "";
            this.DoAllXlsxToJson(this._excelName);
            this._curFinishText += "转表结束...";
        }
        else if (GUILayout.Button("打开Excel文件夹"))
        {
            string path = System.Environment.CurrentDirectory + "/Excel/init.txt";
            EditorUtility.RevealInFinder(path);
        }
        else if (GUILayout.Button("打开Json文件夹"))
        {
            string path = System.Environment.CurrentDirectory + "/Assets/Resources/Config/init.txt";
            EditorUtility.RevealInFinder(path);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(_curFinishText);
        EditorGUILayout.EndHorizontal();
    }

    void DoAllXlsxToJson(string singleExcel = "")
    {
        string dataPath = UnityEngine.Application.dataPath;
        ExcelToJsonTool excels = target as ExcelToJsonTool;
        foreach (ExcelConfig excelName in excels._Excels)
        {
            if (excelName.name.Equals(singleExcel) || singleExcel.Equals(""))
            {
                this._curFinishText += "读取Excel:" + excelName.name + "\n";
                this.DoXlsxToJson(excelName);
                this._curFinishText += "\n";
                if (singleExcel.Equals("") == false)
                    break;
            }
        }
    }//end function

    private bool DoXlsxToJson(ExcelConfig config)
    {
        string dataPath = System.Environment.CurrentDirectory;
        // xlsx路径
        string xlsxPath = dataPath + "/Excel/" + config.name + ".xlsx";
        string savePath = Application.dataPath + "/Resources/Config/";
        FileStream stream = null;
        try
        {
            stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read);
        }
        catch (IOException e)
        {
            this._curFinishText = "请关闭Excel后再进行";
            stream.Close();
            return false;
        }

        if (stream == null)
            return false;
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        excelReader.Read();//需要调用一下Read，否则下面AsDataSet 方法会返回null，不知为何
        DataSet result = excelReader.AsDataSet();
        if (result == null)
        {
            Debug.LogError(config.name + " Has Empty Line!");
            return false;
        }
       
        // 读取
        foreach (string exSheet in config.sheets)
        {
            ReadSingleSheet(result.Tables[exSheet], savePath + exSheet + ".json");
            this._curFinishText += "----转表:" + exSheet + ".json完成\n";
        }

        stream.Close();
        return true;
    }//end function

    private  void ReadSingleSheet(DataTable dataTable, string jsonPath)
    {
        int rows = dataTable.Rows.Count;
        int Columns = dataTable.Columns.Count;
        DataRowCollection collect = dataTable.Rows;
        // xlsx对应的数据字段，规定是第二行
        // xlsx对应的数据字段類型，规定是第3行
        string[] jsonFileds = new string[Columns];
        string[] jclassTypes = new string[Columns];
        for (int i = 0; i < Columns; i++)
        {
            jsonFileds[i] = collect[1][i].ToString();
            jclassTypes[i] = collect[2][i].ToString();
        }

        List<object> objsToSave = new List<object>();
        // 数据从第4行开始
        for (int i = 3; i < rows; i++)
        {
            JObject postedJObject = new JObject();
            bool isEnd = false;
            for (int j = 0; j < Columns; j++)
            {
                string memberName = jsonFileds[j];
                object value = null;
                string classType = jclassTypes[j];
                string str = collect[i][j].ToString();
                switch (classType)
                {
                    case "String":
                        value = Convert.ChangeType(collect[i][j], typeof(string));
                        break;
                    case "Int":
                        int valueint = ExcelToJsonTool.ParseInt(str);
                        value = Convert.ChangeType(valueint, typeof(int));
                        break;
                    case "ArrayInt":
                        if (str.Length > 0)
                        {
                            string[] strs = str.Split(',');
                            int[] ints = new int[strs.Length];
                            for (int k = 0; k < strs.Length; k++)
                            {
                                ints[k] = ExcelToJsonTool.ParseInt(strs[k]);
                            }
                            value = ints;
                        }
                        else
                        {
                            value = new int[0];
                        }
                        break;
                    case "ArrayIntLine":
                        if (str.Length > 0)
                        {
                            string[] strs = str.Split('|');
                            int[] ints = new int[strs.Length];
                            for (int k = 0; k < strs.Length; k++)
                            {
                                ints[k] = ExcelToJsonTool.ParseInt(strs[k]);
                            }
                            value = ints;
                        }
                        else
                        {
                            value = new int[0];
                        }
                        break;
                    case "ArrayString":
                        if (str.Length > 0)
                        {
                            value = str.Split(',');
                        }
                        else
                        {
                            value = new string[0];
                        }
                        break;
                    case "StringArrayLine":
                        if (str.Length > 0)
                        {
                            value = str.Split('|');
                        }
                        else
                        {
                            value = new string[0];
                        }
                        break;
                    case "Float":
                        float valuefloat = ExcelToJsonTool.ParseFloat(str);
                        value = Convert.ChangeType(valuefloat, typeof(float));
                        break;
                    case "ArrayFloat":
                        if (str.Length > 0)
                        {
                            string[] strs = str.Split(',');
                            float[] floats = new float[strs.Length];
                            for (int k = 0; k < strs.Length; k++)
                            {
                                floats[k] = ExcelToJsonTool.ParseFloat(strs[k]);
                            }
                            value = floats;
                        }
                        else
                        {
                            value = new float[0];
                        }
                        break;
                    default:
                        Convert.ChangeType(collect[i][j], typeof(string));
                        break;
                }//end switch

                if (memberName.Equals("ID") || memberName.Equals("IDs"))
                {
                    if (str.Equals(""))
                    {
                        isEnd = true;
                        break;
                    }
                }

                if (value == null)
                {
                    Debug.Log("value is Null");
                }

                postedJObject.Add(memberName, JToken.FromObject(value));
            }//end for j

            if (isEnd)
                break;
            objsToSave.Add(postedJObject);
        }//end for i
        // 保存为Json
        string content = Newtonsoft.Json.JsonConvert.SerializeObject(objsToSave, Formatting.Indented);
        SaveFile(content, jsonPath);
    }//end func

    private  void SaveFile(string content, string jsonPath)
    {
        StreamWriter streamWriter;
        FileStream fileStream;
        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
        }
        fileStream = new FileStream(jsonPath, FileMode.Create);
        streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(content);
        streamWriter.Close();
    }

}//end class


