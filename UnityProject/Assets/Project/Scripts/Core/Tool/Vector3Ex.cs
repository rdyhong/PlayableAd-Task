using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RR95
{
    public static class Vector3Ex
    {
        /// <summary>
        /// 해당 축의 값을 0으로 변환합니다.
        /// </summary>
        /// <param name="v"></param>
        public static Vector3 FlattenX(this Vector3 v)
        {
            v.x = 0;
            return v;
        }
        public static Vector3 FlattenY(this Vector3 v)
        {
            v.y = 0;
            return v;
        }
        public static Vector3 FlattenZ(this Vector3 v)
        {
            v.z = 0;
            return v;
        }

        /// <summary>
        /// 길이를 1로 정규화 합니다.
        /// </summary>
        /// <param name="v"></param>
        public static Vector3 Normalized(this Vector3 v)
        {
            v = v.normalized;
            return v;
        }
        public static Vector3 NormalizedIgnoreY(this Vector3 v)
        {
            v.y = 0;
            v = v.normalized;
            return v;
        }
        public static Vector3 NormalizedIgnoreZ(this Vector3 v)
        {
            v.z = 0;
            v = v.normalized;
            return v;
        }

        /// <summary>
        /// 특정 축을 원하는 값으로 설정합니다.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="x"></param>
        public static Vector3 SetX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }

        public static Vector3 SetY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        public static Vector3 SetZ(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }

        /// <summary>
        /// 특정 축에 값을 추가합니다.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="x"></param>
        public static Vector3 AddX(this Vector3 v, float x)
        {
            v.x += x;
            return v;
        }

        public static Vector3 AddY(this Vector3 v, float y)
        {
            v.y += y;
            return v;
        }

        public static Vector3 AddZ(this Vector3 v, float z)
        {
            v.z += z;
            return v;
        }

        /// <summary>
        /// 현재 방향 기준으로 각도 내 랜덤 방향 반환
        /// </summary>
        /// <param name="curDir">현재 각도</param>
        /// <param name="angle">랜덤 최대 각도</param>
        /// <returns></returns>
        public static Vector3 GetRandomizedDirection(this Vector3 curDir, float angle)
        {
            Vector3 randomAxis = Random.onUnitSphere;
            float randomAngle = Random.Range(-angle, angle);
            Quaternion randomRotation = Quaternion.AngleAxis(randomAngle, randomAxis);
            Vector3 randomizedDirection = randomRotation * curDir;
            return randomizedDirection;
        }
    }
}
