using UnityEngine;
using DG.Tweening;

/// <summary>
/// 적 — 플레이어 추적, 근접 공격, 사망 시 풀링 회수
/// </summary>
public class Monster : EntityBase, IPoolingObject
{
    private EntityBase _target;
    private float _attackTimer;
    private bool _isDying;

    // 스탯 배율 (웨이브에 따라 강화)
    private float _statMultiplier = 1f;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(float statMultiplier = 1f)
    {
        _statMultiplier = statMultiplier;

        var stat = StatData.EnemyDefault();
        stat.maxHp = Mathf.RoundToInt(stat.maxHp * _statMultiplier);
        stat.curHp = stat.maxHp;
        stat.atk = Mathf.RoundToInt(stat.atk * _statMultiplier);

        InitStat(stat);

        _attackTimer = 0f;
        _isDying = false;
        _target = CharacterMgr.Inst.InGameCharacter;
    }

    private void Update()
    {
        if (IsDead || _isDying) return;
        if (_target == null) return;

        ChaseTarget();
        TryAttack();
    }

    #region AI
    private void ChaseTarget()
    {
        Vector3 dir = (_target.transform.position - transform.position).normalized;
        transform.position += dir * Stat.moveSpeed * TimeMgr.ObjDeltaTime;
    }

    private void TryAttack()
    {
        _attackTimer += TimeMgr.ObjDeltaTime;

        float dist = Vector3.Distance(transform.position, _target.transform.position);
        if (dist > Stat.attackRange) return;
        if (_attackTimer < Stat.attackCooldown) return;

        _attackTimer = 0f;
        Attack();
    }

    private void Attack()
    {
        if (_target == null) return;

        var player = _target.GetComponent<InGameCharacter>();
        player?.TakeDamage(Stat.atk, this);

    }
    #endregion

    #region Damage / Death
    public override void OnHit(int damage)
    {
        // 피격 플래시 (셰이더 기반 흰색 플래시)
        if (_hitFlash != null)
            _hitFlash.Flash();

        // 넉백
        if (_target != null)
        {
            Vector3 knockDir = (transform.position - _target.transform.position).normalized;
            transform.DOMove(transform.position + knockDir * 0.3f, 0.1f);
        }

        // 데미지 텍스트
        InGameMgr.Inst.SpawnDamageText(transform.position, damage);
    }

    public override void Dead()
    {
        _isDying = true;
        base.Dead();

        // 경험치 지급
        CharacterMgr.Inst.InGameCharacter.AddExp(GameConfig.EXP_PER_KILL);

        InGameMgr.Inst.OnEnemyKilled(this);

        ObjectPoolMgr.Inst.Recycle(gameObject);
    }
    #endregion

    #region Pooling
    public override void OnSpawn()
    {
        // Init()에서 처리
    }

    public override void OnRecycle()
    {
        _isDying = false;
        _target = null;

        transform.DOKill();
    }
    #endregion
}
