using System;
using UnityEngine;

/// <summary>
/// 모든 게임 엔티티(플레이어, 적)의 공통 베이스
/// </summary>
public abstract class EntityBase : MonoBehaviour, IPoolingObject
{
    [SerializeField] protected Transform _bodyRoot;

    [Header("Stat")]
    public StatData Stat { get; protected set; }

    protected HitFlash _hitFlash;
    
    public bool IsDead => Stat != null && Stat.IsDead;

    protected AnimController _animController;

    protected virtual void Awake()
    {
        
    }

    public virtual void Initialize()
    {
        _animController = GetComponent<AnimController>();

        _hitFlash = GetComponent<HitFlash>();
    }

    public virtual void InitStat(StatData stat)
    {
        Stat = stat;
    }

    /// <summary>
    /// 데미지 처리. 반환값: 실제 적용된 데미지
    /// </summary>
    public virtual int TakeDamage(int damage, EntityBase attacker = null)
    {
        if (IsDead) return 0;

        int actualDamage = Mathf.Max(1, damage);
        Stat.TakeDamage(actualDamage);

        OnHit(actualDamage);

        if (IsDead)
        {
            OnDead();
        }

        return actualDamage;
    }

    public virtual void OnHit(int damage, EntityBase attacker = null)
    {
        if (IsDead) return;

        // 피격 플래시 (셰이더 기반 흰색 플래시)   
        _hitFlash.Flash();

        Stat.TakeDamage(damage);

        if (IsDead)
        {
            OnDead();
        }
    }

    public virtual void OnDead()
    {

    }

    public virtual void OnSpawn()
    {
    }

    public virtual void OnRecycle()
    {
    }
}
