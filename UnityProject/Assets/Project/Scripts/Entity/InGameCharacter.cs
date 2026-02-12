using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 — 조이스틱 이동, 자동공격, 레벨업
/// </summary>
public class InGameCharacter : EntityBase
{
    [Header("Level")]
    public int Level { get; private set; } = 1;
    public int Exp { get; private set; } = 0;
    public int ExpToNextLevel { get; private set; }

    public event Action<int> OnLevelUp;
    public event Action<int, int> OnExpChanged; // cur, max

    private float _attackTimer;

    // 캐싱
    private Transform _cachedTransform;

    protected override void Awake()
    {
        base.Awake();
        _cachedTransform = transform;
    }

    public override void Initialize()
    {
        base.Initialize();

        transform.position = InGameMain.Inst.CachedPosition.CharacterPos;

        InitStat(StatData.PlayerDefault());
        Level = 1;
        Exp = 0;
        ExpToNextLevel = GameConfig.EXP_BASE_REQUIRED;
        _attackTimer = 0f;
    }

    private void Update()
    {
        if (IsDead) return;

        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnHit(1);
        }

        HandleMovement();
        //HandleAutoAttack();
    }

    #region Movement
    private void HandleMovement()
    {
        Vector3 moveDir = Vector3.right;
        _cachedTransform.position += moveDir * Stat.moveSpeed * TimeMgr.ObjDeltaTime;

        _animController.PlayAnim(EAnimType.Walk);
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
    public override void OnHit(int damage)
    {
        base.OnHit(damage);

        // 피격 플래시 (셰이더 기반 흰색 플래시)
        if (_hitFlash != null)
            _hitFlash.Flash();
    }

    public override void Dead()
    {
        base.Dead();

        _animController.PlayAnim(EAnimType.Death);

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
