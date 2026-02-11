using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameState
{
    Ready,
    Playing,
    Paused,
    GameOver
}

/// <summary>
/// 인게임 루프 — 웨이브 스폰, 적 관리, 게임 상태
/// </summary>
public class InGameMgr : Singleton<InGameMgr>
{
    [Header("State")]
    public EGameState GameState { get; private set; } = EGameState.Ready;

    [Header("Wave")]
    public int CurrentWave { get; private set; } = 0;
    public int KillCount { get; private set; } = 0;

    // 활성 적 목록
    private List<Enemy> _activeEnemies = new List<Enemy>();

    // 웨이브 코루틴
    private Coroutine _waveCoroutine;

    // 이벤트
    public event Action<EGameState> OnGameStateChanged;
    public event Action<int> OnWaveChanged;
    public event Action<int> OnKillCountChanged;

    /// <summary>
    /// 씬 진입 시 호출
    /// </summary>
    public void StartGame()
    {
        CurrentWave = 0;
        KillCount = 0;
        _activeEnemies.Clear();

        InGameCharacter.Instance?.Init();

        SetGameState(EGameState.Playing);

        _waveCoroutine = StartCoroutine(WaveLoop());
    }

    public void PauseGame()
    {
        SetGameState(EGameState.Paused);
        TimeMgr.ObjTimeScale = 0f;
    }

    public void ResumeGame()
    {
        SetGameState(EGameState.Playing);
        TimeMgr.ObjTimeScale = 1f;
    }

    private void SetGameState(EGameState state)
    {
        GameState = state;
        OnGameStateChanged?.Invoke(state);
    }

    #region Wave System
    private IEnumerator WaveLoop()
    {
        // 시작 대기
        yield return new WaitForSeconds(1f);

        while (GameState == EGameState.Playing)
        {
            CurrentWave++;
            OnWaveChanged?.Invoke(CurrentWave);

            int enemyCount = GameConfig.WAVE_BASE_ENEMY_COUNT + (CurrentWave - 1) * GameConfig.WAVE_ENEMY_INCREMENT;
            float statMul = 1f + (CurrentWave - 1) * 0.15f;

            yield return SpawnWave(enemyCount, statMul);

            // 모든 적 처치 대기
            yield return new WaitUntil(() => _activeEnemies.Count == 0 || GameState != EGameState.Playing);

            if (GameState != EGameState.Playing) break;

            // 웨이브 간 휴식
            yield return new WaitForSeconds(GameConfig.WAVE_INTERVAL);
        }
    }

    private IEnumerator SpawnWave(int count, float statMultiplier)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameState != EGameState.Playing) yield break;

            SpawnEnemy(statMultiplier);
            yield return new WaitForSeconds(GameConfig.SPAWN_INTERVAL);
        }
    }

    private void SpawnEnemy(float statMultiplier)
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        Enemy enemy = ObjectPoolMgr.Inst.Spawn<Enemy>("Prefabs/Game/Enemy");
        enemy.transform.position = spawnPos;
        enemy.Init(statMultiplier);

        _activeEnemies.Add(enemy);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 playerPos = InGameCharacter.Instance != null ? InGameCharacter.Instance.transform.position : Vector3.zero;

        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist = UnityEngine.Random.Range(GameConfig.SPAWN_RADIUS_MIN, GameConfig.SPAWN_RADIUS_MAX);

        return playerPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * dist;
    }
    #endregion

    #region Enemy Management
    public void OnEnemyKilled(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);
    }

    /// <summary>
    /// 가장 가까운 적 Transform 반환
    /// </summary>
    public Transform GetNearestEnemy(Vector3 position, float maxRange)
    {
        Transform nearest = null;
        float nearestDist = maxRange;

        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            if (_activeEnemies[i] == null || _activeEnemies[i].IsDead)
            {
                _activeEnemies.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(position, _activeEnemies[i].transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = _activeEnemies[i].transform;
            }
        }

        return nearest;
    }
    #endregion

    #region Player Death
    public void OnPlayerDead()
    {
        SetGameState(EGameState.GameOver);

        if (_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
            _waveCoroutine = null;
        }

        // TODO: GameOver UI 표시
    }
    #endregion

    #region Effects
    public void SpawnDamageText(Vector3 position, int damage)
    {
        DamageText dt = ObjectPoolMgr.Inst.Spawn<DamageText>("Prefabs/Effect/DamageText");
        dt.Play(position, damage);
    }

    public void SpawnHitEffect(Vector3 position)
    {
        HitEffect fx = ObjectPoolMgr.Inst.Spawn<HitEffect>("Prefabs/Effect/HitEffect");
        fx.Play(position);
    }
    #endregion
}
