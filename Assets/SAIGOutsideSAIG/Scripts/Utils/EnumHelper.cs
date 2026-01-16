using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class EnumHelper
{

    private static class EnumCache<T> where T : Enum
    {
        public static readonly T[] Values = (T[])Enum.GetValues(typeof(T));
        public static readonly int Count = Values.Length;
    }

    public static T GetRandom<T>() where T : Enum
    {
        var values = EnumCache<T>.Values; 
        int randomIndex = Random.Range(0, values.Length);
        return values[randomIndex];
    }


    public static T Next<T>(T src) where T : Enum
    {
        var values = EnumCache<T>.Values;
        int j = Array.IndexOf(values, src) + 1;
        return (values.Length == j) ? values[0] : values[j];
    }

    public static T Previous<T>(T src) where T : Enum
    {
        var values = EnumCache<T>.Values;
        int j = Array.IndexOf(values, src) - 1;
        return (j < 0) ? values[values.Length - 1] : values[j];
    }

    public static int Count<T>() where T : Enum
    {
        return EnumCache<T>.Count;
    }

    public static T Parse<T>(string value, bool ignoreCase = true) where T : Enum
    {
        try
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
        catch (Exception)
        {
            Debug.LogWarning($"[EnumHelper] Failed to parse '{value}' for {typeof(T)}. Defaulting.");
            return default(T);
        }
    }
}