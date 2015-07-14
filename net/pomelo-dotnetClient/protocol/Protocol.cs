using System;
using SimpleJson;
using System.Text;
using System.Security.Cryptography;

namespace Pomelo.DotNetClient
{
    public class Protocol
    {
        private MessageProtocol messageProtocol;
        private ProtocolState state;
        private Transporter transporter;
        private HandShakeService handshake;
        private HeartBeatService heartBeatService = null;
        private PomeloClient pc;


        public const string Version = "0.3.0";
        public const string Type = "unity-socket";


        private string _clientProto;
        private string _serverProto;
        private string _dict;

        private bool _reqProto = false;

        public PomeloClient getPomeloClient()
        {
            return this.pc;
        }

        public Protocol(PomeloClient pc, System.Net.Sockets.Socket socket, bool reqProto)
        {
            _reqProto = reqProto;

            this.pc = pc;
            this.transporter = new Transporter(socket, this.processMessage);
            this.transporter.onDisconnect = onDisconnect;

            this.handshake = new HandShakeService(this);
            this.state = ProtocolState.start;
        }

        internal void start(JsonObject user, Action<JsonObject> callback)
        {
            this.transporter.start();

            if (user == null)
                user = new JsonObject();

            JsonObject sys = new JsonObject();

            if (_reqProto)
            {
                _clientProto = IOTool.LoadJson(PARAM.JSON_PROTO_CLIENT);
                _serverProto = IOTool.LoadJson(PARAM.JSON_PROTO_SERVER);
                _dict = IOTool.LoadJson(PARAM.JSON_PROTO_DICT);

                using (MD5 md5Hash = MD5.Create())
                {
                    string proto = SimpleJson.SimpleJson.Minify(_clientProto + _serverProto);
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(proto));
                    sys["protoVersion"] = Convert.ToBase64String(data);

                    data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(_dict));
                    sys["dictVersion"] = Convert.ToBase64String(data);
                }
            }

            sys["version"] = Version;
            sys["type"] = Type;


            //Build handshake message
            JsonObject msg = new JsonObject();
            msg["sys"] = sys;
            msg["user"] = user;

            this.handshake.request(msg, callback);

            this.state = ProtocolState.handshaking;
        }

        //Send notify, do not need id
        internal void send(string route, JsonObject msg)
        {
            send(route, 0, msg);
        }

        //Send request, user request id 
        internal void send(string route, uint id, JsonObject msg)
        {
            if (this.state != ProtocolState.working)
                return;

            byte[] body = messageProtocol.encode(route, id, msg);

            send(PackageType.PKG_DATA, body);
        }

        internal void send(PackageType type)
        {
            if (this.state == ProtocolState.closed)
                return;

            transporter.send(PackageProtocol.encode(type, null));
        }

        //Send system message, these message do not use messageProtocol
        //internal void send(PackageType type, JsonObject msg){
        //    //This method only used to send system package
        //    if(type == PackageType .PKG_DATA) return;

        //    byte[] body = Encoding.UTF8.GetBytes(msg.ToString());

        //    send (type, body);
        //}

        //Send message use the transporter
        internal void send(PackageType type, byte[] body)
        {

            if (this.state == ProtocolState.closed) return;

            byte[] pkg = PackageProtocol.encode(type, body);
            transporter.send(pkg);

            //pkg = PackageProtocol.encode(type, body);
            //transporter.send(pkg);

            //pkg = PackageProtocol.encode(type, body);
            //transporter.send(pkg);

            //pkg = PackageProtocol.encode(type, body);
            //transporter.send(pkg);
        }

        //Invoke by Transporter, process the message
        internal void processMessage(byte[] bytes)
        {
            Package pkg = PackageProtocol.decode(bytes);

            //Ignore all the message except handshading at handshake stage
            if (pkg.type == PackageType.PKG_HANDSHAKE && this.state == ProtocolState.handshaking)
            {

                //Ignore all the message except handshading
                JsonObject data = IOTool.DeserializeObject(Encoding.UTF8.GetString(pkg.body));

                processHandshakeData(data);

                this.state = ProtocolState.working;

            }
            else if (pkg.type == PackageType.PKG_HEARTBEAT && this.state == ProtocolState.working)
            {
                this.heartBeatService.resetTimeout();
            }
            else if (pkg.type == PackageType.PKG_DATA && this.state == ProtocolState.working)
            {
                this.heartBeatService.resetTimeout();
                pc.processMessage(messageProtocol.decode(pkg.body));
            }
            else if (pkg.type == PackageType.PKG_KICK)
            {
                this.getPomeloClient().disconnect();
                this.close();
            }
        }


        private void processHandshakeData(JsonObject msg)
        {
            //Handshake error
            if (!msg.ContainsKey("code") || !msg.ContainsKey("sys") || Convert.ToInt32(msg["code"]) != 200)
            {
                throw new Exception("Handshake error! Please check your handshake config.");
            }

            //Set compress data
            JsonObject sys = (JsonObject)msg["sys"];

            JsonObject dict = new JsonObject();
            if (sys.ContainsKey("dict"))
            {
                dict = (JsonObject)sys["dict"];
                if (_reqProto)
                {
                    IOTool.SaveJson(PARAM.JSON_PROTO_DICT, dict.ToString());
                }
            }
            else
            {
                if (_reqProto)
                {
                    dict = IOTool.DeserializeObject(_dict);
                }
            }

            JsonObject protos = new JsonObject();
            JsonObject serverProtos = new JsonObject();
            JsonObject clientProtos = new JsonObject();

            if (sys.ContainsKey("protos"))
            {
                protos = (JsonObject)sys["protos"];
                serverProtos = (JsonObject)protos["server"];
                clientProtos = (JsonObject)protos["client"];

                if (_reqProto)
                {
                    IOTool.SaveJson(PARAM.JSON_PROTO_SERVER, serverProtos.ToString());
                    IOTool.SaveJson(PARAM.JSON_PROTO_CLIENT, clientProtos.ToString());
                }
            }
            else
            {
                if (_reqProto)
                {
                    serverProtos = IOTool.DeserializeObject(_serverProto);
                    clientProtos = IOTool.DeserializeObject(_clientProto);
                }
            }

            messageProtocol = new MessageProtocol(dict, serverProtos, clientProtos);

            //Init heartbeat service
            int interval = 0;
            if (sys.ContainsKey("heartbeat"))
                interval = Convert.ToInt32(sys["heartbeat"]);
            heartBeatService = new HeartBeatService(interval, this);

            if (interval > 0)
            {
                heartBeatService.start();
            }

            //send ack and change protocol state
            handshake.ack();
            this.state = ProtocolState.working;

            //Invoke handshake callback
            JsonObject user = new JsonObject();
            if (msg.ContainsKey("user")) user = (JsonObject)msg["user"];
            handshake.invokeCallback(user);
        }

        //The socket disconnect
        private void onDisconnect()
        {
            this.pc.disconnect();
        }

        internal void close()
        {
            transporter.close();

            if (heartBeatService != null)
                heartBeatService.stop();

            this.state = ProtocolState.closed;
        }
        //internal void Update()
        //{
        //transporter.Update();
        //}
    }
}

