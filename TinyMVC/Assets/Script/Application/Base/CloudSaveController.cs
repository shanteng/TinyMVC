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
        characterInfo = CloudSave.OpenOrCreateDataset("CharacterInfo");

        //游客账号将不会同步云端数据
        if(PlayerIdentityManager.Current.loginStatus == LoginStatus.LoggedIn)
            characterInfo.SynchronizeOnConnectivityAsync(this);
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
        characterInfo.SynchronizeOnConnectivityAsync(this);
    }

    public void SaveToCloud(string jsonName, string jsonStr)
    {
        characterInfo.Put(jsonName, jsonStr);
        characterInfo.SynchronizeOnConnectivityAsync(this);
    }

    public string LoadCloudData(string jsonName)
    {
        return characterInfo.Get(jsonName);
    }

    public bool OnConflict(IDataset dataset, IList<SyncConflict> conflicts)
    {
        List<Record> resolvedRecords = new List<Record>();

        foreach (SyncConflict conflictRecord in conflicts)
        {
            // This example resolves all the conflicts using ResolveWithRemoteRecord 
            // Cloudsave provides the following default conflict resolution methods:
            //      ResolveWithRemoteRecord - overwrites the local with remote records
            //      ResolveWithLocalRecord - overwrites the remote with local records
            //      ResolveWithValue - for developer logic  
            //使用本地的解决冲突
            resolvedRecords.Add(conflictRecord.ResolveWithLocalRecord());
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
        Debug.Log("Successfully synced for dataset: " + dataset.Name);
    }

}
