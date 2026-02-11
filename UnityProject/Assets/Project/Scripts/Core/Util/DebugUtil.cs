using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtil
{
    public static void Log(string log)
    {
#if LOG_ENABLE
        Debug.Log(log);
#endif
    }
    public static void LogWarn(string log)
    {
#if LOG_ENABLE
        Debug.LogWarning(log);
#endif
    }
    public static void LogErr(string log)
    {
#if LOG_ENABLE
        Debug.LogError(log);
#endif
    }
    public static void LogAssert(bool condition, string log)
    {
#if LOG_ENABLE
        Debug.Assert(condition, log);
        if (!condition)
        {
            throw new System.Exception(log);
        }
#endif
    }
}
