using UnityEngine;

public class InGameMainUI : MonoBehaviour
{
    public Canvas Canvas { get; private set; }

    public InGameUI InGameUI => _inGameUI;
    [SerializeField] private InGameUI _inGameUI;

    public void Initialize()
    {
        Canvas = GetComponent<Canvas>();
        InGameUI.Initialize();
    }
}
