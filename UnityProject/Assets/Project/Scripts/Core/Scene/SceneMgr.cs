using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum EScene
{
    None = -1,
    AppStartScene = 0,
    Title,
    LobbyScene,
    InGameScene
}

/// <summary>
/// Manage Load scene
/// </summary>
public class SceneMgr : Singleton<SceneMgr>
{
    private EScene loadSceneKind = EScene.None;

    public static EScene curSceneKind;

    protected override void Awake()
    {
        base.Awake();
    }

    public void LoadScene(EScene sceneKind, bool force = false)
    {
        if (force)
        {
            SceneManager.LoadScene(sceneKind.ToString());
            return;
        }
        
        StartCoroutine(LoadSceneAsync(sceneKind));
    }

    private IEnumerator LoadSceneAsync(EScene sceneKind)
    {
        loadSceneKind = sceneKind;

        yield return UIMgr.Inst.MainOverrideUI.LoadSceneUI.ShowCo();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneKind.ToString());
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            UIMgr.Inst.MainOverrideUI.LoadSceneUI.UpdateProgress(progress);

            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.2f);
                asyncLoad.allowSceneActivation = true;
                UIMgr.Inst.MainOverrideUI.LoadSceneUI.UpdateProgress(1);
            }

            yield return null;
        }

        yield return Resources.UnloadUnusedAssets();

        GC.Collect();

        curSceneKind = loadSceneKind;

        yield return UIMgr.Inst.MainOverrideUI.LoadSceneUI.HideCo();
    }
}