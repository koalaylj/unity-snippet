using SimpleJson;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace Pomelo.DotNetClient
{
    /// <summary>
    /// 网络状态
    /// </summary>
    public enum NetWorkState
    {

        [Description("网络初始状态")]
        CLOSED,         //初始状态

        [Description("正在连接服务器")]
        CONNECTING,     //正在连接服务器

        [Description("已连接服务器")]
        CONNECTED,      // 正常联网状态

        [Description("与服务器断开连接")]
        DISCONNECTED,   // disconnect 与服务器断开

        [Description("无法连接服务器")]
        TIMEOUT,        // Connect超时

        [Description("网络发生错误")]
        ERROR           // socket exception
    }

    public class PomeloClient : IDisposable
    {
        /// <summary>
        /// 网络状态变化的事件
        /// </summary>
        public event Action<NetWorkState> NetWorkStateChangedEvent;

        //当前网络状态
        private NetWorkState _netWorkState = NetWorkState.CLOSED;
        private ManualResetEvent _timeout = new ManualResetEvent(false);
        private int _timeoutMSec = 5000;//超时时间 毫秒单位

        private EventManager eventManager;
        private Socket socket;
        private Protocol protocol;
        private bool disposed = false;
        private uint reqId = 1;

        public PomeloClient()
        {
        }

        IPAddress _ipAddress = null;
        int _port;

        /// <summary>
        /// 初始化客户端socket连接
        /// </summary>
        /// <param name="host">server name or server ip (支持www.xxx.com/127.0.0.1/::1/localhost等形式)</param>
        /// <param name="port">server port</param>
        public void initClient(string host, int port, Action callback = null,bool reqProto = false)
        {
            _timeout.Reset();
            eventManager = new EventManager();
            NetWorkChanged(NetWorkState.CONNECTING);

            _port = port;

            try
            {
                IPAddress[] addresses = Dns.GetHostEntry(host).AddressList;
                foreach (var item in addresses)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                    {
                        _ipAddress = item;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                NetWorkChanged(NetWorkState.ERROR);
                return;
            }


            //Ping p = new Ping();
            //string testData = "ko ni chi wa";
            //byte[] buffer = System.Text.Encoding.ASCII.GetBytes(testData);
            //PingOptions option = new PingOptions();
            //option.DontFragment = true;
            //PingReply reply = p.Send(ipAddress.ToString(),1000,buffer,option);

            //Log.Info("ping:{0},result:{1}",ipAddress.ToString(),reply.Status);

            if (_ipAddress == null)
            {
                throw new Exception("can not parse host : " + host);
            }

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ie = new IPEndPoint(_ipAddress, _port);

            socket.BeginConnect(ie, new AsyncCallback((result) =>
            {
                try
                {
                    this.socket.EndConnect(result);
                    this.protocol = new Protocol(this, this.socket, reqProto);
                    NetWorkChanged(NetWorkState.CONNECTED);

                    if (callback != null)
                    {
                        callback();
                    }
                }
                catch (SocketException e)
                {
                    UnityEngine.Debug.Log("e:" + e.ToString());
                    NetWorkChanged(NetWorkState.ERROR);
                    Dispose();
                }
                finally
                {
                    _timeout.Set();
                }
            }), this.socket);

            if (_timeout.WaitOne(_timeoutMSec, false))
            {
                if (_netWorkState != NetWorkState.CONNECTED)
                {
                    NetWorkChanged(NetWorkState.TIMEOUT);
                    Dispose();
                }
            }
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="state"></param>
        private void NetWorkChanged(NetWorkState state)
        {
            _netWorkState = state;

            if (NetWorkStateChangedEvent != null)
            {
                NetWorkStateChangedEvent(state);
            }
        }

        public void connect()
        {
            connect(null, null);
        }

        public void connect(JsonObject user)
        {
            connect(user, null);
        }

        public void connect(Action<JsonObject> handshakeCallback)
        {
            connect(null, handshakeCallback);
        }

        public bool connect(JsonObject user, Action<JsonObject> handshakeCallback)
        {
            try
            {
                protocol.start(user, handshakeCallback);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private JsonObject emptyMsg = new JsonObject();
        public void request(string route, Action<JsonObject> action)
        {
            this.request(route, emptyMsg, action);
        }

        public void request(string route, JsonObject msg, Action<JsonObject> action)
        {
            Log.Info("request->ip:{0},port:{1},route:{2}",_ipAddress.ToString(),_port,route);
            this.eventManager.AddCallBack(reqId, action);
            protocol.send(route, reqId, msg);

            reqId++;
        }

        public void notify(string route, JsonObject msg)
        {
            protocol.send(route, msg);
        }

        public void on(string eventName, Action<JsonObject> action)
        {
            eventManager.AddOnEvent(eventName, action);
        }

        internal void processMessage(Message msg)
        {
            if (msg.type == MessageType.MSG_RESPONSE)
            {
                msg.data["__route"] = msg.route;
                msg.data["__type"] = "resp";
                eventManager.InvokeCallBack(msg.id, msg.data);
            }
            else if (msg.type == MessageType.MSG_PUSH)
            {
                msg.data["__route"] = msg.route;
                msg.data["__type"] = "push";
                eventManager.InvokeOnEvent(msg.route, msg.data);
            }
        }

        public void disconnect()
        {
            Dispose();
            NetWorkChanged(NetWorkState.DISCONNECTED);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code 
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                // free managed resources
                if (this.protocol != null)
                {
                    this.protocol.close();
                }

                if (this.eventManager != null)
                {
                    this.eventManager.Dispose();
                }

                NetWorkStateChangedEvent = null;
                _timeout = null;

                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
                    this.socket = null;
                }
                catch (Exception)
                {
                    //todo : 有待确定这里是否会出现异常，这里是参考之前官方github上pull request。emptyMsg
                }

                this.disposed = true;
            }
        }
        /// <summary>
        /// 主线程调用，获取接受消息的更新
        /// </summary>
        //public void UpdateRevice()
        //{
        //    if (this.protocol != null)
        //    {
        //        this.protocol.Update();
        //    }
        //}
    }
}