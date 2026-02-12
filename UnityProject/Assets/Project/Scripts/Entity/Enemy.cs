using UnityEngine;
using DG.Tweening;

/// <summary>
/// 적 — 플레이어 추적, 근접 공격, 사망 시 풀링 회수
/// </summary>
public class Enemy : EntityBase, IPoolingObject
{
    private Transform _target;
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
        _target = InGameMgr.Inst.InGameCharacter.transform;

        if (_spriteRenderer != null)
            _spriteRenderer.color = Color.white;
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
        Vector3 dir = (_target.position - transform.position).normalized;
        transform.position += dir * Stat.moveSpeed * TimeMgr.ObjDeltaTime;

        FlipSprite(dir.x);

        if (_animator != null)
            _animator.SetBool("IsMoving", true);
    }

    private void TryAttack()
    {
        _attackTimer += TimeMgr.ObjDeltaTime;

        float dist = Vector3.Distance(transform.position, _target.position);
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

        if (_animator != null)
            _animator.SetTrigger("Attack");
    }
    #endregion

    #region Damage / Death
    protected override void OnHit(int damage)
    {
        // 피격 플래시
        if (_spriteRenderer != null)
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.color = Color.white;
            _spriteRenderer.DOColor(Color.red, 0.05f)
                .OnComplete(() => _spriteRenderer.DOColor(Color.white, 0.1f));
        }

        // 넉백
        if (_target != null)
        {
            Vector3 knockDir = (transform.position - _target.position).normalized;
            transform.DOMove(transform.position + knockDir * 0.3f, 0.1f);
        }

        // 데미지 텍스트
        InGameMgr.Inst.SpawnDamageText(transform.position, damage);
    }

    protected override void Die()
    {
        _isDying = true;
        base.Die();

        // 경험치 지급
        InGameMgr.Inst.InGameCharacter.AddExp(GameConfig.EXP_PER_KILL);

        // 사망 연출 후 회수
        if (_spriteRenderer != null)
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.DOFade(0f, 0.3f).OnComplete(() =>
            {
                ObjectPoolMgr.Inst.Recycle(gameObject);
            });
        }
        else
        {
            ObjectPoolMgr.Inst.Recycle(gameObject);
        }

        InGameMgr.Inst.OnEnemyKilled(this);
    }
    #endregion

    #region Pooling
    public void OnSpawn()
    {
        // Init()에서 처리
    }

    public void OnRecycle()
    {
        _isDying = false;
        _target = null;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.color = Color.white;
        }

        transform.DOKill();
    }
    #endregion
}
