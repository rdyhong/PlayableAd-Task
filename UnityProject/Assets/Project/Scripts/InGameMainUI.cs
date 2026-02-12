using UnityEngine;

public class InGameMainUI : MonoBehaviour
{
    public InGameUI InGameUI => _inGameUI;
    [SerializeField] private InGameUI _inGameUI;

    public void Initialize()
    {
        InGameUI.Initialize();
    }
}
