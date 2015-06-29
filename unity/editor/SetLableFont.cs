using UnityEngine;
using System.Collections;
using UnityEditor;
/// <summary>
/// 解决NGUI UILabel字体丢失问题
/// 使用方法：
///     1：把这个类放到Editor目录下
///     2：在Unity的Hierarchy窗口中 选中(可多选)丢失字体的界面(必须是激活状态的)
///     3：从菜单打开咱的Editor，设置好字体后点按钮就行了，就能把选中的界面下所有的UILabel字体都设置了。
/// </summary>
public class LableFont : EditorWindow
{
    [MenuItem("GAME95/NGUI-SetLableFont")]
    static void CreateWindow()
    {
        EditorWindow.GetWindowWithRect<LableFont>(new Rect(400, Screen.width - 380, 300, 360));
    }

    public Font source;

    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();

        source = EditorGUILayout.ObjectField(source, typeof(Font), true) as Font;

        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("设置 Label 字体"))
        {
            if (source == null)
            {
                ShowNotification(new GUIContent("先选 Font！"));
            }
            else
            {
                Transform[] trans = Selection.transforms;

                foreach (var item in trans)
                {
                    Debug.Log(item.name);

                    UILabel[] labels = item.GetComponents<UILabel>();
                    foreach (var label in labels)
                    {
                        label.trueTypeFont = source;
                        Debug.Log("Lable:" + label.name);
                    }

                    labels = item.GetComponentsInChildren<UILabel>();
                    foreach (var label in labels)
                    {
                        label.trueTypeFont = source;
                        Debug.Log("Lable:" + label.name);
                    }
                }
            }
        }
    }
}