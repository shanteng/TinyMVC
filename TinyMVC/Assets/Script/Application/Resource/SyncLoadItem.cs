

using UnityEngine;

public class SyncLoader
{
    public bool isDone;
    public Object obj;
}

public class SyncLoadItem : GameLoadRequest
{
    public AssetBundleCreateRequest AbRequest;
    public AssetBundleRequest ObjRequset;

    private SyncLoader _unityLoader;
    public string FileName;
    public Object Obj { get; private set; }
    private bool _isDone = false;

    public SyncLoadItem(string key, string path, string abName, string suffix, ILoadListener listener, bool keepAb)
    {
        Key = key;
        Path = path;
        AbName = abName;
        Suffix = suffix;
        FileName = string.IsNullOrEmpty(path) ? string.Empty : System.IO.Path.GetFileName(path.ToLower());
        Listener = listener;
        KeepAb = keepAb;
    }

    public SyncLoader UnityLoader
    {
        get
        {
            if (null == _unityLoader)
            {
                _unityLoader = new SyncLoader();
                _unityLoader.isDone = isDone;
                _unityLoader.obj = Obj;
            }
            return _unityLoader;
        }

    }

    public new bool isDone
    {
        get
        {
            return _isDone;
        }

    }

    public new float progress
    {
        get
        {
            if (null != ObjRequset)
            {
                return (0.95f + 0.05f * ObjRequset.progress);
            }
            if (null != AbRequest)
            {
                return 0.95f * AbRequest.progress;
            }
            return 0;
        }
    }

    public void SetAssetbundle(AssetBundle ab)
    {
        assetBundle = ab;
        if (assetBundle)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                ObjRequset = assetBundle.LoadAssetAsync(FileName);
            }
            else
            {
                SetObject(null);
            }
        }
        else
        {
            Obj = null;
            Listener?.OnLoadFailed(Key, "null");
        }
    }

    public void SetObject(Object o)
    {
        Obj = o;
        _isDone = true;
        if (null != _unityLoader)
        {
            _unityLoader.isDone = true;
            _unityLoader.obj = o;
        }
        if (null != Listener)
        {
            if (Obj || null == ObjRequset)
            {
                Listener.OnLoadSuccess(Key, o);
            }
            else
            {
                Listener.OnLoadFailed(Key, "null");
            }
        }
    }

}//end class