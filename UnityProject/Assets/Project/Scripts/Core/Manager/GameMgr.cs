//using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : Singleton<GameMgr>
{
    public static string s_Version { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    // Application setting
    public void ApplicationSetting()
    {
        //Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        UnityEngine.Random.InitState(DateTime.Now.TimeOfDay.Seconds);
        s_Version = Application.version;
        Application.runInBackground = false;
    }

    public bool IsDev()
    {
#if UNITY_EDITOR
        return true;
#endif
        return false;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
