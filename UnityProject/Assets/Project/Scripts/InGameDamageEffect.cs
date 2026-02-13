using UnityEngine;
using TMPro;
using DG.Tweening;

public class InGameDamageEffect : MonoBehaviour, IPoolingObject
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = 0.7f;

    private Sequence _seq;

    public void PlayEffect(Vector3 position, int damage)
    {
        transform.position = position;
        transform.localScale = Vector3.one * 0.5f;
        _text.text = damage.ToString();
        _text.alpha = 1f;

        _seq?.Kill();
        _seq = DOTween.Sequence()
            // 스케일 펀치 (타격감)
            .Append(transform.DOScale(1.3f, 0.1f).SetEase(Ease.OutBack))
            .Append(transform.DOScale(1f, 0.1f).SetEase(Ease.InOutSine))
            // 위로 떠오르며 페이드
            .Join(transform.DOMoveY(position.y + 0.8f, _duration).SetEase(Ease.OutCubic))
            .Join(_text.DOFade(0f, _duration * 0.5f).SetDelay(_duration * 0.5f))
            .OnComplete(OnRecycle);
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    public void OnRecycle()
    {
        _seq?.Kill();
        _seq = null;
        gameObject.SetActive(false);
    }
}