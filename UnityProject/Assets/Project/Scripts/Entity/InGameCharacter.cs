using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 플레이어 — 조이스틱 이동, 자동공격, 레벨업
/// </summary>
public class InGameCharacter : EntityBase
{
    public static InGameCharacter Instance { get; private set; }

    [Header("Level")]
    public int Level { get; private set; } = 1;
    public int Exp { get; private set; } = 0;
    public int ExpToNextLevel { get; private set; }

    public event Action<int> OnLevelUp;
    public event Action<int, int> OnExpChanged; // cur, max

    private float _attackTimer;
    private bool _isInvincible;

    // 캐싱
    private Transform _cachedTransform;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _cachedTransform = transform;
    }

    public void Init()
    {
        InitStat(StatData.PlayerDefault());
        Level = 1;
        Exp = 0;
        ExpToNextLevel = GameConfig.EXP_BASE_REQUIRED;
        _attackTimer = 0f;
        _isInvincible = false;
    }

    private void Update()
    {
        if (IsDead) return;

        HandleMovement();
        HandleAutoAttack();
    }

    #region Movement
    private void HandleMovement()
    {
        Vector3 dir = InputMgr.StickDir;
        float weight = InputMgr.StickWeight;

        if (weight <= 0) return;

        Vector3 moveDir = dir.normalized;
        _cachedTransform.position += moveDir * Stat.moveSpeed * weight * TimeMgr.ObjDeltaTime;

        FlipSprite(moveDir.x);

        if (_animator != null)
            _animator.SetBool("IsMoving", weight > 0.1f);
    }
    #endregion

    #region Auto Attack
    private void HandleAutoAttack()
    {
        _attackTimer += TimeMgr.ObjDeltaTime;

        if (_attackTimer < Stat.attackCooldown) return;

        // 가장 가까운 적 탐색
        Transform target = InGameMgr.Inst.GetNearestEnemy(_cachedTransform.position, Stat.attackRange);
        if (target == null) return;

        _attackTimer = 0f;
        FireProjectile(target);
    }

    private void FireProjectile(Transform target)
    {
        Vector3 dir = (target.position - _cachedTransform.position).normalized;

        Projectile proj = ObjectPoolMgr.Inst.Spawn<Projectile>("Prefabs/Game/Projectile");
        proj.Init(_cachedTransform.position, dir, Stat.atk);
    }
    #endregion

    #region Damage / Death
    protected override void OnHit(int damage)
    {
        if (_isInvincible) return;

        // 피격 깜빡임
        if (_spriteRenderer != null)
        {
            _spriteRenderer.DOColor(Color.red, 0.1f)
                .OnComplete(() => _spriteRenderer.DOColor(Color.white, 0.1f));
        }
    }

    protected override void Die()
    {
        base.Die();
        if (_animator != null)
            _animator.SetTrigger("Die");

        // GameOver 통보
        InGameMgr.Inst.OnPlayerDead();
    }
    #endregion

    #region EXP / Level
    public void AddExp(int amount)
    {
        Exp += amount;
        OnExpChanged?.Invoke(Exp, ExpToNextLevel);

        while (Exp >= ExpToNextLevel)
        {
            Exp -= ExpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        ExpToNextLevel = Mathf.RoundToInt(ExpToNextLevel * GameConfig.EXP_LEVEL_MULTIPLIER);

        // 스탯 강화
        Stat.maxHp += 10;
        Stat.curHp = Stat.maxHp;
        Stat.atk += 3;

        OnLevelUp?.Invoke(Level);
        OnExpChanged?.Invoke(Exp, ExpToNextLevel);
    }
    #endregion
}
