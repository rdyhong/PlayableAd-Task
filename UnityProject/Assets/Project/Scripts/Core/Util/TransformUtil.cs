using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtil
{
    public static Vector3 GetVectorTo(Transform from, Transform to)
    {
        return to.position - from.position;
    }
    public static Vector3 GetDirectionTo(Transform from, Transform to)
    {
        return (to.position - from.position).normalized;
    }

    public static float GetDistance(Transform from, Transform to)
    {
        return Vector3.Distance(from.position, to.position);
    }

    public static float GetDistance_IgnoreY(Transform from, Transform to)
    {
        Vector3 f = from.position;
        Vector3 t = to.position;
        f.y = 0;
        t.y = 0;
        return Vector3.Distance(f, t);
    }

    public static float GetDistance_IgnoreZ(Transform from, Transform to)
    {
        Vector3 f = from.position;
        Vector3 t = to.position;
        f.z = 0;
        t.z = 0;
        return Vector3.Distance(f, t);
    }
}
