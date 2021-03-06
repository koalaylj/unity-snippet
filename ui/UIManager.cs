using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 界面管理类
/// author:于小懒
/// </summary>
public class UIManager
{

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 界面根节点
    /// </summary>
    private Transform _root;

    public Transform CacheRoot
    {
        get
        {
            if (_root == null)
            {
                GameObject go = GameObject.Find("Scene/UI/Camera");
                if (go != null)
                {
                    _root = go.transform;
                }
            }

            if (_root == null)
            {
                _root = Camera.main.transform;
            }

            return _root;
        }
    }


    private Transform _root_scene;
    public Transform CacheRootScene
    {
        get
        {
            if (_root_scene == null)
            {
                GameObject obj = GameObject.Find("Scene/UI/CameraScene");
                if (obj != null)
                    _root_scene = obj.transform;
            }
            return _root_scene;
        }
    }


    // private Stack<Presenter> _uiStack = new Stack<Presenter>();

    /// <summary>
    /// 显示某个界面
    /// </summary>
    /// <param name="uiname"></param>
    /// <returns></returns>
    public Presenter Show(string uiname)
    {
        GameObject ui = FindUI(uiname);
        Presenter page = Show(ui);
        return page;
    }

    /// <summary>
    /// 获得UI类
    /// </summary>
    /// <param name="uiname"></param>
    /// <returns></returns>
    public Presenter GetPresenter(string uiname)
    {
        GameObject ui = FindUI(uiname);
        if (ui != null)
            return ui.GetComponent<Presenter>();
        return null;
    }

    public Transform FindObj(string uiname)
    {
        Transform ui = CacheRoot.FindChild(uiname); ;
        if (ui != null)
            return ui;
        return null;
    }


    private Presenter Show(GameObject ui)
    {
        Presenter page = ui.GetComponent<Presenter>();
        if (page != null)
        {
            Show(page);
        }
        else
        {
            ui.SetActive(true);
        }
        return page;
    }

    public void Show(Presenter page)
    {
        // page.gameObject.GetComponent<UIPanel>().depth = _uiStack.Count * 10;
        // _uiStack.Push(page);
        page.OnShowing();
        page.gameObject.SetActive(true);
        page.OnShown();
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="uiname"></param>
    public void Hide(string uiname)
    {
        GameObject ui = FindUI(uiname);
        Hide(ui);
    }

    private void Hide(GameObject ui)
    {
        Presenter page = ui.GetComponent<Presenter>();
        if (page != null)
        {
            Hide(page);
        }
        else
        {
            ui.SetActive(false);
        }
    }

    public void Hide(Presenter page = null)
    {
        // Presenter p = _uiStack.Pop();

        //   if (page == null)
        //{
        //    page = p;
        //}

        page.OnClosing();
        page.gameObject.SetActive(false);
        page.OnClosed();
    }

    //private Vector3 _messagebox_pos = new Vector3(0, 0, -550);

    public void ShowMessagebox(MessageBox info)
    {
        Presenter p = null;
        if (info.Buttons == MessageBoxButtons.OkCancel)
        {
            p = Show("Panel_MessageBox2");
        }
        else if (info.Buttons == MessageBoxButtons.Ok)
        {
            p = Show("Panel_MessageBox1");
        }
        else
        {
            p = Show("Panel_MessageBoxNoNet");
        }

        //p.transform.localPosition = _messagebox_pos;
        p.DataContent = info;
    }

    /// <summary>
    /// 显示提示消息，只有一个确定按钮。
    /// </summary>
    /// <param name="info"></param>
    public void ShowMessagebox(string info)
    {
        ShowMessagebox(new MessageBox(info));
    }


    /// <summary>
    /// 关闭消息框，一般无需自己调用。
    /// </summary>
    public void CloseMessageBox1()
    {
        Hide("Panel_MessageBox1");
    }

    /// <summary>
    /// 关闭消息框，一般无需自己调用。
    /// </summary>
    public void CloseMessageBox2()
    {
        Hide("Panel_MessageBox2");
    }

    /// <summary>
    /// 显示错误信息
    /// </summary>
    /// <param name="errorCode"></param>
    public void ShowError(int code, Action callback = null)
    {
        if (CODE.ERROR_DESC.ContainsKey(code))
        {
#if UNITY_EDITOR
            ShowMessagebox(new MessageBox(code + ":" + CODE.ERROR_DESC[code], MessageBoxButtons.Ok, (result) => { if (callback != null) callback(); }));
#else
            ShowMessagebox(new MessageBox(CODE.ERROR_DESC[code], MessageBoxButtons.Ok, (result) => { if (callback != null) callback(); }));
#endif
        }
        else
        {
            ShowMessagebox(new MessageBox("未知错误:" + code, MessageBoxButtons.Ok, (result) => { if (callback != null) callback(); }));
        }
        UIManager.Instance.EndWaiting();
    }

    /// <summary>
    /// 显示菊花
    /// </summary>
    public void BeginWaiting()
    {
        Show("Panel_Loading_Correspond");
    }

    /// <summary>
    /// 关闭菊花
    /// </summary>
    public void EndWaiting()
    {
        Hide("Panel_Loading_Correspond");
    }

    /// <summary>
    /// 在camera下查找某个界面
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject FindUI(string name)
    {
        Transform ui = CacheRoot.FindChild(name);

        if (ui == null)
        {
            GameObject panel = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/MainScenePanelPrefabs/" + name));
            if(panel != null)
            {
                Vector3 pos = panel.transform.localPosition;
                ui = panel.transform;
                ui.parent = CacheRoot;
                ui.localPosition = pos;
                ui.localScale = Vector3.one;
                ui.name = name;
            }
        }

        if (ui == null)
        {
            throw new Exception("无法在" + _root.name + "下找到界面：" + name);
        }
        return ui.gameObject;
    }
}