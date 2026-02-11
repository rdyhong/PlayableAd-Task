using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// UI Manager
public sealed class UIWindowMgr : Singleton<UIWindowMgr>
{
    public static List<UIWindow> totalOpenWindows = new List<UIWindow>();

    public static List<UIWindow> TotalOpenWindows
    {
        get
        {
            if (totalOpenWindows == null)
            {
                totalOpenWindows = new List<UIWindow>();
            }
            else
            {
                totalOpenWindows.RemoveAll(_ => _ == null);
            }
            return totalOpenWindows;
        }
        set
        {
            totalOpenWindows = value;
        }
    }

    // Main Canvas
    private static Canvas mainCanvas;
    public static Canvas MainCanvas
    {
        get
        {
            if (mainCanvas == null)
                mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
            if (mainCanvas == null)
                Debug.Log("UIWindowMgr : Not Exists MainCanvas");
            return mainCanvas;
        }
    }
    // Currunt scene Canvas
    private static Canvas sceneCanvas;
    public static Canvas SceneCanvas
    {
        get
        {
            if (sceneCanvas == null)
                sceneCanvas = GameObject.Find("SceneCanvas").GetComponent<Canvas>();
            if (sceneCanvas == null)
                Debug.Log("UIWindowMgr : Not Exists SceneCanvas");
            return sceneCanvas;
        }
    }

    public static List<UIWindow> totalUIWindows = new List<UIWindow>();
    private static Dictionary<string, UIWindow> cachedTotalUIWindows = new Dictionary<string, UIWindow>();
    private static Dictionary<string, object> cachedInstances = new Dictionary<string, object>();

    public static void AddTotalWindow(UIWindow uiwindow)
    {
        if (totalUIWindows.Contains(uiwindow) || cachedTotalUIWindows.ContainsKey(uiwindow.GetType().Name))
        {
            if (cachedTotalUIWindows[uiwindow.GetType().Name] != null)
            {
                return;
            }
            else
            {
                for (int i = 0; i < totalUIWindows.Count; i++)
                {
                    if (totalUIWindows[i] == null)
                        totalUIWindows.RemoveAt(i);
                }
                totalUIWindows.Add(uiwindow);
                cachedTotalUIWindows[uiwindow.GetType().Name] = uiwindow;
                return;
            }
        }

        totalUIWindows.Add(uiwindow);
        cachedTotalUIWindows.Add(uiwindow.GetType().Name, uiwindow);
    }

    public static void AddOpendWindow(UIWindow uiwindow)
    {
        if (TotalOpenWindows.Contains(uiwindow) == false)
            TotalOpenWindows.Add(uiwindow);
    }

    public static void RemoveOpendWindow(UIWindow uiwindow)
    {
        if (TotalOpenWindows.Contains(uiwindow))
            TotalOpenWindows.Remove(uiwindow);
    }

    public static T GetInstance<T>()
    {
        string type = typeof(T).ToString();
        if (cachedTotalUIWindows.ContainsKey(type) == false)
            return default(T);

        if (cachedInstances.ContainsKey(type) == false)
            cachedInstances.Add(type, (T)Convert.ChangeType(cachedTotalUIWindows[typeof(T).ToString()], typeof(T)));
        else if (cachedInstances[type].Equals(null))
            cachedInstances[type] = (T)Convert.ChangeType(cachedTotalUIWindows[type], typeof(T));

        return (T)cachedInstances[typeof(T).ToString()];
    }

    public static void CloseAll()
    {
        for (int i = 0; i < totalUIWindows.Count; i++)
        {
            if (totalUIWindows[i] != null)
                totalUIWindows[i].Close(true);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        InitAllWindows();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIWindow targetWindow = GetTopUIWindow();
            if (targetWindow != null && targetWindow.isCloseESC)
                targetWindow.Close();
        }
    }

    public static void InitAllWindows()
    {
        for (int i = 0; i < totalUIWindows.Count; i++)
        {
            UIWindow targetWindow = totalUIWindows[i];

            if (targetWindow != null)
            {
                if (targetWindow.isOpen)
                    targetWindow.Open(true);
                else
                    targetWindow.Close(true);
            }
        }

        for (int i = 0; i < totalUIWindows.Count; i++)
        {
            UIWindow targetWindow = totalUIWindows[i];

            if (targetWindow != null)
                targetWindow.InitWindow();
        }
    }

    public static UIWindow GetTopUIWindow()
    {
        UIWindow targetWindow = null;

        if (TotalOpenWindows.Count > 0)
            targetWindow = TotalOpenWindows[TotalOpenWindows.Count - 1];

        return targetWindow;
    }
}
