using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public InGameBoardUI BoardUI => _boardUI;
    [SerializeField] private InGameBoardUI _boardUI;

    public void Initialize()
    {
        BoardUI.Initialize();
    }
}
