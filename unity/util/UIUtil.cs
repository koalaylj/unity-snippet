using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class UIUtil
{
    /// <summary>
    /// 按添加类型添加子对象
    /// </summary>
    public static GameObject AppendChild(GameObject parent, Object child)
    {
        if (child == null)
            return null;

        if (parent == null)
        {
            return GameObject.Instantiate(child) as GameObject;
        }

        GameObject go = GameObject.Instantiate(child) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }

    static public GameObject AppendChild(Transform parent, GameObject child)
    {
        return AppendChild(parent.gameObject, child);
    }

    public static GameObject AppendChild(GameObject parent, string prefab)
    {
        if (string.IsNullOrEmpty(prefab))
            return null;

        GameObject child = Resources.Load("UI/Prefabs/People/" + prefab) as GameObject;// todo unload assets

        return AppendChild(parent, child);
    }

    /// <summary>
    /// 释放当前transtrom所有子物体
    /// </summary>
    /// <param name="parent"></param>
    public static void DestoryTransformChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.DestroyObject(child.gameObject);
        }
    }

    public static int IsValidPlayerName(string userName)
    {
        if (string.IsNullOrEmpty(userName)
        || userName.Length < 2
        || userName.Length > 10)
        {
            return CODE.ERR_C_PLAYERNAME_LEN_INVALIDE;
        }

        //Regex regex = new Regex(@"[a-zA-Z0-9]+");
        // if (!string.IsNullOrEmpty(userName) && regex.IsMatch(userName))
        //  {
        return 1;
        //   }

        //    return CODE.ERR_C_USERNAME_CHAR_INVALIDE;
    }

    public static int IsValidUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName)
        || userName.Length < 6
        || userName.Length > 22)
        {
            return CODE.ERR_C_USERNAME_LEN_INVALIDE;
        }

        Regex regex = new Regex(@"[a-zA-Z0-9]+");
        if (!string.IsNullOrEmpty(userName) && regex.IsMatch(userName))
        {
            return 1;
        }

        return CODE.ERR_C_USERNAME_CHAR_INVALIDE;
    }


    public static int IsValidPassword(string pwd)
    {
        if (string.IsNullOrEmpty(pwd)
        || pwd.Length < 6
        || pwd.Length > 22)
        {
            return CODE.ERR_C_PASSWORD_LEN_INVALIDE;
        }

        Regex regex = new Regex(@"[a-zA-Z0-9_]+");
        if (regex.IsMatch(pwd))
        {
            return 1;
        }

        return CODE.ERR_C_PASSWORD_CHAR_INVALIDE;
    }

    /// <summary>
    /// 循环设置层
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    public static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }
}