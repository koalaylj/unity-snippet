using UnityEngine;
using System.Collections.Generic;
using System;

public class ConfigBase {

    private string _name;
    private Vector3 _position;
    private Vector3 _scale;
    private Quaternion _rotation;

	
	public void SetTransform(Transform trans) 
	{
		SetTransform(trans, true);
	}

    /// <summary>
    /// 只有这个值需要手动设置
    /// </summary>
    public void SetTransform(Transform trans,bool isLocal) {
        _name = trans.name;
        if (isLocal) {
            _position = trans.localPosition;
            _scale = trans.localScale;
            _rotation = trans.localRotation;
        }
        else {
            _position = trans.position;
            _scale = trans.lossyScale;
            _rotation = trans.rotation;
        }
    }

    public Vector3 GetPosition() {
        return _position;
    }

    public Vector3 GetScale() {
        return _scale;
    }

    public Quaternion GetRotation() {
        return _rotation;
    }

    public String Name { get { return _name; } set { _name = value; } }

    public List<double> Position {
        get { return Vector3ToList(_position); }
        set { _position = ListToVector3(value); }
    }

    public List<double> Rotation {
        get { return QuartToList(_rotation); }
        set { _rotation = ListToQuart(value); }
    }

    public List<double> Scale {
        get { return Vector3ToList(_scale); }
        set { _scale = ListToVector3(value); }
    }

    public List<double> Vector3ToList(Vector3 v) {
        return new List<double>() { v.x, v.y, v.z };
    }

    public Vector3 ListToVector3(List<double> data) {
        if (data == null || data.Count < 3) {
            throw new Exception("格式不符合Vector3");
        }
        return new Vector3((float)data[0], (float)data[1], (float)data[2]);
    }

    public Quaternion ListToQuart(List<double> data) {
        return new Quaternion((float)data[0], (float)data[1], (float)data[2], (float)data[3]);
    }

    public List<double> QuartToList(Quaternion q) {
        return new List<double>() { q.x, q.y, q.z, q.w };
    }
}