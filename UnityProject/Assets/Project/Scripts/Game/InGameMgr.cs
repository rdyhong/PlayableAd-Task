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
    public int CurrentMiniWave { get; private set; } = 0;
    public int KillCount { get; private set; } = 0;

    // 웨이브 코루틴
    private Coroutine _waveCoroutine;

    public void Initialize()
    {

    }

    /// <summary>
    /// 씬 진입 시 호출
    /// </summary>
    public void StartGame()
    {
        CurrentWave = 0;
        KillCount = 0;

        SetGameState(EGameState.Playing);

        CharacterMgr.Inst.Initialize();

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
    }

    #region Wave System
    private IEnumerator WaveLoop()
    {
        // 시작 대기
        yield return new WaitForSeconds(1f);

        while (GameState == EGameState.Playing)
        {
            CurrentWave++;
            CurrentMiniWave = 0;

            while (CurrentMiniWave <= 3)
            {
                CurrentMiniWave++;

                int enemyCount = GameConfig.WAVE_BASE_ENEMY_COUNT + (CurrentWave - 1) * GameConfig.WAVE_ENEMY_INCREMENT;
                float statMul = 1f + (CurrentWave - 1) * 0.15f;

                yield return SpawnWave(enemyCount, statMul);

                yield return new WaitForSeconds(GameConfig.MINIWAVE_INTERVAL);

                // 모든 적 처치 대기
                yield return new WaitUntil(() => MonsterMgr.Inst.SpawnedMonsterList.Count == 0 || GameState != EGameState.Playing);
            }

            // 웨이브 간 휴식
            yield return new WaitForSeconds(GameConfig.WAVE_INTERVAL);
        }
    }

    private IEnumerator SpawnWave(int count, float statMultiplier)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameState != EGameState.Playing) yield break;

            MonsterMgr.Inst.SpawnMonster();
            yield return new WaitForSeconds(GameConfig.SPAWN_INTERVAL);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 playerPos = Vector3.zero;

        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist = UnityEngine.Random.Range(GameConfig.SPAWN_RADIUS_MIN, GameConfig.SPAWN_RADIUS_MAX);

        return playerPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * dist;
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
        EffectBase fx = ObjectPoolMgr.Inst.Spawn<EffectBase>("Prefabs/Effect/HitEffect");
        fx.Play(position);
    }
    #endregion
}
