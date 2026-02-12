using UnityEngine;

public class InGameMain : MonoBehaviour
{
    public static InGameMain Inst { get; private set; } = null;

    public InGameCamController CamController => _camController;
    [SerializeField] private InGameCamController _camController;

    public InGameMainUI MainUI => _mainUI;
    [SerializeField] private InGameMainUI _mainUI;

    public InGameCachedPosition CachedPosition => _cachedPosition;
    [SerializeField] private InGameCachedPosition _cachedPosition;

    private void Awake()
    {
        Inst = this;

        Initialize();
    }

    private void Initialize()
    {
        InGameMgr.Inst.Initialize();

        MainUI.Initialize();

        // Start Game
        InGameMgr.Inst.StartGame();
    }
}
