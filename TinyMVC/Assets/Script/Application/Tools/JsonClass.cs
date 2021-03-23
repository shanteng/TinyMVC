
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEngine;

public enum JsonKeyType
{
    INT,
    STRING,
}

public class ConfigDefine
{

	public static string ItemInfo = "ItemInfo";
	public static string Language = "Language";
	public static string LanError = "LanError";
}
public class LanErrorConfig : Config<LanErrorConfig>
{
	public string Value;
	public LanErrorConfig() : base(ConfigDefine.LanError,JsonKeyType.STRING) { }
}

public class LanguageConfig : Config<LanguageConfig>
{
	public string Value;
	public LanguageConfig() : base(ConfigDefine.Language,JsonKeyType.STRING) { }
}

public class ItemInfoConfig : Config<ItemInfoConfig>
{
	public string Icon;
	public string Name;
	public string Desc;
	public ItemInfoConfig() : base(ConfigDefine.ItemInfo,JsonKeyType.STRING) { }
}










