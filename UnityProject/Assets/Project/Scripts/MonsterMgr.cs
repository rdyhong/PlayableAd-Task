using System.Collections.Generic;
using UnityEngine;

public class MonsterMgr : Singleton<MonsterMgr>
{
    public List<Monster> SpawnedMonsterList { get; private set; } = new List<Monster>();

    private const string MonsterPath = "Project/Prefabs/Monster/";

    public void Initialize()
    {
        SpawnedMonsterList.Clear();


    }

    public void SpawnMonster()
    {
        Monster monster = ObjectPoolMgr.Inst.Spawn<Monster>($"{MonsterPath}Monster_{Random.Range(0, 5).ToString()}");
        monster.Initialize();

        Vector3 spawnPos = CharacterMgr.Inst.InGameCharacter.transform.position + new Vector3(5f, 5f, 0f);
        monster.transform.position = GetRandomSpawnPosition();

        SpawnedMonsterList.Add(monster);
    }

    public void OnMonsterDead(Monster m)
    {
        SpawnedMonsterList.Remove(m);
    }

    public Monster GetNearestMonster(Vector3 fromPos, float range)
    {
        Monster nearest = null;
        float minDistX = float.MaxValue;

        for (int i = 0; i < SpawnedMonsterList.Count; i++)
        {
            float distX = Mathf.Abs(SpawnedMonsterList[i].transform.position.x - fromPos.x);

            if (distX < minDistX && distX <= range)
            {
                minDistX = distX;
                nearest = SpawnedMonsterList[i];
            }
        }

        return nearest;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 targetPos = CharacterMgr.Inst.InGameCharacter.transform.position + new Vector3(20f, Random.Range(4f, 12f), 0f);

        return targetPos;
    }
}
