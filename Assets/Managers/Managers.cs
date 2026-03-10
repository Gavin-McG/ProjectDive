using System;
using System.Collections.Generic;
using UnityEngine;

public static class Managers
{
    private static Dictionary<Type, MonoBehaviour> managers = new();

    public static void Add<T>(T manager) where T : MonoBehaviour
    {
        Type type = typeof(T);
        managers.Add(type, manager);
    }

    public static void Add(MonoBehaviour manager)
    {
        Type type = manager.GetType();
        managers.Add(type, manager);
    }
    
    public static T Get<T>() where T : MonoBehaviour
    {
        if (managers.ContainsKey(typeof(T)))
        {
            MonoBehaviour manager = managers[typeof(T)];
            if (manager is T tmanager)
            {
                return tmanager;
            }
        }
        return null;
    }

    public static MonoBehaviour Get(Type type)
    {
        if (managers.ContainsKey(type))
        {
            return managers[type];
        }
        return null;
    }
}
