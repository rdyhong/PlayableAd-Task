using UnityEngine;

/// <summary>
/// 웨이브별 적 구성 데이터 (ScriptableObject)
/// 커스텀 웨이브 설정이 필요할 때 사용
/// </summary>
[CreateAssetMenu(fileName = "WaveData", menuName = "Game/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class WaveEntry
    {
        [Tooltip("적 프리팹 Resources 경로")]
        public string enemyPrefabPath = "Prefabs/Game/Enemy";

        [Tooltip("스폰 수")]
        public int count = 5;

        [Tooltip("스탯 배율")]
        public float statMultiplier = 1f;

        [Tooltip("스폰 간격")]
        public float spawnInterval = 0.3f;
    }

    [Header("Wave Settings")]
    public WaveEntry[] waves;

    public WaveEntry GetWave(int waveIndex)
    {
        if (waves == null || waves.Length == 0) return null;

        int idx = Mathf.Min(waveIndex, waves.Length - 1);
        return waves[idx];
    }
}
