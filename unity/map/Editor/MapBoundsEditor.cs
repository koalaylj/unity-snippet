using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System;

[CustomEditor(typeof(MapBounds))]
public class MapBoundsEditor : Editor {

    void OnSceneGUI() {

        MapBounds tar = (MapBounds)target;

        Transform bounds_root = tar.transform.parent;
        int bounds_points_count = bounds_root.childCount;

        //绘制scene中显示的效果
        List<Transform> bounds_points_list = new List<Transform>();
        Transform bounds_points_trans = null;
        Vector3 v_label_offset = new Vector3(0.1f, 0, 0.1f);
        for (int i = 0; i < bounds_points_count; i++) {
            bounds_points_trans = bounds_root.GetChild(i);
            bounds_points_list.Add(bounds_points_trans);
            Handles.color = Color.green;
            Handles.SphereCap(i, bounds_points_trans.position, Quaternion.identity, 0.2f);
            Vector3 pos = Handles.FreeMoveHandle(tar.transform.position,
                               Quaternion.identity,
                               0.2f,
                               Vector3.zero,
                               Handles.CircleCap);
            tar.transform.position = new Vector3(pos.x, tar.transform.position.y, pos.z);
            Handles.color = Color.red;
            Handles.Label(bounds_points_trans.position + v_label_offset, bounds_points_trans.name);
        }

        //按照字符串排序
        var bounds_points_sorted =
        from point in bounds_points_list
        orderby point.name ascending
        select point;

        List<Vector3> v_bounds_points = new List<Vector3>();

        foreach (var item in bounds_points_sorted) {
            v_bounds_points.Add(item.position);
        }

        if (bounds_points_count > 1) {
            Handles.color = Color.yellow;
            Handles.DrawPolyLine(v_bounds_points.ToArray());
            Handles.DrawLine(v_bounds_points[0], v_bounds_points[v_bounds_points.Count - 1]);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    private bool filter() {

        return true;
    }

}