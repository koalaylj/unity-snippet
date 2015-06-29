using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class MessageAttribute : Attribute {

    public string Route { get; set; }

    public MessageAttribute(string route)
    {
        this.Route = route;
    }
}


public class Message
{
    public Message(SimpleJson.JsonObject obj)
    {
        if (obj == null) { return; }
        object v;
        if (obj.TryGetValue("code", out v)) this.code = Convert.ToInt32(v); else Debug.LogError(" ! ");
    }

    public int code { get; set; }

    public bool Success
    {
        get
        {
            return code == CODE.SUC_OK;
        }
    }


    ///// <summary>
    ///// 转化 SimpleJson.JsonArray 为特定类型的 List
    ///// </summary>
    //public static List<T> ConvertList<T>(SimpleJson.JsonArray array)
    //{
    //    return array.ConvertAll<T>(new System.Converter<object, T>(ConvertTo<T>));
    //}

    //private static T ConvertTo<T>(object obj)
    //{
    //    return SimpleJson.SimpleJson.DeserializeObject<T>(obj.ToString());
    //}


    //public override string ToString()
    //{
    //    StringBuilder sb = new StringBuilder();
    //    foreach (System.Reflection.PropertyInfo property in this.GetType().GetProperties())
    //    {

    //        sb.Append(property.Name);
    //        sb.Append(": ");
    //        if (property.GetIndexParameters().Length > 0)
    //        {
    //            sb.Append("Indexed Property cannot be used");
    //        }
    //        else
    //        {
    //            sb.Append(property.GetValue(this, null));
    //        }

    //        sb.Append(System.Environment.NewLine);
    //    }

    //    return sb.ToString();
    //}
}