using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SMVC.Patterns;

//数据代理层的基类,用于存储数据和服务器交互，类似一个数据仓库，每个单独系统可以建立一个proxy来处理
public class BaseProxy : Proxy
{
    private static readonly object sycObj = new object();
    public BaseProxy(string name) : base(name)
    {
        NAME = name;
    }
}
