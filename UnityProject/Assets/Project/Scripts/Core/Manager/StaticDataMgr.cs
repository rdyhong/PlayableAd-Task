using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

/// <summary>
/// StaticData.json ·Îµå ¹× °ü¸® Å¬·¡½º.
/// Resources/Static/StaticData.json À» ·ÎµåÇÏ¿© system, locale, character µ¥ÀÌÅÍ¸¦ Á¦°øÇÑ´Ù.
/// </summary>
public class StaticDataMgr : Singleton<StaticDataMgr>
{
    private const string RESOURCES_FOLDER = "Static";
    private const string JSON_FILE_NAME = "StaticData";

#if UNITY_EDITOR
    private const string RESOURCES_PATH = "Assets/Resources/" + RESOURCES_FOLDER;
#endif

    public ELanguage CurrentLanguage { get; private set; } = ELanguage.en;

    // system: key -> (version -> value)
    private Dictionary<string, Dictionary<string, string>> _systemData = new();

    // locale: id -> (langCode -> text)
    private Dictionary<int, Dictionary<string, string>> _localeData = new();

    // character: id -> JObject (È®Àå¿ë)
    private Dictionary<int, JObject> _characterData = new();

    // ÇöÀç Å¬¶óÀÌ¾ðÆ® ¹öÀü (system µ¥ÀÌÅÍ¿¡¼­ ¹öÀüº° °ª Á¶È¸ ½Ã »ç¿ë)
    [SerializeField] private string currentVersion = "0.0.1";

    private bool _isLoaded = false;
    public bool IsLoaded => _isLoaded;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ·Îµå
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    /// <summary>
    /// Resources Æú´õ¿¡¼­ StaticData.json À» ·ÎµåÇÑ´Ù.
    /// </summary>
    public void LoadStaticData()
    {
        string resourcePath = $"{RESOURCES_FOLDER}/{JSON_FILE_NAME}";
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);

        if (jsonAsset == null)
        {
            Debug.LogError($"[StaticDataLoader] ¸®¼Ò½º ÆÄÀÏÀ» Ã£À» ¼ö ¾ø½À´Ï´Ù: {resourcePath}");
            return;
        }

        ParseAll(jsonAsset.text);
        _isLoaded = true;

