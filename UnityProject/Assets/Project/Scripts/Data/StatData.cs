using System;

/// <summary>
/// 런타임 스탯 구조체
/// </summary>
[Serializable]
public class StatData
{
    public int maxHp;
    public int curHp;
    public int atk;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;

    public bool IsDead => curHp <= 0;

    public StatData() { }

    public StatData(int hp, int atk, float moveSpeed, float attackRange, float attackCooldown)
    {
        this.maxHp = hp;
        this.curHp = hp;
        this.atk = atk;
        this.moveSpeed = moveSpeed;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        curHp -= damage;
        if (curHp < 0) curHp = 0;
    }

    public void Heal(int amount)
    {
        curHp += amount;
        if (curHp > maxHp) curHp = maxHp;
    }

    public void Reset()
    {
        curHp = maxHp;
    }

    public static StatData PlayerDefault()
    {
        return new StatData(
            GameConfig.PLAYER_MAX_HP,
            GameConfig.PLAYER_ATK,
            GameConfig.PLAYER_MOVE_SPEED,
            GameConfig.PLAYER_ATTACK_RANGE,
            GameConfig.PLAYER_ATTACK_COOLDOWN
        );
    }

    public static StatData EnemyDefault()
    {
        return new StatData(
            GameConfig.ENEMY_HP,
            GameConfig.ENEMY_ATK,
            GameConfig.ENEMY_MOVE_SPEED,
            GameConfig.ENEMY_ATTACK_RANGE,
            GameConfig.ENEMY_ATTACK_COOLDOWN
        );
    }
}
