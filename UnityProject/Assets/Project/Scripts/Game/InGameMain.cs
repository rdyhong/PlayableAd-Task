using UnityEngine;

public class InGameMain : MonoBehaviour
{
    public InGameMainUI MainUI => _mainUI;
    [SerializeField] private InGameMainUI _mainUI;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        MainUI.Initialize();

        InGameMgr.Inst.StartGame();
    }
}
