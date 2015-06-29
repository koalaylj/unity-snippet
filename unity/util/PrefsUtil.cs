using UnityEngine;
using System.Collections;
using System.IO;
using System;


/// <summary>
/// 存档管理类
/// </summary>
public static class PrefsUtil
{

#if UNITY_STANDALONE || UNITY_EDITOR
    /// <summary>
    /// windows/mac/linux 下的存档。
    /// windows: %userprofile%/AppData/LocalLow/LazyFish/my_strawberry
    /// </summary>
    private readonly static string PREF_NAME = Path.Combine(Application.persistentDataPath, "prefs.json");
#else
    /// <summary>
    /// 移动设备存档
    /// </summary>
    private readonly static string PREF_NAME = "com.fbi.me";
#endif

    private static PrefsConfModel _prefs = null;

    /// <summary>
    /// 保存文本形式的存档(json形式的文本比较方便)  Todo
    /// Windows 存档位置注册表的 HKCU\Software\[company name]\[product name]键下(这里company和product名是在Project Setting中设置的.)
    /// </summary>
    private static void SavePrefs()
    {
        //Debug.Log("存储存档: " + _prefs);

        string text = _prefs.ToJson();// JsonMapper.ToJson(_prefs);
#if UNITY_STANDALONE || UNITY_EDITOR
        File.WriteAllText(PREF_NAME, text, System.Text.Encoding.UTF8);
#else
        PlayerPrefs.SetString(PREF_NAME, text);
        PlayerPrefs.Save();
#endif

     //   Debug.Log("存档已保存:" + PREF_NAME);

    }


    /***********************************  API  **************************************/

    /// <summary>
    /// 加载存档
    /// </summary>
    public static void LoadPrefs()
    {
        Debug.Log("加载存档:" + PREF_NAME + " " + _prefs);

        string json = "";

#if UNITY_STANDALONE || UNITY_EDITOR
        if (File.Exists(PREF_NAME))
        {
            json = File.ReadAllText(PREF_NAME, System.Text.Encoding.UTF8);
        }
#else
        json = PlayerPrefs.GetString(PREF_NAME, "");
#endif

        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("初始化存档....");
            _prefs = new PrefsConfModel();
            SavePrefs();
        }
        else
        {
            _prefs = IOUtil.DeserializeObject<PrefsConfModel>(json);
            if (jsonObj != null)
            {
                _prefs = new PrefsConfModel(jsonObj);
            }
            else
            {
                Debug.Log("json存档错误");
                if (File.Exists(PREF_NAME))
                {
                    File.Delete(PREF_NAME);
                }

                Debug.Log("初始化存档....");
                _prefs = new PrefsConfModel();
                SavePrefs();
            }

            if (PrefsManager.SoundOpen)
            {
                AudioManager.Instance.SetMusicActive(true);
                AudioManager.Instance.SetSoundActive(true);
            }
            else
            {
                AudioManager.Instance.SetMusicActive(false);
                AudioManager.Instance.SetSoundActive(false);
            }
        }
    }

    /// <summary>
    /// 注册的用户名
    /// </summary>
    public static string UserName
    {
        set
        {
            if (_prefs == null)
                return;
            if (_prefs.UserName != value)
            {
                // 清空现有数据
                _prefs.Reset();
            }
            _prefs.UserName = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return "";
            return _prefs.UserName;
        }
    }


    private class PrefsConfModel
    {
    }
}