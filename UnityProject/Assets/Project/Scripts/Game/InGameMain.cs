using UnityEngine;

public class InGameMain : MonoBehaviour
{
    public static InGameMain Inst { get; private set; } = null;

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
        MainUI.Initialize();

        InGameMgr.Inst.StartGame();
    }
}
