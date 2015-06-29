using UnityEngine;
using System.Collections;


/// <summary>
/// 界面组件的父类
/// </summary>
public class KComponent : MonoBehaviour
{
    /// <summary>
    /// 界面显示出来 将调用组件的这个方法。
    /// </summary>
    public virtual void OnShown() { }

    /// <summary>
    /// 需要的数据
    /// </summary>
    public virtual object DataContent { get; set; }
}

public class KComponent<T> : MonoBehaviour where T : class
{
    /// <summary>
    /// 界面显示出来 将调用组件的这个方法。
    /// </summary>
    public virtual void OnShown() { }


    /// <summary>
    /// 需要的数据
    /// </summary>
    public virtual T DataContent { get; set; }
}