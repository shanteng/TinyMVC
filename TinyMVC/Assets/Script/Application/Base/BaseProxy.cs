using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SMVC.Patterns;

//数据代理层的基类
public class BaseProxy : Proxy
{
    private static readonly object sycObj = new object();
    public BaseProxy(string name) : base(name)
    {
        NAME = name;
    }
}
