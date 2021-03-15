using UnityEngine.U2D;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : SingletonFactory<ResourcesManager>
{
    private string UI_Path = "UI/";
    private string Atlas_Path = "Atlas/";

    public GameObject LoadUIRes(string resName)
    {
        //UI目录下的预制体加载
        return Resources.Load<GameObject>(UI_Path + resName);
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

    private Dictionary<string, SpriteAtlas> _atlasDic = new Dictionary<string, SpriteAtlas>();
    private SpriteAtlas GetAtlas(string atlasName)
    {
        SpriteAtlas atlas = null;
        if (!_atlasDic.TryGetValue(atlasName, out atlas))
        {
            atlas = Resources.Load<SpriteAtlas>(Atlas_Path + atlasName);
            _atlasDic.Add(atlasName, atlas);
        }
        return atlas;
    }
}