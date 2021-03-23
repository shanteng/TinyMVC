

using UnityEngine;
public class GameLoadRequest : AsyncOperation
{
    public string Key { get; protected set; }
    public string Path { get; protected set; }
    public string AbName { get; protected set; }
    public string Suffix { get; protected set; }
    public bool KeepAb { get; set; }

    public AssetBundle assetBundle { get; protected set; }

    public ILoadListener Listener { get; set; }
}

public interface ILoadListener
{
    void OnLoadProgress(string key, float progress);
    void OnLoadSuccess(string key, Object obj);
    void OnLoadFailed(string key, string error);
}