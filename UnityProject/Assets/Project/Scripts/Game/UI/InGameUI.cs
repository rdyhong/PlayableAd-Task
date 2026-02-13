using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public InGameBoardUI BoardUI => _boardUI;
    [SerializeField] private InGameBoardUI _boardUI;

    [SerializeField] private CanvasGroup _cgWaveOver;

    public void Initialize()
    {
        BoardUI.Initialize();
    }

    public IEnumerator WaveOverCo(bool isHide)
    {
        if(isHide) yield return _cgWaveOver.DOFade(1f, 0.5f).WaitForCompletion();
        else yield return _cgWaveOver.DOFade(0f, 0.5f).WaitForCompletion();
    }
}
