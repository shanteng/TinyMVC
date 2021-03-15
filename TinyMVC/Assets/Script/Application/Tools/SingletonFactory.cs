
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/// <summary>
/// 单例工厂,使用单例的可以直接继承他即可
/// </summary>
/// 

public class SingletonFactory<T> where T : new()
{
    private static readonly object sycObj = new object();
    private static T t;
    public static T Instance
    {
        get
        {
            if (t == null)
            {
                lock (sycObj)
                {
                    if (t == null)
                    {
                        t = new T();
                    }
                }
            }
            return t;
        }
    }


    public SingletonFactory()
    {
        this.init();
    }

    protected virtual void init()
    {

    }
}
