using UnityEngine;

public enum ArcType { Up, Down }

/// <summary>
/// 투사체 — 곡선 이동, 타겟 추적, 도달 시 1초 대기 후 회수
/// </summary>
public class Projectile : MonoBehaviour, IPoolingObject
{
    private EntityBase _target;
    private int _damage;
    private float _lifeTimer;
    private bool _isActive;
    private bool _isWaitingRecycle;

    private Vector3 _startPos;
    private float _elapsed;
    private float _duration;
    private float _resolvedArcHeight;
    private float _recycleTimer;

    [SerializeField] private float _arcHeight = 2f;
    [SerializeField] private float _arcRandomRange = 0.5f;
    [SerializeField] private float _recycleDelay = 1f;
    [SerializeField] private ParticleSystem _trail;
    [SerializeField] private GameObject _go;

    public void Init(EntityBase startEntity, EntityBase targetEntity, int damage, ArcType arcType = ArcType.Up, float duration = GameConfig.PROJECTILE_LIFETIME)
    {
        _startPos = startEntity.transform.position;
        transform.position = _startPos;
        _target = targetEntity;
        _damage = damage;
        _elapsed = 0f;
        _duration = duration;
        _lifeTimer = _duration;
        _isActive = true;
        _isWaitingRecycle = false;

        float randomOffset = Random.Range(-_arcRandomRange, _arcRandomRange);
        _resolvedArcHeight = arcType == ArcType.Up
            ? _arcHeight + randomOffset
            : -(_arcHeight + randomOffset);

        if (_trail != null)
        {
            _trail.Clear();
            _trail.Play();
        }

        _go.SetActive(true);
    }

    private void Update()
    {
        if (_isWaitingRecycle)
        {
            _recycleTimer -= TimeMgr.ObjDeltaTime;
            if (_recycleTimer <= 0f)
                Recycle();
            return;
        }

        if (!_isActive) return;

        _elapsed += TimeMgr.ObjDeltaTime;
        _lifeTimer -= TimeMgr.ObjDeltaTime;

        if (_lifeTimer <= 0f || _target == null || _target.IsDead)
        {
            OnArrive();
            return;
        }

        float t = Mathf.Clamp01(_elapsed / _duration);
        Vector3 targetPos = _target.transform.position;

        Vector3 linearPos = Vector3.Lerp(_startPos, targetPos, t);
        float arc = _resolvedArcHeight * 4f * t * (1f - t);
        linearPos.y += arc;

        Vector3 dir = linearPos - transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }

        transform.position = linearPos;

        // t가 1 이상이면 도달
        if (t >= 1f)
        {
            OnArrive();
        }
    }

    private void OnArrive()
    {
        _go.SetActive(false);
        _isActive = false;
        _isWaitingRecycle = true;
        _recycleTimer = _recycleDelay;

        // 데미지 처리
        if (_target != null && !_target.IsDead)
        {
            _target.OnHit(_damage);

            EffectBase eff = ObjectPoolMgr.Inst.Spawn<EffectBase>("Project/Prefabs/Effect/InGameOnHitEffect");
            eff.Play(transform.position);
            //var monster = _target as Monster;
            //if (monster != null)
            //{
            //    monster.OnHit(_damage);
            //    //InGameMgr.Inst.SpawnHitEffect(monster.transform.position);
            //}
        }

        // 파티클 emit 중지 → 대기 동안 자연 소멸
        if (_trail != null)
            _trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void Recycle()
    {
        _isWaitingRecycle = false;
        _isActive = false;
        if (_trail != null)
            _trail.Clear();
        ObjectPoolMgr.Inst.Recycle(gameObject);
    }   

    public void OnSpawn()
    {
        _isActive = false;
        _isWaitingRecycle = false;
    }

    public void OnRecycle()
    {
        _isActive = false;
        _isWaitingRecycle = false;
    }
}
