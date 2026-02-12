using UnityEngine;

/// <summary>
/// 전역 게임 상수
/// </summary>
public static class GameConfig
{
    // 스폰
    public const float SPAWN_RADIUS_MIN = 0f;
    public const float SPAWN_RADIUS_MAX = 8f;
    public const float SPAWN_INTERVAL = 0.3f;

    // 플레이어
    public const float PLAYER_MOVE_SPEED = 5f;
    public const float PLAYER_ATTACK_COOLDOWN = 0.15f;
    public const float PLAYER_ATTACK_RANGE = 6f;
    public const int PLAYER_MAX_HP = 100;
    public const int PLAYER_ATK = 10;

    // 적
    public const float ENEMY_MOVE_SPEED = 2f;
    public const int ENEMY_HP = 30;
    public const int ENEMY_ATK = 5;
    public const float ENEMY_ATTACK_RANGE = 0.8f;
    public const float ENEMY_ATTACK_COOLDOWN = 1f;

    // 투사체
    public const float PROJECTILE_SPEED = 15f;
    public const float PROJECTILE_LIFETIME = 0.5f;

    // 이펙트
    public const float DAMAGE_TEXT_DURATION = 0.8f;
    public const float DAMAGE_TEXT_RISE_HEIGHT = 1.5f;
    public const float HIT_EFFECT_DURATION = 0.3f;

    // 카메라
    public const float CAMERA_FOLLOW_SPEED = 8f;
    public static readonly Vector3 CAMERA_OFFSET = new Vector3(0, 0, -10f);

    // 웨이브
    public const float WAVE_INTERVAL = 3f;
    public const int WAVE_BASE_ENEMY_COUNT = 5;
    public const int WAVE_ENEMY_INCREMENT = 3;

    // 경험치 / 레벨업
    public const int EXP_PER_KILL = 10;
    public const int EXP_BASE_REQUIRED = 50;
    public const float EXP_LEVEL_MULTIPLIER = 1.3f;
}
