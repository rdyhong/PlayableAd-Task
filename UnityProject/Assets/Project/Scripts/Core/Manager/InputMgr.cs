using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : MonoBehaviour
{
    public static Vector3 StickDir;
    public static float StickWeight;

    public static bool IsSpawn()
    {
        return Input.GetMouseButton(0);
    }

    public static bool IsAttack()
    {
        return Input.GetMouseButton(1);
    }
}
