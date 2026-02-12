using System.Collections;
using UnityEngine;

/// <summary>
/// 피격 시 모든 자식 SpriteRenderer를 흰색으로 플래시.
/// Custom/SpriteHitFlash 셰이더의 _HitFlash 프로퍼티를 MaterialPropertyBlock으로 제어.
/// </summary>
public class HitFlash : MonoBehaviour
{
    [SerializeField] private float _flashDuration = 0.12f;

    private SpriteRenderer[] _renderers;
    private MaterialPropertyBlock _mpb;
    private Coroutine _flashCoroutine;

    private static readonly int HitFlashID = Shader.PropertyToID("_HitFlash");

    private void Awake()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        _mpb = new MaterialPropertyBlock();
    }

    /// <summary>
    /// 플래시 실행. 중복 호출 시 기존 코루틴 중단 후 재시작.
    /// </summary>
    public void Flash()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetFlash(1f);

        float elapsed = 0f;
        while (elapsed < _flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _flashDuration);
            SetFlash(1f - t);
            yield return null;
        }

        SetFlash(0f);
        _flashCoroutine = null;
    }

    private void SetFlash(float value)
    {
        _mpb.SetFloat(HitFlashID, value);
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null)
                _renderers[i].SetPropertyBlock(_mpb);
        }
    }

    /// <summary>
    /// 플래시 즉시 초기화. 풀 재사용 시 호출.
    /// </summary>
    public void ResetFlash()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }
        SetFlash(0f);
    }

    /// <summary>
    /// 런타임 중 자식이 변경되었을 때 렌더러 배열 갱신용
    /// </summary>
    public void RefreshRenderers()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
}
