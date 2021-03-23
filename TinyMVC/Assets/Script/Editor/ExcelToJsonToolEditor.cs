#if UNITY_EDITOR
using Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
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
            string path = System.Environment.CurrentDirectory + "/Assets/Resources/config/init.txt";
            EditorUtility.RevealInFinder(path);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(_curFinishText);
        EditorGUILayout.EndHorizontal();
    }

    void DoAllXlsxToJson(string singleExcel = "")
    {
        string filepath = Application.dataPath + "/Script/Application/Tools/JsonClass.cs";
        _csharpText = File.ReadAllText(filepath);
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

        //将代码写回c#文件
        if (File.Exists(filepath))
        {
            File.SetAttributes(filepath, FileAttributes.Normal);
            File.WriteAllText(filepath, this._csharpText);
        }
    }//end function

    private string _csharpText = "";
    private bool DoXlsxToJson(ExcelConfig config)
    {
        string dataPath = System.Environment.CurrentDirectory;
        // xlsx路径
        string xlsxPath = dataPath + "/Excel/" + config.name + ".xlsx";
        string savePath = Application.dataPath + "/Resources/config/";
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
        excelReader.Read();//需要调用一下Read，否则下面AsDataSet 方法会返回null，不知为何，困惑
        DataSet result = excelReader.AsDataSet();
        if (result == null)
        {
            Debug.LogError(config.name + " Has Empty Line!");
            return false;
        }

        // 读取
        foreach (string exSheet in config.sheets)
        {
            List<TypeValue> kvList = new List<TypeValue>();
            JsonKeyType idType = ReadSingleSheet(result.Tables[exSheet], savePath + exSheet + ".json", ref kvList);
            this.GenerateSheetCSharpCode(exSheet, kvList, idType);
            this._curFinishText += "----转表:" + exSheet + ".json完成\n";
        }

        stream.Close();

        return true;
    }//end function

    public class UICodeConfig
    {
        public const string classDefine = "\tpublic static string {0} = \"{1}\";\n";
        public const string variable = "\tpublic {0} {1};";
        public const string construct = "public {0}() : base(ConfigDefine.{1},{2})";
    }

    private void GenerateSheetCSharpCode(string sheetName, List<TypeValue> memberList, JsonKeyType keyType)
    {
        int startIndex = this._csharpText.IndexOf("ConfigDefine");
        int endIndex = this._csharpText.IndexOf("}", startIndex) + 1;
        string defineString = _csharpText.Substring(startIndex, endIndex - startIndex);
        string strHas = sheetName + " = ";
        if (defineString.Contains(strHas))
            return;//已经有定义了
        string classDefine = EditorUtil.format(UICodeConfig.classDefine, sheetName, sheetName);
        //插入Config定义到代码里
        this._csharpText = this._csharpText.Insert(endIndex - 1, classDefine);

        //类定义代码
        startIndex = this._csharpText.IndexOf("ConfigDefine");
        endIndex = this._csharpText.IndexOf("}", startIndex) + 1;

        int classStartIndex = endIndex + 1;
        string configStr = StringUtils.combine(sheetName, "Config");
        string inhiertStr = "Config<" + configStr + ">";

        string className = "\npublic class " + configStr + " : " + inhiertStr;
        List<string> memberStrList = new List<string>();
        int count = memberList.Count;
        for (int i = 0; i < count; ++i)
        {
            string memberStr = EditorUtil.format(UICodeConfig.variable, memberList[i].memberType, memberList[i].memberName);
            memberStrList.Add(memberStr);
        }
        string finalMemberStr = string.Join("\n", memberStrList);//finalMemberStr
        className = className + "\n{\n" + finalMemberStr;


        //构造函数代码
        string KeyTypeStr = "JsonKeyType.STRING";
        if (keyType == JsonKeyType.INT)
            KeyTypeStr = "JsonKeyType.INT";
        string constructStr = EditorUtil.format(UICodeConfig.construct, configStr, sheetName, KeyTypeStr) +" { }";//construct
        className = className + "\n\t" + constructStr+"\n}\n";

        this._csharpText = this._csharpText.Insert(endIndex + 1, className);
    }

    public class TypeValue
    {
        public string memberType;
        public string memberName;
    }
    private JsonKeyType ReadSingleSheet(DataTable dataTable, string jsonPath,ref List<TypeValue> kvList)
    {
        JsonKeyType type = JsonKeyType.INT;
        kvList = new List<TypeValue>();//返回成员变量名字和类型
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

            string memberName = jsonFileds[i];
            if (memberName.Equals("ID") || memberName.Equals("IDs"))
            {
                continue;
            }

            TypeValue kv = new TypeValue();
            kv.memberName = jsonFileds[i];
            switch (jclassTypes[i])
            {
                case "String":
                    kv.memberType = "string";
                    break;
                case "Int":
                    kv.memberType = "int";
                    break;
                case "ArrayInt":
                case "ArrayIntLine":
                    kv.memberType = "int[]";
                    break;
                case "ArrayString":
                case "StringArrayLine":
                    kv.memberType = "string[]";
                    break;
                case "Float":
                    kv.memberType = "float";
                    break;
                case "ArrayFloat":
                    kv.memberType = " float[]";
                    break;
                default:
                    kv.memberType = "string";
                    break;
            }//end switch
            kvList.Add(kv);
        }//end for

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
                        int valueint = EditorUtil.ParseInt(str);
                        value = Convert.ChangeType(valueint, typeof(int));
                        break;
                    case "ArrayInt":
                        if (str.Length > 0)
                        {
                            string[] strs = str.Split(',');
                            int[] ints = new int[strs.Length];
                            for (int k = 0; k < strs.Length; k++)
                            {
                                ints[k] = EditorUtil.ParseInt(strs[k]);
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
                                ints[k] = EditorUtil.ParseInt(strs[k]);
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
                        float valuefloat = EditorUtil.ParseFloat(str);
                        value = Convert.ChangeType(valuefloat, typeof(float));
                        break;
                    case "ArrayFloat":
                        if (str.Length > 0)
                        {
                            string[] strs = str.Split(',');
                            float[] floats = new float[strs.Length];
                            for (int k = 0; k < strs.Length; k++)
                            {
                                floats[k] = EditorUtil.ParseFloat(strs[k]);
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

                if (memberName.Equals("IDs"))
                    type = JsonKeyType.STRING;
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
        return type;
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


public class EditorUtil
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

    public static string format(string valuestr, params object[] paramStrs)
    {
        string afterStr = "";
        try
        {
            afterStr = string.Format(valuestr, paramStrs);
        }
        catch (Exception ex)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(string.Format(": {0} 参数数量不匹配", valuestr));
#endif
        }
        return afterStr;
    }
}

#endif
