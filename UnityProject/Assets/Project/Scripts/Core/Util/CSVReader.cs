using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public enum TableType
{
    
}

public partial class CSVReader : Singleton<CSVReader>
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    // File name, Index
    private static Dictionary<TableType, Dictionary<int, Dictionary<string, string>>> tables = new Dictionary<TableType, Dictionary<int, Dictionary<string, string>>>();
    readonly static string tablePath = "04.Table/";
    public static bool isAllTableLoaded = false;
    //private bool isLoad = false;

    public static Dictionary<TableType, Dictionary<int, Dictionary<string, string>>> Tables { get => tables; set => tables = value; }

    public void Init()
    {
        LoadTableAll();
    }

    public void LoadTableAll()
    {
        foreach (var file in Resources.LoadAll(tablePath))
        {
            Load(file.name);
        }

        isAllTableLoaded = true;
    }
    public void Load(string file , int rawSpace = 0)
    {
        string source;
        TextAsset textAsset = Resources.Load(Path.Combine(tablePath,file)) as TextAsset;
        source = textAsset.text;

        var lines = Regex.Split(source, LINE_SPLIT_RE);
        if (lines.Length <= 1) return;
        var header = Regex.Split(lines[0], SPLIT_RE);

        var rowEntry = new Dictionary<int, Dictionary<string, string>> ();
        for (var i = 1 + rawSpace; i < lines.Length; i++)
        {
            try
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                int idx;
                if (int.TryParse(values[0], out idx) == false)
                {
                    continue;
                }
                var columnEntry = new Dictionary<string, string>();
                for (var j = 1; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n");
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                    object finalvalue = value;

                    if(!columnEntry.ContainsKey(header[j]))
                    {
                        columnEntry.Add(header[j], finalvalue.ToString());
                    }
                    else
                    {
                        Exception ex = new Exception($"{file} - {header[j]} : already has key.");
                    }
                }
                if (!rowEntry.ContainsKey(idx))
                {
                    rowEntry.Add(idx, columnEntry);
                }
                else
                {
                    Exception ex = new Exception($"{file} - {idx} : already has key.");
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogErr($"{file} Load Error {ex.Message}");
            }
        }
        try
        {
            Tables.Add(GameUtil.StringToEnum<TableType>(file), rowEntry);
        }
        catch
        {
            DebugUtil.LogErr($"Add Table Error\n{file}\n{source}");
        }
    }
}
