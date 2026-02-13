using System.Collections.Generic;
using UnityEngine;

public class CharacterMgr : Singleton<CharacterMgr>
{
    public InGameCharacter InGameCharacter { get; private set; } = null;

    private const string InGameCharacterPath = "Project/Prefabs/Character/InGameCharacter";

    public void Initialize()
    {
        InGameCharacter = ObjectPoolMgr.Inst.Spawn<InGameCharacter>(InGameCharacterPath);
        InGameCharacter.Initialize();
        InGameMain.Inst.CamController.SetCameraTarget(InGameCharacter.transform);

    }
}
