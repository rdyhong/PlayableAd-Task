using UnityEngine;
using DG.Tweening;

/// <summary>
/// 피격 이펙트 — 스케일+페이드 후 회수
/// </summary>
public class HitEffect : MonoBehaviour, IPoolingObject
{
    [SerializeField] private SpriteRenderer _sprite;
    private Sequence _seq;

    public void Play(Vector3 position)
    {
        transform.position = position;
        transform.localScale = Vector3.zero;

        if (_sprite == null)
            _sprite = GetComponent<SpriteRenderer>();

        if (_sprite != null)
            _sprite.color = Color.white;

        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOScale(1f, GameConfig.HIT_EFFECT_DURATION * 0.4f).SetEase(Ease.OutBack));
        _seq.Append(transform.DOScale(0f, GameConfig.HIT_EFFECT_DURATION * 0.6f).SetEase(Ease.InQuad));
        _seq.OnComplete(() =>
        {
            ObjectPoolMgr.Inst.Recycle(gameObject);
        });
    }

    public void OnSpawn() { }

    public void OnRecycle()
    {
        _seq?.Kill();
        transform.localScale = Vector3.zero;
    }
}
