using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour
{
    public UnitySdkUI _SdkUi;
    private Camera _camera;
    private Dictionary<WindowLayer,Transform> _windowLayers;
    public static UIRoot Intance { get; private set; }

    public Camera UICamera => this._camera;
    void Awake()
    {
      
        this._camera = this.GetComponent<Canvas>().worldCamera;
        Intance = this;
        this._windowLayers = new Dictionary<WindowLayer, Transform>();
        int count = this.transform.childCount;
        for (int i = 0; i < count; ++i)
        {
            Transform layer = this.transform.GetChild(i);
            layer.gameObject.SetActive(true);
            this._windowLayers.Add((WindowLayer)i+1,layer);
        }
        this.SetSdkVisible(true);
    }

    public void SetSdkVisible(bool vis)
    {
        this._SdkUi.gameObject.SetActive(vis);
    }

    public GameObject InstantiateUIInCenter(GameObject obj, WindowLayer layer, bool NeedAnchor = true, bool NeedZDepth = true)
    {
        Transform parent = this.GetLayer(layer);
        GameObject view = GameObject.Instantiate(obj, parent, false);
        this.ShowUIInCenter(view, NeedAnchor, NeedZDepth);
        return view;
    }

    public void ShowUIInCenter(GameObject ui, bool setAnchorCenter, bool NeedZDepth = false)
    {
        var rectForm = ui.GetComponent<RectTransform>();
        if (setAnchorCenter)
        {
            var offmini = rectForm.offsetMin;
            var offmax = rectForm.offsetMax;
            rectForm.offsetMax = Vector2.zero;
            rectForm.offsetMin = Vector2.zero;
        }
        rectForm.localScale = Vector3.one;
        if (NeedZDepth)
        {
            rectForm.localPosition = new Vector3(0, 0, -1000);
        }
        else
        {
            rectForm.localPosition = Vector3.zero;
        }
        ui.SetActive(true);
    }

    public Transform GetLayer(WindowLayer layer)
    {
        return this._windowLayers[layer];
    }

}