        Debug.Log($"[StaticDataLoader] ·Îµå ¿Ï·á - system:{_systemData.Count}, locale:{_localeData.Count}, character:{_characterData.Count}");
    }

    /// <summary>
    /// JSON ¹®ÀÚ¿­À» Á÷Á¢ ÆÄ½ÌÇÑ´Ù. (À¥ µî ¿ÜºÎ ¼Ò½º¿ë)
    /// </summary>
    public void LoadFromJsonString(string json)
    {
        ParseAll(json);
        _isLoaded = true;
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ÆÄ½Ì
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    private void ParseAll(string json)
    {
        JObject root;
        try
        {
            root = JObject.Parse(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[StaticDataLoader] JSON ÆÄ½Ì ½ÇÆÐ: {e.Message}");
            return;
        }

        if (root["system"] is JObject systemObj)
            ParseSystemData(systemObj);

        if (root["locale"] is JObject localeObj)
            ParseLocaleData(localeObj);

        if (root["character"] is JObject characterObj)
            ParseCharacterData(characterObj);
    }

    private void ParseSystemData(JObject obj)
    {
        _systemData.Clear();

        foreach (var prop in obj.Properties())
        {
            string key = prop.Name;
            var versionDict = new Dictionary<string, string>();

            if (prop.Value is JObject versions)
            {
                foreach (var v in versions.Properties())
                {
                    versionDict[v.Name] = v.Value.ToString();
                }
            }

            _systemData[key] = versionDict;
        }
    }

    private void ParseLocaleData(JObject obj)
    {
        _localeData.Clear();

        foreach (var prop in obj.Properties())
        {
            if (!int.TryParse(prop.Name, out int id)) continue;

            var langDict = new Dictionary<string, string>();

            if (prop.Value is JObject langs)
            {
                foreach (var l in langs.Properties())
                {
                    langDict[l.Name] = l.Value.ToString();
                }
            }

            _localeData[id] = langDict;
        }
    }

    private void ParseCharacterData(JObject obj)
    {
        _characterData.Clear();

        foreach (var prop in obj.Properties())
        {
            if (!int.TryParse(prop.Name, out int id)) continue;
            _characterData[id] = prop.Value as JObject ?? new JObject();
        }
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // System µ¥ÀÌÅÍ Á¢±Ù
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    /// <summary>
    /// ÇöÀç ¹öÀü¿¡ ÇØ´çÇÏ´Â ½Ã½ºÅÛ °ªÀ» ¹®ÀÚ¿­·Î ¹ÝÈ¯ÇÑ´Ù.
    /// ÇöÀç ¹öÀüÀÌ ¾øÀ¸¸é °¡Àå ¸¶Áö¸· ¹öÀü °ªÀ» ¹ÝÈ¯ÇÑ´Ù.
    /// </summary>
    public string GetSystemKey(string key)
    {
        if (!_systemData.TryGetValue(key, out var versionDict))
        {
            Debug.LogWarning($"[StaticDataLoader] ½Ã½ºÅÛ Å° ¾øÀ½: {key}");
            return "";
        }

        // ÇöÀç ¹öÀü ¿ì¼±
        if (versionDict.TryGetValue(currentVersion, out string val))
            return val;

        // ¾øÀ¸¸é ¸¶Áö¸· Ç×¸ñ ¹ÝÈ¯
        string lastValue = "";
        foreach (var v in versionDict.Values)
            lastValue = v;

        return lastValue;
    }

    /// <summary>
    /// ½Ã½ºÅÛ °ªÀ» Á¦³×¸¯ Å¸ÀÔÀ¸·Î ¹ÝÈ¯ÇÑ´Ù.
    /// </summary>
    public T GetSystemKey<T>(string key)
    {
        string raw = GetSystemKey(key);
        if (string.IsNullOrEmpty(raw)) return default;

        try
        {
            return (T)Convert.ChangeType(raw, typeof(T), CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            Debug.LogError($"[StaticDataLoader] ½Ã½ºÅÛ Å° Å¸ÀÔ º¯È¯ ½ÇÆÐ: {key}, °ª: [{raw}], ¿¡·¯: {e.Message}");
            return default;
        }
    }

    /// <summary>
    /// Æ¯Á¤ ¹öÀüÀÇ ½Ã½ºÅÛ °ªÀ» ¹ÝÈ¯ÇÑ´Ù.
    /// </summary>
    public string GetSystemKeyByVersion(string key, string version)
    {
        if (_systemData.TryGetValue(key, out var versionDict) &&
            versionDict.TryGetValue(version, out string val))
        {
            return val;
        }

        Debug.LogWarning($"[StaticDataLoader] ½Ã½ºÅÛ Å° ¾øÀ½: {key} (v{version})");
        return "";
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Locale µ¥ÀÌÅÍ Á¢±Ù
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    public string GetLocaleText(string key)
    {
        if (int.TryParse(key, out int id) && _localeData.TryGetValue(id, out var langDict))
        {
            if (langDict.TryGetValue(CurrentLanguage.ToString(), out string value))
                return value;
        }

        Debug.LogWarning($"[StaticDataLoader] ·ÎÄÉÀÏ Å° ¾øÀ½: {key}, ¾ð¾î: {CurrentLanguage}");
        return $"#{key}";
    }

    public string GetLocaleText(int id)
    {
        if (_localeData.TryGetValue(id, out var langDict))
        {
            if (langDict.TryGetValue(CurrentLanguage.ToString(), out string value))
                return value;
        }

        Debug.LogWarning($"[StaticDataLoader] ·ÎÄÉÀÏ Å° ¾øÀ½: {id}, ¾ð¾î: {CurrentLanguage}");
        return $"#{id}";
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Character µ¥ÀÌÅÍ Á¢±Ù
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    /// <summary>
    /// Ä³¸¯ÅÍ ¿øº» JObject¸¦ ¹ÝÈ¯ÇÑ´Ù. ÇÊ¿ä ½Ã Á÷Á¢ ÆÄ½ÌÇÏ°Å³ª ToObject »ç¿ë.
    /// </summary>
    public JObject GetCharacterRaw(int id)
    {
        if (_characterData.TryGetValue(id, out var obj))
            return obj;

        Debug.LogWarning($"[StaticDataLoader] Ä³¸¯ÅÍ µ¥ÀÌÅÍ ¾øÀ½: {id}");
        return null;
    }

    /// <summary>
    /// Ä³¸¯ÅÍ µ¥ÀÌÅÍ¸¦ Á¦³×¸¯ Å¸ÀÔÀ¸·Î ¿ªÁ÷·ÄÈ­ÇÏ¿© ¹ÝÈ¯ÇÑ´Ù.
    /// </summary>
    public T GetCharacterData<T>(int id) where T : class
    {
        JObject raw = GetCharacterRaw(id);
        if (raw == null) return null;

        try
        {
            return raw.ToObject<T>();
        }
        catch (Exception e)
        {
            Debug.LogError($"[StaticDataLoader] Ä³¸¯ÅÍ µ¥ÀÌÅÍ ¿ªÁ÷·ÄÈ­ ½ÇÆÐ: {id}, ¿¡·¯: {e.Message}");
            return null;
        }
    }

    public int GetCharacterCount() => _characterData.Count;

    public IReadOnlyDictionary<int, JObject> GetAllCharacterData() => _characterData;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¾ð¾î ¼³Á¤
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    public void SetLanguage(ELanguage lang) => CurrentLanguage = lang;

    public void SetCurrentVersion(string version) => currentVersion = version;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¿¡µðÅÍ À¯Æ¿
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

#if UNITY_EDITOR
    /// <summary>
    /// JSON ¹®ÀÚ¿­À» Resources/Static/ ¿¡ ÀúÀåÇÑ´Ù.
    /// </summary>
    public void SaveJsonToResources(string json, string fileName = null)
    {
        fileName ??= JSON_FILE_NAME;

        if (!Directory.Exists(RESOURCES_PATH))
        {
            Directory.CreateDirectory(RESOURCES_PATH);
            AssetDatabase.Refresh();
        }

        string filePath = Path.Combine(RESOURCES_PATH, fileName + ".json");
        File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"[StaticDataLoader] JSON ÀúÀå ¿Ï·á: {filePath}");
    }
#endif
}

// ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
// Locale È®Àå ¸Þ¼­µå (GoogleSheetsLoader È£È¯)
// ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

public static class StaticLocaleExtension
{
    public static string Locale(this int key, params object[] args)
    {
        return LocaleInternal(key.ToString(), args);
    }

    public static string Locale(this string key, params object[] args)
    {
        return LocaleInternal(key, args);
    }

    private static string LocaleInternal(string key, object[] args)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        string raw = StaticDataMgr.Inst.GetLocaleText(key);
        raw = ProcessEscapeSequences(raw);

        if (args == null || args.Length == 0) return raw;

        int maxIndex = GetMaxPlaceholderIndex(raw);
        if (maxIndex >= 0 && (args.Length - 1) < maxIndex)
        {
            var padded = new object[maxIndex + 1];
            for (int i = 0; i < padded.Length; i++)
                padded[i] = i < args.Length ? args[i] : string.Empty;
            args = padded;
        }

        try
        {
            return string.Format(CultureInfo.InvariantCulture, raw, args);
        }
        catch (FormatException e)
        {
            Debug.LogWarning($"Locale format mismatch for key '{key}': {e.Message}. Raw='{raw}'");
            return raw + " " + string.Join(" ", args);
        }
    }

    private static int GetMaxPlaceholderIndex(string s)
    {
        var matches = Regex.Matches(s, @"\{(\d+)(?:[^}]*)\}");
        int max = -1;
        foreach (Match m in matches)
        {
            if (int.TryParse(m.Groups[1].Value, out int idx) && idx > max)
                max = idx;
        }
        return max;
    }

    private static string ProcessEscapeSequences(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        return input
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t")
            .Replace("\\\\", "\\");
    }
}

public enum ELanguage
{
    kr,
    en
}