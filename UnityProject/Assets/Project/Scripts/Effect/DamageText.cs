using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 데미지 텍스트 팝업 — 풀링 연동
/// </summary>
public class DamageText : MonoBehaviour, IPoolingObject
{
    [SerializeField] private TextMeshPro _text;
    private Sequence _seq;

    public void Play(Vector3 position, int damage)
    {
        transform.position = position + new Vector3(Random.Range(-0.3f, 0.3f), 0.5f, 0);

        if (_text == null)
            _text = GetComponent<TextMeshPro>();

        _text.text = damage.ToString();
        _text.color = damage >= 30 ? Color.yellow : Color.white;
        _text.alpha = 1f;

        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOMoveY(transform.position.y + GameConfig.DAMAGE_TEXT_RISE_HEIGHT, GameConfig.DAMAGE_TEXT_DURATION).SetEase(Ease.OutQuad));
        _seq.Join(_text.DOFade(0f, GameConfig.DAMAGE_TEXT_DURATION).SetEase(Ease.InQuad));
        _seq.OnComplete(() =>
        {
            ObjectPoolMgr.Inst.Recycle(gameObject);
        });
    }

    public void OnSpawn() { }

    public void OnRecycle()
    {
        _seq?.Kill();
        if (_text != null)
        {
            _text.alpha = 0f;
        }
    }
}
