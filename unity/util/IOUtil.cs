//#define _UNIT_TEST

using UnityEngine;
using System.IO;

public class IOUtil
{
    /// <summary>
    /// 配置文件存放的目录
    /// </summary>
    private static string _persitDir = System.IO.Path.Combine(Application.persistentDataPath, "config/");

    private static string _defaultDir = Application.streamingAssetsPath;

    /// <summary>
    /// 加载json
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string LoadJson(string fileName)
    {

        string json = "";

#if _UNIT_TEST
        var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "../../../Assets/StreamingAssets/" + fileName);//System.IO.Path.Combine(@"E:\95\Client3D\CatchThePrincess\trunk\Assets\StreamingAssets", fileName);
#else
        var filePath = System.IO.Path.Combine(_persitDir, Path.GetFileName(fileName));

        if (!File.Exists(filePath))
        {
            filePath = System.IO.Path.Combine(_defaultDir, fileName);
        }
#endif

        json = ReadAllText(filePath);

        return json;
    }

    public static void SaveJson(string fileName, string json)
    {
#if _UNIT_TEST
        var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "../../../Assets/StreamingAssets/" + fileName);//System.IO.Path.Combine(@"E:\95\Client3D\CatchThePrincess\trunk\Assets\StreamingAssets", fileName);
#else

        var filePath = System.IO.Path.Combine(_persitDir, Path.GetFileName(fileName));
#endif

        WriteText(filePath, json);
    }

    /// <summary>
    /// 读取文本文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string ReadAllText(string filePath)
    {
        Debug.Log("load file:" + filePath);

        string text = "";

#if _UNIT_TEST
        Log.Info("Loading file : " + filePath);
        text = System.IO.File.ReadAllText(filePath);
#else
        if (!File.Exists(filePath))
        {
            Log.Error("can not found file : " + filePath);
        }
        else
        {
            if (filePath.Contains("://"))
            {
                var www = new WWW(filePath);
                while (!www.isDone) { }
                text = www.text;
            }
            else
                text = System.IO.File.ReadAllText(filePath);
        }

#endif

        return text;
    }

    /// <summary>
    /// 写文本文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="text"></param>
    public static void WriteText(string path, string text)
    {
        DeleteFile(path);

        Debug.Log("write file:" + path);
        File.WriteAllText(path, text);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteFile(string path)
    {
        Debug.Log("delete file:" + path + "," + File.Exists(path));

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    //public static List<T> DeserializeList<T>(string json)
    //{
    //    return Message.ConvertList<T>((SimpleJson.JsonArray)DeserializeObject(json, typeof(string)));
    //}

    public static SimpleJson.JsonObject DeserializeObject(string json)
    {
        return (SimpleJson.JsonObject)SimpleJson.SimpleJson.DeserializeObject(json);
    }

    public static T DeserializeObject<T>(string json)
    {
        T re = SimpleJson.SimpleJson.DeserializeObject<T>(json);
        if (re == null)
            Debug.LogError("数据解析错误");
        return re;
    }

    //public static object DeserializeObject(string json, Type type)
    //{
    //    object re = SimpleJson.SimpleJson.DeserializeObject(json, type, null);
    //    if (re == null)
    //        Debug.LogError("数据解析错误");
    //    return re;
    //}

    /// <summary>
    /// 从磁盘加载预制
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public static GameObject LoadGameObject(string prefabName)
    {
        Log.Info("Loading prefab:" + prefabName);
        UnityEngine.Object prefab = Resources.Load(prefabName);
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        //Resources.UnloadUnusedAssets();
        return go;
    }
}
