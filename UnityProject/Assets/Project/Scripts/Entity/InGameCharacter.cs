using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;

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

    private double _lastAttackTime;

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
        _lastAttackTime = TimeMgr.GetUtcTimeSeconds();

        StartCoroutine(LifeCycleCo());
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnHit(1);
        }
    }

    private IEnumerator LifeCycleCo()
    {
        while(true)
        {
            Monster nearestMonster = MonsterMgr.Inst.GetNearestMonster(transform.position, GameConfig.PLAYER_ATTACK_RANGE);

            if (nearestMonster != null)
            {
                yield return AttackCo();
            }
            else
            {
                UpdateMovement();
            }

            yield return null;
        }
        
    }

    private void UpdateMovement()
    {
        Vector3 moveDir = Vector3.right;
        _cachedTransform.position += moveDir * Stat.moveSpeed * TimeMgr.ObjDeltaTime;

        _animController.PlayAnim(EAnimType.Walk);
    }

    public IEnumerator AttackCo()
    {
        float attackAnimDelay = 1f; // 공격 애니메이션 딜레이 시간 설정 필요

        while (true)
        {
            if (TimeMgr.GetUtcTimeSeconds() - _lastAttackTime >= Stat.attackCooldown)
            {
                // 가장 가까운 적 탐색
                EntityBase target = MonsterMgr.Inst.GetNearestMonster(_cachedTransform.position, 10);
                if (target != null)
                {
                    _animController.PlayAnim(EAnimType.Attack);

                    _lastAttackTime = 0f;
                    FireProjectile(target);
                }
                else
                {
                    break;
                }

                yield return new WaitForSeconds(attackAnimDelay);
            }
            yield return null;
        }
    }

    #region Auto Attack
    private void HandleAutoAttack()
    {
        if (_lastAttackTime < Stat.attackCooldown) return;

        // 가장 가까운 적 탐색
        EntityBase target = MonsterMgr.Inst.GetNearestMonster(_cachedTransform.position, Stat.attackRange);
        if (target == null) return;

        _lastAttackTime = 0f;
        FireProjectile(target);
    }

    private void FireProjectile(EntityBase target)
    {
        Vector3 dir = (target.transform.position - _cachedTransform.position).normalized;

        Projectile proj = ObjectPoolMgr.Inst.Spawn<Projectile>("Project/Prefabs/Effect/Projectile_0");
        proj.Init(this, target, GameConfig.PLAYER_ATK + GameConfig.PLAYER_ATK_PER_FISH_GRADE * SlotData.HighstGrade, ArcType.Down);
    }
    #endregion

    #region Damage / Death
    public override void OnHit(int damage, EntityBase attacker = null)
    {
        base.OnHit(damage, attacker);
    }

    public override void OnDead()
    {
        base.OnDead();

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
