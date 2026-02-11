using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : Singleton<UIMgr>
{
    public MainOverrideUI MainOverrideUI { get; private set; } = null;

    public void Initialize()
    {
        if (MainOverrideUI == null) MainOverrideUI = ObjectPoolMgr.Inst.Spawn<MainOverrideUI>("Prefabs/UI/MainOverrideUI");

        MainOverrideUI.Initialize();
    }

}
