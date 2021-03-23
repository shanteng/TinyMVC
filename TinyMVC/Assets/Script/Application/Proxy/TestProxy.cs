

using UnityEngine;
public class TestProxy : BaseProxy
{
    public TestProxy() : base(ProxyNameDefine.TEST)
    {

    }

    public void OnReceiveNoti(string text)
    {
        Debug.Log("--------TestProxy Has Receive Noti:" + text);
    }
}