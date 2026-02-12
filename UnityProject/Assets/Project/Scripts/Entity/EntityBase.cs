using System;
using UnityEngine;

/// <summary>
/// 모든 게임 엔티티(플레이어, 적)의 공통 베이스
/// </summary>
public abstract class EntityBase : MonoBehaviour, IPoolingObject
{
    [Header("Stat")]
    public StatData Stat { get; protected set; }

    public bool IsDead => Stat != null && Stat.IsDead;

    public event Action<EntityBase> OnDeath;
    public event Action<EntityBase, int> OnDamaged;

    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Animator _animator;

    protected virtual void Awake()
    {
        
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

        OnDamaged?.Invoke(this, actualDamage);
        OnHit(actualDamage);

        if (IsDead)
        {
            Dead();
        }

        return actualDamage;
    }

    public virtual void OnHit(int damage)
    {
        // 피격 연출 (자식에서 override)
    }

    public virtual void Dead()
    {
        OnDeath?.Invoke(this);
    }

    /// <summary>
    /// 스프라이트 방향 전환
    /// </summary>
    protected void FlipSprite(float dirX)
    {
        if (_spriteRenderer == null) return;
        if (dirX == 0) return;

        _spriteRenderer.flipX = dirX < 0;
    }

    public virtual void OnSpawn()
    {
    }

    public virtual void OnRecycle()
    {
    }
}
