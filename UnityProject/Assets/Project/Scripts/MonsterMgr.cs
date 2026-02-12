using System.Collections.Generic;
using UnityEngine;

public class MonsterMgr : Singleton<MonsterMgr>
{
    public List<EntityBase> SpawnedMonster { get; private set; } = new List<EntityBase>();

    public void Initialize()
    {
        SpawnedMonster.Clear();


    }


}
