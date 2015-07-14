using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ClassUtil {
    public static object LoadClass(string cn) {
        System.Type type = System.Reflection.Assembly.GetExecutingAssembly().GetType(cn);
        return LoadClass(type);
    }

    public static object LoadClass(System.Type type) {
        return System.Activator.CreateInstance(type);
    }


    public static object LoadClass(string cn, object[] parameters)
    {
        System.Type type = System.Reflection.Assembly.GetExecutingAssembly().GetType(cn);
        return LoadClass(type, parameters);
    }

    public static object LoadClass(System.Type type, object[] parameters)
    {
        return System.Activator.CreateInstance(type, parameters);
    }

    public static Type GetType(string type) {
        return System.Reflection.Assembly.GetExecutingAssembly().GetType(type);
    }

    /// <summary>
    /// 获取当前Assembly中某各类的所有子类
    /// </summary>
    /// <param name="father"></param>
    /// <returns></returns>
    public static List<Type> GetSubClassesOf(Type father)
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        List<Type> classlist = new List<Type>();

        foreach (Type type in asm.GetTypes())
        {
            if (type.IsClass && type.IsSubclassOf(father))
                classlist.Add(type);
        }

        return classlist;
    }
}