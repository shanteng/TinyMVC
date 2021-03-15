

using UnityEngine;
public class TestProxy : BaseProxy
{
    public TestProxy() : base(ProxyNameDefine.TEST)
    {

    }

    public void OnReceiveNoti(string text)
    {
        Debug.LogError("TestProxy Has Receive Noti:" + text);
    }
}