

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

//资源加载Proxy
public class ResourceProxy : BaseProxy
{
    private readonly Dictionary<string, AssetBundle> _assetBundles;//内存种保存的ab
    private readonly Dictionary<string, SpriteAtlas> _atlases;
    private readonly Dictionary<string, AssetBundle> _assetBundlesNeedUnload;//等待unload的ab
    private AssetBundleManifest _manifest;
    private string _resourcePath;

    public ResourceProxy() : base(ProxyNameDefine.RESOURCE)
    {
        _atlases = new Dictionary<string, SpriteAtlas>();
        _assetBundles = new Dictionary<string, AssetBundle>();
        _assetBundlesNeedUnload = new Dictionary<string, AssetBundle>();
        SpriteAtlasManager.atlasRequested += RequestAtlas;
        if (GameIndex.UseAssetBundle)
        {
            LoadManifest();
        }
    }

    private void RequestAtlas(string tag, Action<SpriteAtlas> callback)
    {
        var sa = LoadAtlas(tag, true);
        callback(sa);
    }

    private void LoadManifest()
    {
        _resourcePath = Path.Combine(Application.persistentDataPath, "M6Resources/");
        var path = Path.Combine(_resourcePath, $"BuildResources");

        if (false == File.Exists(path))
            path = CommonFunction.GetStreamAssetPath($"BuildResources");
     
        var bundle = AssetBundle.LoadFromFile(path);
        if (bundle)
        {
            var obj = bundle.LoadAsset("AssetBundleManifest");
            bundle.Unload(false);
            _manifest = obj as AssetBundleManifest;
        }
    }

    public SpriteAtlas LoadAtlas(string name, bool keepAtlas)
    {
        SpriteAtlas sa = null;
        if (_atlases.TryGetValue(name, out sa))
        {
            return sa;
        }

        if (GameIndex.UseAssetBundle)
        {
            sa = LoadAtlasFormAb(name, keepAtlas, false);
        }
        else
        {
            sa = CommonFunction.LoadAssetAtPath<SpriteAtlas>($"Assets/DevResources/atlas/{name}.spriteatlas");
            _atlases.Add(name, sa);
        }
        return sa;
    }

    private SpriteAtlas LoadAtlasFormAb(string name, bool keepAtlas, bool keepAb)
    {
        SpriteAtlas sa;
        var ab = LoadAssetbundle($"{name}.atlas", keepAb);
        if (ab)
        {
            sa = ab.LoadAsset<SpriteAtlas>(name);
            if (keepAtlas)
            {
                _atlases.Add(name, sa);
            }
            if (!keepAb)
            {
                UnloadAssetBundle(ab);
            }
            return sa;
        }
        return null;
    }

    public T LoadObject<T>(string path, string abFile, string suffix, bool keepAb) where T : UnityEngine.Object
    {
        if (GameIndex.UseAssetBundle && !string.IsNullOrEmpty(abFile))
        {
            var ab = LoadAssetbundle(abFile, keepAb);
            if (ab)
            {
                var fileName = Path.GetFileName(path);
               var obj = ab.LoadAsset<T>(fileName);
                if (!keepAb)
                {
                    UnloadAssetBundle(ab);
                }
                return obj;
            }
            return null;
        }
        else
        {
            return CommonFunction.LoadAssetAtPath<T>($"Assets/DevResources/ui/{path}.{suffix}");
        }
    }


    private AssetBundle LoadAssetbundle(string abName, bool keepAb)
    {
        AssetBundle ab = null;
        try
        {
            abName = abName.ToLower();
            if (!IsLoadedAssetBundle(abName, out ab))
            {
                LoadAssetBundleDependcy(abName, keepAb);
                ab = AssetBundle.LoadFromFile(GetAssetBundlePath(abName));
                SaveAssetBundle(ab, keepAb);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Load Error {abName} - {e}");
        }
        return ab;
    }

    bool IsLoadedAssetBundle(string abName, out AssetBundle ab)
    {
        if (!_assetBundles.TryGetValue(abName, out ab))
        {
            return _assetBundlesNeedUnload.TryGetValue(abName, out ab);
        }
        return true;
    }

    private void LoadAssetBundleDependcy(string path, bool keepAb)
    {
        keepAb = true;//todo  被依赖的资源先不释放了
        if (_manifest != null)
        {
            var dependencies = _manifest.GetAllDependencies(path);
            var count = dependencies.Length;
            if (dependencies.Length > 0)
            {
                //load all dependencies
                for (var i = 0; i < count; i++)
                {
                    LoadAssetbundle(dependencies[i], keepAb);
                }
            }
        }
    }

    string GetAssetBundlePath(string abname)
    {
        string destab = abname.ToLower();
        var path = Path.Combine(_resourcePath, destab);
        return path;
    }

    void SaveAssetBundle(AssetBundle ab, bool keepAb)
    {
        if (!_assetBundles.ContainsKey(ab.name))
        {
            if (keepAb)
            {
                _assetBundles.Add(ab.name, ab);
            }
            else
            {
                if (!_assetBundlesNeedUnload.ContainsKey(ab.name))
                {
                    _assetBundlesNeedUnload.Add(ab.name, ab);
                }
            }
        }
    }

    void UnloadAssetBundle(AssetBundle ab)
    {
        if (null == ab || _assetBundles.ContainsKey(ab.name))
            return;
        if (_assetBundlesNeedUnload.ContainsKey(ab.name))
        {
            _assetBundlesNeedUnload.Remove(ab.name);
        }
        ab.Unload(false);
        ab = null;
    }


}//end class