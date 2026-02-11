using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainOverrideUI : MonoBehaviour
{
    public LoadSceneUI LoadSceneUI => _loadSceneUI;
    [SerializeField] private LoadSceneUI _loadSceneUI;

    public void Initialize()
    {
        LoadSceneUI.Initialize();


    }
}
