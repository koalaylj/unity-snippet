using UnityEngine;
using System.Collections;
using Pomelo.DotNetClient;
using System;

public class NetworkManager : MonoBehaviour
{
    public void Initialize()
    {
        GChannel.Instance.NetStateChangedEvent += (state) =>
        {
            StartCoroutine("ShowError", state);
        };
    }

    void Start()
    {
        // CheckNet();
    }

    void Update()
    {
        GChannel.Instance.PumpMessage();
    }

    private void CheckNet()
    {
        //if (PhoneUtil.NetworkConnected)
        //{
        //    UIManager.Instance.ShowWaiting(null);
        //    Client.Instance.Initialize();
        //    InvokeRepeating("Tick", 0f, 1f);
        //    UIManager.Instance.CloseWaiting();
        //}
        //else
        //{
        //    MessageBoxModel mb = new MessageBoxModel("提示", "未连接到网络，打开网络后点击确定。", MessageBoxButtons.Ok);
        //    IContext args = new ContextBase().Add("model", mb).Add("handler", new UIMessageBoxEventHandler(OnMessageBoxClose));
        //    UIManager.Instance.ShowMessageBox(args);
        //}
    }

    private IEnumerator ShowError(NetWorkState state)
    {
        yield return 1;

        switch (state)
        {
            case NetWorkState.CLOSED:
                break;
            case NetWorkState.CONNECTING:
                break;
            case NetWorkState.CONNECTED:
                break;
            case NetWorkState.TIMEOUT:
                UIManager.Instance.ShowMessagebox("");
                PrefsManager.HasLogin = false;
                break;
            case NetWorkState.DISCONNECTED:
            case NetWorkState.ERROR:
                PrefsManager.HasLogin = false;
                UIManager.Instance.ShowMessagebox(new MessageBox("", MessageBoxButtons.NoNet, (dialogResult) =>
                {
                    if (dialogResult == DialogResult.Ok)
                    {
                        GChannel.Instance.Disconnect();
                        SceneManager.Instance.LoadStartScene();
                        //UIManager.Instance.Hide("Panel_Login");
                        //UIManager.Instance.Show("Panel_Login");
                    }
                    else
                    {
                        Application.Quit();
                    }
                }));
                break;
            default:
                break;
        }
    }
}