using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// 피격 이펙트 — ParticleSystem 재생 후 duration 대기하고 풀 회수
/// </summary>
public class EffectBase : MonoBehaviour, IPoolingObject
{
    [SerializeField] private ParticleSystem _particle;

    private Coroutine _recycleCoroutine;

    private RectTransform _rt;

    public void Play(Vector3 position, bool isUI = false)
    {
        if (isUI)
        {
            if (_rt == null) _rt = GetComponent<RectTransform>();

            _rt.anchoredPosition3D = position;
            _rt.SetParent(InGameMain.Inst.MainUI.InGameUI.BoardUI.RtEffectParent, false);
        }
        else
        {
            transform.position = position;
        }

        transform.localScale = Vector3.one;

        Initialize();
    }

    private void Initialize()
    {
        if (_particle == null)
            _particle = GetComponent<ParticleSystem>();

        if (_particle == null) return;

        _particle.Clear();
        _particle.Play();

        if (_recycleCoroutine != null)
            StopCoroutine(_recycleCoroutine);

        _recycleCoroutine = StartCoroutine(RecycleAfterDuration());
    }

    private IEnumerator RecycleAfterDuration()
    {
        yield return new WaitForSeconds(_particle.main.duration + _particle.main.startLifetime.constantMax);
        ObjectPoolMgr.Inst.Recycle(gameObject);
        _recycleCoroutine = null;
    }

    public void OnSpawn() { }

    public void OnRecycle()
    {
        if (_recycleCoroutine != null)
        {
            StopCoroutine(_recycleCoroutine);
            _recycleCoroutine = null;
        }

        if (_particle != null)
            _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
