using System;
using System.Text;
using SimpleJson;
using System.Net;
using System.Net.Sockets;

namespace Pomelo.DotNetClient
{
	public class HandShakeService
	{
		private Protocol protocol;
		private Action<JsonObject> callback;

		public HandShakeService (Protocol protocol)
		{
			this.protocol = protocol;
		}
		
		public void request(JsonObject user, Action<JsonObject> callback){
			byte[] body = Encoding.UTF8.GetBytes(user.ToString());

			protocol.send(PackageType.PKG_HANDSHAKE, body);

			this.callback = callback;
		}

		internal void invokeCallback(JsonObject data){
			//Invoke the handshake callback
			if(callback != null) callback.Invoke(data);
		}

		public void ack(){
			protocol.send(PackageType.PKG_HANDSHAKE_ACK, new byte[0]);
		}
	}
}