using UnityEngine;

/// <summary>
/// 투사체 — 직선 이동, 적 충돌 시 데미지, 풀링 연동
/// </summary>
public class Projectile : MonoBehaviour, IPoolingObject
{
    private Vector3 _direction;
    private float _speed;
    private int _damage;
    private float _lifeTimer;
    private bool _isActive;

    // 관통 여부 (추후 확장)
    [SerializeField] private bool _isPiercing = false;

    public void Init(Vector3 startPos, Vector3 direction, int damage)
    {
        transform.position = startPos;
        _direction = direction.normalized;
        _speed = GameConfig.PROJECTILE_SPEED;
        _damage = damage;
        _lifeTimer = GameConfig.PROJECTILE_LIFETIME;
        _isActive = true;

        // 회전 — 발사 방향으로
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        if (!_isActive) return;

        transform.position += _direction * _speed * TimeMgr.ObjDeltaTime;

        _lifeTimer -= TimeMgr.ObjDeltaTime;
        if (_lifeTimer <= 0f)
        {
            Recycle();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;

        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(_damage);

                // 히트 이펙트
                InGameMgr.Inst.SpawnHitEffect(other.ClosestPoint(transform.position));

                if (!_isPiercing)
                {
                    Recycle();
                }
            }
        }
    }

    private void Recycle()
    {
        _isActive = false;
        ObjectPoolMgr.Inst.Recycle(gameObject);
    }

    public void OnSpawn()
    {
        _isActive = false;
    }

    public void OnRecycle()
    {
        _isActive = false;
    }
}
