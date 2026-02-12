using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 적 — 플레이어 추적, 근접 공격, 사망 시 풀링 회수
/// </summary>
public class Monster : EntityBase, IPoolingObject
{
    private EntityBase _target;
    private bool _isDying;

    // 스탯 배율 (웨이브에 따라 강화)
    private float _statMultiplier = 1f;

    private Coroutine _lifeCycleCo;

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

        _isDying = false;
        _target = CharacterMgr.Inst.InGameCharacter;

        if (_lifeCycleCo != null)
            StopCoroutine(_lifeCycleCo);
        _lifeCycleCo = StartCoroutine(LifeCycleCo());
    }

    private IEnumerator LifeCycleCo()
    {
        while (!IsDead && !_isDying)
        {
            if (_target == null || _target.IsDead)
            {
                _animController.PlayAnim(EAnimType.Idle);
                yield return null;
                continue;
            }

            float dist = Mathf.Abs(_target.transform.position.x - transform.position.x);

            if (dist <= Stat.attackRange)
            {
                yield return AttackCo();
            }
            else
            {
                MoveUpdate();
            }

            yield return null;
        }
    }

    private void MoveUpdate()
    {
        Vector3 dir = (_target.transform.position - transform.position).normalized;
        dir.y = 0f;
        dir.z = 0f;
        transform.position += Vector3.left * Stat.moveSpeed * TimeMgr.ObjDeltaTime;

        _animController.PlayAnim(EAnimType.Walk);
    }

    private IEnumerator AttackCo()
    {
        _animController.PlayAnim(EAnimType.Attack);

        // 공격 판정
        if (_target != null && !_target.IsDead)
        {
            //_target.OnHit(Stat.atk, this);
            FireProjectile(_target);
        }

        // 공격 쿨다운 대기
        yield return new WaitForSeconds(Stat.attackCooldown);
    }

    private void FireProjectile(EntityBase target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;

        Projectile proj = ObjectPoolMgr.Inst.Spawn<Projectile>("Project/Prefabs/Effect/Projectile_1");
        proj.Init(this, target, GameConfig.PLAYER_ATK + GameConfig.PLAYER_ATK_PER_FISH_GRADE * SlotData.HighstGrade, ArcType.Up);
    }

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

        if (_lifeCycleCo != null)
        {
            StopCoroutine(_lifeCycleCo);
            _lifeCycleCo = null;
        }

        _animController.PlayAnim(EAnimType.Death);

        // 경험치 지급
        CharacterMgr.Inst.InGameCharacter.AddExp(GameConfig.EXP_PER_KILL);

        MonsterMgr.Inst.OnMonsterDead(this);

        ObjectPoolMgr.Inst.Recycle(gameObject);
    }
    #endregion

    #region Pooling
    public override void OnSpawn()
    {
        // Initialize()에서 처리
    }

    public override void OnRecycle()
    {
        _isDying = false;
        _target = null;

        if (_lifeCycleCo != null)
        {
            StopCoroutine(_lifeCycleCo);
            _lifeCycleCo = null;
        }

        _bodyRoot.DOKill();
        _bodyRoot.localPosition = Vector3.zero;
    }
    #endregion
}
