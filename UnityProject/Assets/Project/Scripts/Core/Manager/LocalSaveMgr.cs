using UnityEngine;
using System;
using System.Text;
using System.Linq;

public static class LocalSaveMgr
{
    private static string Encrypt(string plainText)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(bytes);
    }

    private static string Decrypt(string encodedText)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(encodedText);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            Debug.LogWarning("복호화 실패. 기본값을 반환합니다.");
            return "";
        }
    }

    public static void SaveInt(string key, int value)
    {
        SaveString(key, value.ToString());
    }

    public static int LoadInt(string key, int defaultValue = 0)
    {
        string decrypted = LoadString(key);
        if (int.TryParse(decrypted, out int result))
            return result;
        return defaultValue;
    }

    public static void SaveFloat(string key, float value)
    {
        SaveString(key, value.ToString());
    }

    public static float LoadFloat(string key, float defaultValue = 0f)
    {
        string decrypted = LoadString(key);
        if (float.TryParse(decrypted, out float result))
            return result;
        return defaultValue;
    }

    // int 배열 저장
    public static void SaveIntArray(string key, int[] array)
    {
        if (array == null || array.Length == 0)
        {
            SaveString(key, "");
            return;
        }

        string arrayString = string.Join(",", array.Select(x => x.ToString()).ToArray());
        SaveString(key, arrayString);
    }

    // int 배열 로드
    public static int[] LoadIntArray(string key, int[] defaultValue = null)
    {
        string decrypted = LoadString(key);

        if (string.IsNullOrEmpty(decrypted))
            return defaultValue ?? new int[0];

        try
        {
            return decrypted.Split(',')
                           .Select(x => int.Parse(x.Trim()))
                           .ToArray();
        }
        catch
        {
            Debug.LogWarning($"int 배열 파싱 실패: {key}. 기본값을 반환합니다.");
            return defaultValue ?? new int[0];
        }
    }

    public static void SaveString(string key, string value)
    {
        string encrypted = Encrypt(value);
        PlayerPrefs.SetString(key, encrypted);
        PlayerPrefs.Save();
    }

    public static string LoadString(string key, string defaultValue = "")
    {
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        string encrypted = PlayerPrefs.GetString(key);
        string decrypted = Decrypt(encrypted);
        return string.IsNullOrEmpty(decrypted) ? defaultValue : decrypted;
    }

    public static DateTime LoadTime(string key, DateTime? defaultValue = null)
    {
        string timeString = LoadString(key);

        if (string.IsNullOrEmpty(timeString))
        {
            return defaultValue ?? DateTime.Now;
        }

        try
        {
            // Ticks 방식으로 저장된 경우
            if (long.TryParse(timeString, out long ticks))
            {
                return new DateTime(ticks);
            }

            // ISO 8601 문자열 형식으로 저장된 경우
            return DateTime.Parse(timeString);
        }
        catch
        {
            Debug.LogWarning($"DateTime 파싱 실패: {key}. 기본값을 반환합니다.");
            return defaultValue ?? DateTime.Now;
        }
    }

    public static void SaveTime(string key, DateTime value)
    {
        // Ticks 방식 (더 정확하고 안전함)
        string timeString = value.Ticks.ToString();
        SaveString(key, timeString);
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}