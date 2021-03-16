using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Config<T> where T : new()
{
    public string jName;
    public int ID;
    public string IDs;
    public Dictionary<int, T> _configDic;
    public Dictionary<string, T> _configStringDic;

    private JsonKeyType keyType;

    private static readonly object sycObj = new object();
    private static T t;
    private bool _isInit = false;
    public static T Instance
    {
        get
        {
            if (t == null)
            {
                lock (sycObj)
                {
                    if (t == null)
                    {
                        t = new T();
                    }
                }
            }
            return t;
        }
    }

    public  JArray JsonRead(string name)
    {
        string json = "";
        string path = "Config/" + name;
        TextAsset text = Resources.Load<TextAsset>(path);
        if (text == null)
            return null;
        json = text.text;
        JArray jArray = JArray.Parse(json);
        return jArray;
    }

    public Config(string jName, JsonKeyType tt = JsonKeyType.INT)
    {
        this.jName = jName;
        this.keyType = tt;
        if (tt == JsonKeyType.INT)
            this._configDic = new Dictionary<int, T>();
        else if (tt == JsonKeyType.STRING)
            this._configStringDic = new Dictionary<string, T>();
    }
    private void Init()
    {
        if (this._isInit)
            return;
        this._isInit = true;
        //读取Json
        JArray list = this.JsonRead(this.jName);
        foreach (var item in list)
        {
            T oneJson = item.ToObject<T>();
            if (this.keyType == JsonKeyType.INT)
            {
                int ID = item.Value<int>("ID");
                this._configDic[ID] = oneJson;
            }
            else if (this.keyType == JsonKeyType.STRING)
            {
                string IDs = item.Value<string>("IDs");
                this._configStringDic[IDs] = oneJson;
            }
        }//end for
    }


    public Dictionary<int, T> getDataArray()
    {
        this.Init();
        return this._configDic;
    }

    public Dictionary<string, T> getStrDataArray()
    {
        this.Init();
        return this._configStringDic;
    }

    public T GetData(int id)
    {
        this.Init();
        T config;
        if (this._configDic.TryGetValue(id, out config))
            return config;
        return default(T);
    }

    public T GetData(string ids)
    {
        this.Init();
        T config;
        if (this._configStringDic.TryGetValue(ids, out config))
            return config;
        return default(T);
    }
}