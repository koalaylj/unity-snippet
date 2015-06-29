using UnityEngine;
using System.Collections;
using UnityEditor;
/// <summary>
/// 解决NGUI UILabel字体丢失问题
/// 使用方法：
///     1：把这个类放到Editor目录下
///     2：在Unity的Hierarchy窗口中 选中(可多选)丢失图集的界面(必须是激活状态的)
///     3：从菜单打开咱的Editor，设置好图集后点按钮就行了，就能把选中的界面下所有的UISprite的图集都设置了。
/// </summary>
public class UISpriteAtlas : EditorWindow
{
    [MenuItem("GAME95/NGUI-SetUISpriteAtlas")]
    static void CreateWindow()
    {
        EditorWindow.GetWindowWithRect<UISpriteAtlas>(new Rect(400, Screen.width - 380, 300, 360));
    }

    public UIAtlas source;

    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();

        source = EditorGUILayout.ObjectField(source, typeof(UIAtlas),true) as UIAtlas;

        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("设置 Atlas "))
        {
            if (source == null)
            {
                ShowNotification(new GUIContent("傻逼!\n先选 Atlas！"));
            }
            else
            {
                Transform[] trans = Selection.transforms;

                foreach (var item in trans)
                {
                    Debug.Log(item.name);

                    UISprite[] sprites = item.GetComponents<UISprite>();
                    foreach (var sprite in sprites)
                    {
                        sprite.atlas = source;
                        Debug.Log("atlas:" + sprite.name);
                    }

                    sprites = item.GetComponentsInChildren<UISprite>();
                    foreach (var sprite in sprites)
                    {
                        sprite.atlas = source;
                        Debug.Log("atlas:" + sprite.name);
                    }
                }
            }
        }
    }
}