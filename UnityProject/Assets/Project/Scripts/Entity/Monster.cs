using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

    public override void Initialize()
    {
        base.Initialize();

        _statMultiplier = InGameMgr.Inst.CurrentWave;

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
        Vector3 dir = Vector3.left;
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
        player.OnHit(Stat.atk, this);

    }
    #endregion

    #region Damage / Death
    public override void OnHit(int damage, EntityBase attacker = null)
    {
        base.OnHit(damage, attacker);

        // 넉백 → 복귀
        if (_target != null)
        {
            _bodyRoot.DOKill();
            _bodyRoot.localPosition = Vector3.zero;

            Vector3 knockDir = (transform.position - _target.transform.position).normalized;
            _bodyRoot.DOLocalMove(knockDir * 0.3f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _bodyRoot.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.InQuad));
        }
    }

    public override void OnDead()
    {
        _isDying = true;
        base.OnDead();

        // 경험치 지급
        CharacterMgr.Inst.InGameCharacter.AddExp(GameConfig.EXP_PER_KILL);

        MonsterMgr.Inst.OnMonsterDead(this);

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
