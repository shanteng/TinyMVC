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

    public void InitCloud()
    {
        _cloudSave = new CloudSave(PlayerIdentityManager.Current);
        CloudSaveInitializer.AttachToGameObject(this.gameObject);
        characterInfo = CloudSave.OpenOrCreateDataset("CharacterInfo");
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
        return true;//不处理
    }

    public void OnError(IDataset dataset, DatasetSyncException syncEx)
    {
        Debug.Log("Sync failed for dataset : " + dataset.Name);
    }

    public void OnSuccess(IDataset dataset)
    {
        Debug.Log("Successfully synced for dataset: " + dataset.Name);
    }

}
