using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerIdentity;
using UnityEngine.PlayerIdentity.UI;
using UnityEngine.CloudSave;

public class CloudSaveController : MonoBehaviour
    , ISyncCallback
{
    //云存储
    public IDataset characterInfo { get; private set; }
    private CloudSave _cloudSave;
    public CloudSave CloudSave
    {
        get
        {
            return _cloudSave;
        }
    }

    public static CloudSaveController Intance { get; private set; }
    void Awake()
    {
        Intance = this;
    }

    public void InitCloud()
    {
        CloudSaveInitializer.AttachToGameObject(this.gameObject);
        _cloudSave = new CloudSave(PlayerIdentityManager.Current);
        //打开或者新建 Dataset CharacterInfo
        characterInfo = CloudSave.OpenOrCreateDataset("CharacterInfo");
        //游客账号将不会同步云端数据
        if(PlayerIdentityManager.Current.loginStatus == LoginStatus.LoggedIn)
            characterInfo.SynchronizeAsync(this);//同步云端数据
    }

    public void ClearCloudData()
    {
        // CloudSave.WipeOut();
        List<Record> list = (List<Record>)this.characterInfo.GetAllRecords();
        int count = list.Count;
        for (int i = 0; i < count; ++i)
        {
            characterInfo.Put(list[i].Key, "");
        }
        //立即同步云端
        characterInfo.SynchronizeOnConnectivityAsync(this);
    }

    public void SaveToCloud(string jsonKey, string jsonStr)
    {
        //将数据存储为Json格式，方便序列化和反序列化
        characterInfo.Put(jsonKey, jsonStr);
        //立即同步云端
        characterInfo.SynchronizeOnConnectivityAsync(this);
    }

    public string LoadCloudData(string jsonKey)
    {
        return characterInfo.Get(jsonKey);
    }

    public bool OnConflict(IDataset dataset, IList<SyncConflict> conflicts)
    {
        List<Record> resolvedRecords = new List<Record>();
        foreach (SyncConflict conflictRecord in conflicts)
        {
            //使用云端的解决冲突
            resolvedRecords.Add(conflictRecord.ResolveWithRemoteRecord());
        }

        // resolves the conflicts in local storage
        dataset.ResolveConflicts(resolvedRecords);
        return true;
    }

    public void OnError(IDataset dataset, DatasetSyncException syncEx)
    {
        Debug.Log("Sync failed for dataset : " + dataset.Name);
        Debug.LogException(syncEx);
    }

    public void OnSuccess(IDataset dataset)
    {
        this.characterInfo = dataset;
        Debug.Log("Successfully synced for dataset: " + dataset.Name);
    }

}
