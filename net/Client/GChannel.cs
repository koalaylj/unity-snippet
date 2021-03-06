using UnityEngine;
using System;
using Pomelo.DotNetClient;
using SimpleJson;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 客户端网络处理类
/// </summary>
public class GChannel
{
    /// <summary>
    /// 邮件push事件
    /// </summary>
    public event Action<JsonObject> OnSysMailNumEvent;

    /// <summary>
    /// 广播push事件
    /// </summary>
    public event Action<JsonObject> OnBroadcastEvent;

    /// <summary>
    /// 聊天push事件
    /// </summary>
    public event Action<JsonObject> OnChatEvent;

    /// <summary>
    /// 网络变化消息事件
    /// 参数 ： 网络当前状态， 发生此变化的原因。
    /// </summary>
    internal event Action<NetWorkState> NetStateChangedEvent;

    /// <summary>
    /// 消息处理函数
    /// message -> messageHandler 对应表
    /// </summary>
    private Dictionary<string, Action<object>> _msg_handler = new Dictionary<string, Action<object>>();

    /// <summary>
    /// message -> server route 对应表
    /// </summary>
    private Dictionary<string, string> _msg_route_map = new Dictionary<string, string>();

    /// <summary>
    /// server route -> message 对应表
    /// </summary>
    private Dictionary<string, string> _route_msg_map = new Dictionary<string, string>();

    //消息队列
    private Queue<JsonObject> _recieveMsgQueue = new Queue<JsonObject>();

    private PomeloClient _pomelo;

    private string _host;
    private int _port;

    /// <summary>
    /// 本地消息默认消息id
    /// </summary>
    private int _localMsgId = 0;

    private int LocalMsgId
    {
        get { return _localMsgId; }
        set
        {
            if (value > int.MaxValue)
            {
                _localMsgId = 0;
            }
            _localMsgId = value;
        }
    }

    private GChannel()
    {
        string conf = IOUtil.LoadJson("Config.json");
        JsonObject jo = IOUtil.DeserializeObject(conf);
        //JsonObject jo = IOUtil.DeserializeObject<JsonObject>(conf);
        _host = jo["ip"].ToString();
        _port = int.Parse(jo["port"].ToString());

        List<Type> messages = ClassUtil.GetSubClassesOf(typeof(Message));
        Type attributeType = typeof(MessageAttribute);

        foreach (var item in messages)
        {
            Attribute attribute = Attribute.GetCustomAttribute(item, attributeType);

            if (attribute == null)
            {
                Debug.Log(item.Name + " 没有配置 MessageAttribute!");
            }
            else
            {
                MessageAttribute msgAttribute = attribute as MessageAttribute;
                _msg_route_map.Add(item.Name, msgAttribute.Route);
                _route_msg_map.Add(msgAttribute.Route, item.Name);
            }
        }
    }

    #region API

    private static GChannel _instance;

