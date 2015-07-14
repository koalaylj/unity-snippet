using UnityEngine;
using System.Collections;
using System.IO;
using System;


//0：第一次登陆(无账号)
//1：用户名密码
//2：游客账号
public enum AccountType
{
    NO_ACCOUNT = 0,
    USER_ACCOUNT = 1,
    GUEST_ACCOUNT = 2
};


/// <summary>
/// 存档管理类
/// </summary>
public static class PrefsManager
{

#if UNITY_STANDALONE || UNITY_EDITOR
    /// <summary>
    /// windows/mac/linux 下的存档。
    /// windows: %userprofile%/AppData/LocalLow/LazyFish/my_strawberry
    /// </summary>
    private readonly static string PREF_NAME = Path.Combine(Application.persistentDataPath, "princess.json");
#else
    /// <summary>
    /// 移动设备存档
    /// </summary>
    private readonly static string PREF_NAME = "com.game95.princess";
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
            //_prefs = IOTool.DeserializeObject<PrefsConfModel>(json);
            SimpleJson.JsonObject jsonObj = IOTool.DeserializeObject(json);
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
    
    public static int CurrentMission
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.CurrentMission = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1; 
            return _prefs.CurrentMission;
        }
    }
    

    public static int CurrentSeleBatPanel
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.CurrentSeleBatPanel = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1; 
            return _prefs.CurrentSeleBatPanel;
        }
    }


    public static int CurrentSeleQuest
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.CurrentSeleQuest = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1;
            return _prefs.CurrentSeleQuest;
        }
    }

    // 第一次玩游戏场景
    public static int FirstPlay
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.FirstPlay = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1;
            return _prefs.FirstPlay;
        }
    }
    

    public static int CurrentCreamMission
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.CurrentCreamMission = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1;
            return _prefs.CurrentCreamMission;
        }
    }
    public static int CurrentCreamSeleBatPanel
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.CurrentCreamSeleBatPanel = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1;
            return _prefs.CurrentCreamSeleBatPanel;
        }
    }
    public static int CurrentCreamSeleQuest
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.CurrentCreamSeleQuest = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1;
            return _prefs.CurrentCreamSeleQuest;
        }
    }


    public static string GuestName
    {
        set
        {
            _prefs.GuestName = value;
            SavePrefs();
        }
        get
        {
            return _prefs.GuestName;
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

    /// <summary>
    /// 密码
    /// </summary>
    public static string Password
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.Password = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return "";
            return _prefs.Password;
        }
    }
    
    public static int NewPlayerFight
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.NewPlayerFight = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 1;
            return _prefs.NewPlayerFight;
        }
    }


    public static AccountType AccountType
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.AccountType = (int)value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 0;
            return (AccountType)_prefs.AccountType;
        }
    }

    public static int ServerId
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.ServerId = value;
            SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 0;
            return _prefs.ServerId;
        }
    }

    public static int Skill1Index
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.Skill1Index = (int)value;
            //SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 0;
            return _prefs.Skill1Index;
        }
    }
    public static int Skill2Index
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.Skill2Index = (int)value;
            //SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 0;
            return _prefs.Skill2Index;
        }
    }
    public static int Skill3Index
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.Skill3Index = (int)value;
            //SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 0;
            return _prefs.Skill3Index;
        }
    }
    public static int Skill4Index
    {
        set
        {
            if (_prefs == null)
                return;
            _prefs.Skill4Index = (int)value;
            //SavePrefs();
        }
        get
        {
            if (_prefs == null)
                return 0;
            return _prefs.Skill4Index;
        }
    }

    private static bool _hasLogin = false;

    /// <summary>
    /// 角色是否登录
    /// </summary>
    public static bool HasLogin
    {
        get { return _hasLogin; }
        set { _hasLogin = value; }
    }
    
    /// <summary>
    /// 音乐音效开启
    /// </summary>
    public static bool SoundOpen
    {
        get { return _prefs.SoundOpen == 1; }
        set
        {
            _prefs.SoundOpen = value ? 1 : 0;
            SavePrefs();
        }
    }
    

    /******************************* 存档类 ******************************/

    /// <summary>
    /// 玩家存档
    /// </summary>
    private class PrefsConfModel
    {
        public PrefsConfModel() { Reset(); }
        public PrefsConfModel(SimpleJson.JsonObject obj)
        {
            object v = null;
            if (obj.TryGetValue("FirstPlay", out v)) this.FirstPlay = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("CurrentCreamMission", out v)) this.CurrentCreamMission = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("CurrentCreamSeleBatPanel", out v)) this.CurrentCreamSeleBatPanel = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("CurrentCreamSeleQuest", out v)) this.CurrentCreamSeleQuest = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("NewPlayerFight", out v)) this.NewPlayerFight = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("CurrentMission", out v)) this.CurrentMission = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("CurrentSeleBatPanel", out v)) this.CurrentSeleBatPanel = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("CurrentSeleQuest", out v)) this.CurrentSeleQuest = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("UserName", out v)) this.UserName = Convert.ToString(v); else Debug.LogError(" !");
            if (obj.TryGetValue("GuestName", out v)) this.GuestName = Convert.ToString(v); else Debug.LogError(" !");
            if (obj.TryGetValue("Password", out v)) this.Password = Convert.ToString(v); else Debug.LogError(" !");
            if (obj.TryGetValue("AccountType", out v)) this.AccountType = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("ServerId", out v)) this.ServerId = Convert.ToInt32(v); else Debug.LogError(" !");

            if (obj.TryGetValue("Skill1Index", out v)) this.Skill1Index = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("Skill2Index", out v)) this.Skill2Index = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("Skill3Index", out v)) this.Skill3Index = Convert.ToInt32(v); else Debug.LogError(" !");
            if (obj.TryGetValue("Skill4Index", out v)) this.Skill4Index = Convert.ToInt32(v); else Debug.LogError(" !");

            if (obj.TryGetValue("SoundOpen", out v)) this._soundOpen = Convert.ToInt32(v); else Debug.LogError(" !");
        }
              

        public string ToJson()
        {
            return "{" + string.Format("\"CurrentMission\":{0}, \"CurrentSeleBatPanel\":{1}, \"CurrentSeleQuest\":{2}, \"CurrentCreamMission\":{3}, \"CurrentCreamSeleBatPanel\":{4}, \"CurrentCreamSeleQuest\":{5}, \"UserName\":\"{6}\",\"Password\":\"{7}\",\"AccountType\":{8}, \"ServerId\":{9}, \"NewPlayerFight\":{10}, \"FirstPlay\":{11}, \"Skill1Index\":{12}, \"Skill2Index\":{13}, \"Skill3Index\":{14}, \"Skill4Index\":{15}, \"GuestName\":\"{16}\", \"SoundOpen\":{17} ",
                               _currentMission, _currentSelectedBattlePanel, _currentSelectedQuest, _currentCreamMission, _currentCreamSelectedBattlePanel, _currentCreamSelectedQuest,
                               _userName, _password, _accountType, _serverId, _newPlayerFight, _firstPlay, _skill1Index, _skill2Index, _skill3Index, _skill4Index, _guestName, _soundOpen) + "}";
        }

        public void Reset()
        {
            _currentMission = 1;
            _currentSelectedBattlePanel = 1;
            _currentSelectedQuest = 1;
            _currentCreamMission = 1;
            _currentCreamSelectedBattlePanel = 1;
            _currentCreamSelectedQuest = 1;
            _userName = "";
            _password = "";
            _accountType = 0;
            _serverId = -1;
            _newPlayerFight = 0;
            _firstPlay = 0;
            _skill1Index = 0;
            _skill2Index = 0;
            _skill3Index = 0;
            _skill4Index = 0;
            _soundOpen = 1;
        }

        private int _soundOpen = 1;
        public int SoundOpen
        {
            get { return _soundOpen; }
            set { _soundOpen = value; }
        }


        private int _skill1Index = 0;
        public int Skill1Index
        {
            get { return _skill1Index; }
            set { _skill1Index = value; }
        }
        private int _skill2Index = 0;
        public int Skill2Index
        {
            get { return _skill2Index; }
            set { _skill2Index = value; }
        }
        private int _skill3Index = 0;
        public int Skill3Index
        {
            get { return _skill3Index; }
            set { _skill3Index = value; }
        }
        private int _skill4Index = 0;
        public int Skill4Index
        {
            get { return _skill4Index; }
            set { _skill4Index = value; }
        }


        
        private int _firstPlay = 0;
        public int FirstPlay
        {
            get { return _firstPlay; }
            set { _firstPlay = value; }
        }

        private int _currentCreamMission = 0;
        /// <summary>
        /// 当前关卡
        /// </summary>
        public int CurrentCreamMission
        {
            get { return _currentCreamMission; }
            set { _currentCreamMission = value; }
        }

        private int _currentCreamSelectedBattlePanel = 1;
        /// <summary>
        /// 当前选择的关卡牌
        /// </summary>
        public int CurrentCreamSeleBatPanel
        {
            get { return _currentCreamSelectedBattlePanel; }
            set { _currentCreamSelectedBattlePanel = value; }
        }

        private int _currentCreamSelectedQuest = 1;
        /// <summary>
        /// 当前选择的小关卡
        /// </summary>
        public int CurrentCreamSeleQuest
        {
            get { return _currentCreamSelectedQuest; }
            set { _currentCreamSelectedQuest = value; }
        }

        private int _newPlayerFight = 0;
        /// <summary>
        /// 是否经过新手战斗帮助 0未 1已
        /// </summary>
        public int NewPlayerFight
        {
            get { return _newPlayerFight; }
            set { _newPlayerFight = value; }
        }

        private int _currentMission = 1;
        /// <summary>
        /// 当前关卡
        /// </summary>
        public int CurrentMission
        {
            get { return _currentMission; }
            set { _currentMission = value; }
        }

        private int _currentSelectedBattlePanel = 1;
        /// <summary>
        /// 当前选择的关卡牌
        /// </summary>
        public int CurrentSeleBatPanel
        {
            get { return _currentSelectedBattlePanel; }
            set { _currentSelectedBattlePanel = value; }
        }

        private int _currentSelectedQuest = 1;
        /// <summary>
        /// 当前选择的小关卡
        /// </summary>
        public int CurrentSeleQuest
        {
            get { return _currentSelectedQuest; }
            set { _currentSelectedQuest = value; }
        }


        private string _guestName = "";

        /// <summary>
        /// 用户名
        /// </summary>
        public string GuestName
        {
            get { return _guestName; }
            set { _guestName = value; }
        }


        private string _userName = "";

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password = "";

        /// <summary>
        /// 密码 md5
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private int _accountType = 0;

        /// <summary>
        /// 登陆模式：
        ///     0：第一次登陆(无账号)
        ///     1：用户名密码
        ///     2：游客账号
        /// </summary>
        public int AccountType
        {
            get { return _accountType; }
            set { _accountType = value; }
        }

        private int _serverId = -1;

        /// <summary>
        /// 上次登录的服id
        /// </summary>
        public int ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }
    }
}