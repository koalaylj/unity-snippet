using UnityEngine;
using UnityEditor;
/// <summary>
/// 常用开工具工具集合
/// </summary>
public class GameTools : EditorWindow {
    /// <summary>
    /// 秒计数
    /// </summary>
	int timecount = 0;
    /// <summary>
    /// 保存间隔时间（秒）
    /// </summary>
	public int timeInterval = 1800;
   /// <summary>
   /// 是否启用自动保存
   /// </summary>
	bool posGroupEnabled;
    /// <summary>
    /// 窗体初始化
    /// </summary>
	[MenuItem ("Window/Game Tools")]
	static void Init () {
		GameTools window = (GameTools)EditorWindow.GetWindow (typeof (GameTools));
	}
	void Start()
	{
	}
    /// <summary>
    /// 更新时间
    /// </summary>
	void Update()
	{
       if (posGroupEnabled)
	    {
            timecount++;
            if ( timecount / 100 >= timeInterval)
            {
               timecount = 0;
			    autoSave(); 
            }
	    } 
	}
    /// <summary>
    /// 自动保存资源场景
    /// </summary>
	void autoSave()
	{
		Debug.Log("开始自动保存");
		this.ShowNotification(new GUIContent("开始自动保存"));
		EditorApplication.SaveAssets();
		EditorApplication.SaveScene();
	}
    /// <summary>
    /// 功能绘图
    /// </summary>
	void OnGUI () {
		GUILayout.Label("-------编辑器状态-------");
		GUILayout.Label(string.Format("编译状态 : {0}",EditorApplication.isCompiling ? "isCompling" : "Complied"));
		GUILayout.Label("------场景资源管理------");
		posGroupEnabled = EditorGUILayout.BeginToggleGroup("自动保存", posGroupEnabled);
		timeInterval = EditorGUILayout.IntField("自动保存时间间隔（秒）", timeInterval);
		EditorGUILayout.EndToggleGroup();
		if (GUILayout.Button("新场景")) 
		{
            EditorApplication.SaveScene();
			EditorApplication.NewScene();
			EditorApplication.SaveAssets();
		}
		if (GUILayout.Button("保存场景")) 
		{
			EditorApplication.SaveScene();
		}
		if (GUILayout.Button("保存序列化资源")) 
		{		
			EditorApplication.SaveAssets();
		}
		if (GUILayout.Button("新建组件制作场景")) 
		{		
			EditorApplication.SaveScene();
			EditorApplication.NewScene();
			EditorApplication.SaveAssets();
			UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Simple2D);
		}
		GUILayout.Label("------常用NGUI功能------");
		if (GUILayout.Button("创建2D UI")) 
		{		
			UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Simple2D);
		}
	}
}