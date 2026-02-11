using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneUI : MonoBehaviour
{
    private CanvasGroup _cg;

    [SerializeField] private RectTransform _rtRoot;
    [SerializeField] private TextMeshProUGUI _txtContent;
    [SerializeField] private Image _imgProgress;

    public void Initialize()
    {
        _rtRoot.gameObject.SetActive(false);

        _cg = GetComponent<CanvasGroup>();

        _cg.alpha = 0f;
        _txtContent.text = string.Empty;
        UpdateProgress(0);
    }

    public IEnumerator ShowCo()
    {
        _cg.alpha = 0f;
        _txtContent.text = string.Empty;
        UpdateProgress(0);

        _rtRoot.gameObject.SetActive(true);
        yield return _cg.DOFade(1, 1).SetEase(Ease.Linear).WaitForCompletion();
    }

    public IEnumerator HideCo()
    {
        _imgProgress.fillAmount = 1;

        yield return _cg.DOFade(0, 1).SetEase(Ease.Linear).WaitForCompletion();

        _rtRoot.gameObject.SetActive(false);
    }

    /// <summary>
    /// 0 ~ 1
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateProgress(float amount)
    {
        _imgProgress.fillAmount = amount;
    }

    public void UpdateContentText(string content)
    {
        _txtContent.text = content;
    }


}
