using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIView : MonoBehaviour
{
    public Button _btn;
    public Text _Text;
    // Start is called before the first frame update
    void Start()
    {
        this._btn.onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        //测试MVC的通信,Mediator可以直接监听接收
        //TestProxy通过TestCommand中介，实现和UI层的通信，将数据和UI进行分离
        ApplicationFacade.instance.SendNotification(NotiDefine.TEST_NOTI,"The Button Is Clicked!");
    }

    public void OnReceiveNoti(string text)
    {
        this._Text.text = text;
    }

    public void InitData()
    {
        this._Text.text = "Please click the button";
    }
}
