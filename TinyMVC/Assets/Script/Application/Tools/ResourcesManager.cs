using UnityEngine.U2D;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : SingletonFactory<ResourcesManager>
{
    private static ResourceProxy _resourceloader;
    private static ResourceProxy _resourceProxy => _resourceloader ?? (_resourceloader =
                                                      ApplicationFacade.instance.RetrieveProxy(ProxyNameDefine
                                                          .RESOURCE) as ResourceProxy);

    private string UI_Path = "UI/";
    private string Atlas_Path = "Atlas/";

    public const string ITEM_ATLAS = "Item"; //道具图标
    public GameObject LoadUIRes(string resName, bool keepAb = false)
    {
        return _resourceProxy?.LoadObject<GameObject>(resName, $"{resName}.pbab", "prefab", keepAb);
    }

    public Sprite getAtlasSprite(string atlasName, string spName)
    {
        //读取SpriteAtlas的Sprite
        var sAtlas = GetAtlas(atlasName);
        if (sAtlas == null)
            return null;
        var sprite = sAtlas.GetSprite(spName);
        return sprite;
    }

    private SpriteAtlas GetAtlas(string atlasName)
    {
        SpriteAtlas atlas = ResourcesManager.LoadAtlas(atlasName);
        return atlas;
    }

    public static SpriteAtlas LoadAtlas(string path)
    {
        return _resourceProxy?.LoadAtlas(path, true);
    }
}