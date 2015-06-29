using UnityEngine;
using System.Collections;
using System;
using SimpleJson;

/// <summary>
/// 用户登录
/// </summary>
[MessageAttribute("connector.connectorHandler.userLogin")]
public class UserLoginMessage : Message
{
}

/// <summary>
/// 加载玩家数据消息
/// </summary>
[MessageAttribute("hall.hallHandler.loadPlayer")]
public class LoadPlayerMessage : Message
{
    public UserDataMessage data { get; set; }
}