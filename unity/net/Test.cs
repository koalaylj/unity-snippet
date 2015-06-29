using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJson;
using System.Collections;

public class Test
{
	/// <summary>
	/// 选服
	/// </summary>
	/// <param name="callback"></param>
	private void SelectServer(int serverId, Action<bool> callback)
	{
		JsonObject mes_json = new JsonObject();
		mes_json["serverId"] = serverId;
		GChannel.Instance.SendMessage<SelectServerMessage>(mes_json, (mes) =>
		{
			_serverConnected = mes.Success;

			if (mes.Success)
			{
				GChannel.Instance.Disconnect();

				GChannel.Instance.ConnectConnectorServer(mes.ip, mes.port, (data) =>
				{
					callback(true);
				});
			}
			else
			{
				callback(false);
			}
		});
	}
}