    internal static GChannel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GChannel();
            }
            return _instance;
        }
    }


    public Action<object> ConvertT<T>(Action<T> myActionT)
    {
        if (myActionT == null)
            return null;
        else return new Action<object>(o => myActionT((T)o));
    }

    /// <summary>
    /// 请求服务器 发送消息
    /// </summary>
    /// <typeparam name="T">服务器回复的消息类型ID</typeparam>
    /// <param name="message">消息数据</param>
    /// <param name="callback">网络回调</param>
    public void SendMessage<T>(JsonObject message, Action<T> callback) where T : Message
    {
        string messageName = typeof(T).Name;

        if (_msg_route_map.ContainsKey(messageName))
        {
            ICollection locker = _msg_handler;
            lock (locker.SyncRoot)
            {
                if (_msg_handler.ContainsKey(messageName))
                {
                    return;
                }
                _msg_handler.Add(messageName, ConvertT(callback));
            }


            if (message == null)
            {
                message = new JsonObject();
            }
            string route = _msg_route_map[messageName];
            _pomelo.request(route, message, (data) =>
            {
                Debug.Log("收到消息 : " + data);
                _recieveMsgQueue.Enqueue(data);
            });
            Debug.Log("发送消息: " + route + ":" + messageName + "->" + message);
        }
        else
        {
            throw new Exception("找不到" + messageName + "的请求路由！");
        }

        //string json_msg = SimpleJson.SimpleJson.SerializeObject(msg);
        //JsonObject obj_msg = IOTool.DeserializeObject<JsonObject>(json_msg);
    }

    /// <summary>
    /// 发送本地消息 模拟网络消息
    /// </summary>
    /// <param name="message">xxxFakeMessage</param>
    /// <param name="callback"></param>
    /// <param name="data">带外数据</param>
    public void SendLocalMessage(Action<object> callback, string mes = null, object data = null)
    {

        string __mid = string.IsNullOrEmpty(mes) ? ++LocalMsgId + "" : mes;

        JsonObject localMessage = new JsonObject();
        localMessage["__mid"] = __mid;
        localMessage["data"] = data;
        localMessage["__type"] = "local";

        bool isDuplicatedName = _msg_route_map.ContainsKey(__mid);

        if (isDuplicatedName)
        {
            throw new Exception("duplicated local message name with remote message name:" + __mid);
        }

        ICollection locker1 = _recieveMsgQueue;
        lock (locker1.SyncRoot)
        {
            _recieveMsgQueue.Enqueue(localMessage);
        }

        ICollection locker = _msg_handler;
        lock (locker.SyncRoot)
        {
            _msg_handler.Add(__mid, callback);
        }

        Debug.Log("发送本地消息: " + localMessage.ToString());
    }

    /// <summary>
    /// 获取消息，主线程Update中调用。
    /// </summary>
    /// <returns></returns>
    public void PumpMessage()
    {
        if (_recieveMsgQueue.Count <= 0)
        {
            return;
        }

        JsonObject msg = _recieveMsgQueue.Dequeue();

        string msgType = msg["__type"].ToString();// msg["__type"] == null ? "local" : msg["__type"].ToString();


        Action<object> callback = null;
        object msgObj = null;
        string mid = "";

        if (msgType.Equals("local"))
        {
            msgObj = msg["data"];
            mid = msg["__mid"].ToString();
        }
        else//push resp
        {
            mid = _route_msg_map[msg["__route"].ToString()];
            //msgObj = IOTool.DeserializeObject(msg.ToString());//, ClassUtil.GetType(mid));
            //msgObj = SimpleJson.SimpleJson.DeserializeObject(msg);
        }

        ICollection locker = _msg_handler;

        lock (locker.SyncRoot)
        {
            if (_msg_handler.ContainsKey(mid))
            {
                callback = _msg_handler[mid];
                _msg_handler.Remove(mid);
            }
        }

        if (callback != null)
        {
            if (msgType.Equals("local"))
            {
                callback(msgObj);
            }
            else
            {
                object obj = ClassUtil.LoadClass(mid, new object[] { msg });
                callback(obj);//msgObj);
            }
        }
        else
        {
            Debug.LogError(mid + " message callback is null!");
        }
    }

    #endregion

    private void OnNetWorkChanged(NetWorkState state)
    {
        Debug.Log("NetState:" + state);
        SendLocalMessage((data) =>
        {
            if (NetStateChangedEvent != null)
            {
                NetStateChangedEvent(state);
            }
        }, null, state);
    }

    /// <summary>
    /// 登录GateServer
    /// </summary>
    /// <param name="callback">登录成功的回调</param>
    public void ConnectGateServer(Action<object> callback)
    {
        Debug.Log("Connecting Gate server");
        if (_pomelo != null)
        {
            _pomelo.Dispose();
        }
        _pomelo = new PomeloClient();

        _pomelo.NetWorkStateChangedEvent += (state) =>
        {
            if (state == NetWorkState.ERROR || state == NetWorkState.TIMEOUT)
            {
                OnNetWorkChanged(state);
            }
        };

        _pomelo.initClient(_host, _port, () =>
        {
            _pomelo.connect(null, data =>
            {
                SendLocalMessage(args =>
                {
                    callback(args);
                }, "gate_connected_success", data);
            });
        });
    }

    public void Disconnect()
    {
        _msg_handler.Clear();
        _recieveMsgQueue.Clear();
        //NetStateChangedEvent = null;

        if (_pomelo != null)
        {
            _pomelo.Dispose();
        }
        _pomelo = null;
    }

    /// <summary>
    /// 连接connector服务器
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="callback"></param>
    public void ConnectConnectorServer(string host, int port, Action<object> callback)
    {
        Debug.Log("Connecting Connector server");
        if (_pomelo != null)
        {
            _pomelo.Dispose();
        }
        _pomelo = new PomeloClient();

        _pomelo.NetWorkStateChangedEvent += (state) =>
        {
            if (state == NetWorkState.ERROR || state == NetWorkState.TIMEOUT || state == NetWorkState.DISCONNECTED)
            {
                OnNetWorkChanged(state);
            }
        };

        _pomelo.initClient(host, port, () =>
        {
            _pomelo.connect(null, data =>
            {
                SendLocalMessage(args =>
                {
                    callback(args);
                }, "connector_connect_success", data);//,"connector_connenct"
            });
        }, true);

        InitPushEvent();
    }

    /// <summary>
    /// 初始化push的事件
    /// </summary>
    private void InitPushEvent()
    {
        _pomelo.on("onChat", (mes) =>
        {
            SendLocalMessage((arg) =>
            {
                if (OnChatEvent != null)
                {
                    OnChatEvent(mes);
                }
            }, null, mes);
        });

        _pomelo.on("onSysMailNum", (mes) =>
        {
            SendLocalMessage((arg) =>
            {
                if (OnSysMailNumEvent != null)
                {
                    OnSysMailNumEvent(mes);
                }
            }, null, mes);
        });

        _pomelo.on("onBroadcast", (mes) =>
        {
            SendLocalMessage((arg) =>
            {
                if (OnBroadcastEvent != null)
                {
                    OnBroadcastEvent(mes);
                }
            }, null, mes);
        });
    }

    /// <summary>
    /// notify server功能。
    /// </summary>
    /// <param name="route">server路由 "chat.chatHandler.send"</param>
    /// <param name="message"></param>
    public void Notify(string route, JsonObject message = null)
    {

        Debug.Log(string.Format("Notify: {0}{1}", route, message != null ? "," + message.ToString() : ""));

        if (message == null)
        {
            message = new JsonObject();
        }
        _pomelo.notify(route, message);
    }
}