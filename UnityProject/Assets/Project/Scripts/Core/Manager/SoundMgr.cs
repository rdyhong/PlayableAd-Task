using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : Singleton<SoundMgr>
{
    public BGMController BGMController { get; private set; }
    public SFXController SFXController { get; private set; }

    public void Initialize()
    {
        if (BGMController == null)
        {
            string path = "Prefabs/Sound/BGMController";
            BGMController = Instantiate(Resources.Load(path) as GameObject).GetComponent<BGMController>();
            BGMController.transform.SetParent(transform, false);
        }

        if (SFXController == null)
        {
            string path = "Prefabs/Sound/SFXController";
            SFXController = Instantiate(Resources.Load(path) as GameObject).GetComponent<SFXController>();
            SFXController.transform.SetParent(transform, false);
        }

        BGMController.Initialize();
        SFXController.Initialize();
    }
}
