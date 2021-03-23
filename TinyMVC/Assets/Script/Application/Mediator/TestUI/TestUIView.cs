using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIView : MonoBehaviour
{
    public Button _btn;
    public Text _Text;
    public Image _TestSpriteAtlasItem;
    public Text _JsonTestTxt;
    // Start is called before the first frame update
    void Start()
    {
        this._btn.onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        //测试加载Json配置
        ItemInfoConfig config = ItemInfoConfig.Instance.GetData("gold");
        this._JsonTestTxt.text = config.Name;
        //测试加载SpriteAtlas
        this._TestSpriteAtlasItem.sprite = ResourcesManager.Instance.getAtlasSprite(ResourcesManager.ITEM_ATLAS, "gold");
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
        this._Text.text = "click test Mvc Command!";
    }
}
