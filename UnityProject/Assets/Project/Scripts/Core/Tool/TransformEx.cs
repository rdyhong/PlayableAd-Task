using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RR95
{
    public static class TransformEx
    {
        /// <summary>
        /// 특정 축의 위치를 설정합니다.
        /// </summary>
        public static void SetPositionX(this Transform tf, float v)
            => tf.position = new Vector3(v, tf.position.y, tf.position.z);

        public static void SetPositionY(this Transform tf, float v)
            => tf.position = new Vector3(tf.position.x, v, tf.position.z);

        public static void SetPositionZ(this Transform tf, float v)
            => tf.position = new Vector3(tf.position.x, tf.position.y, v);

        /// <summary>
        /// 특정 축의 로컬 위치를 설정합니다.
        /// </summary>
        public static void SetLocalPositionX(this Transform tf, float v)
            => tf.localPosition = new Vector3(v, tf.localPosition.y, tf.localPosition.z);

        public static void SetLocalPositionY(this Transform tf, float v)
            => tf.localPosition = new Vector3(tf.localPosition.x, v, tf.localPosition.z);

        public static void SetLocalPositionZ(this Transform tf, float v)
            => tf.localPosition = new Vector3(tf.localPosition.x, tf.localPosition.y, v);

        public static void SetRotation(this Transform tf, Vector3 v)
            => tf.rotation = Quaternion.Euler(v.x, v.y, v.z);
        /// <summary>
        /// 특정 축의 회전 값을 설정합니다.
        /// </summary>
        public static void SetRotationX(this Transform tf, float v)
            => tf.rotation = Quaternion.Euler(v, tf.rotation.eulerAngles.y, tf.rotation.eulerAngles.z);

        public static void SetRotationY(this Transform tf, float v)
            => tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, v, tf.rotation.eulerAngles.z);

        public static void SetRotationZ(this Transform tf, float v)
            => tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, tf.rotation.eulerAngles.y, v);

        /// <summary>
        /// 특정 축의 로컬 회전 값을 설정합니다.
        /// </summary>
        public static void SetLocalRotationX(this Transform tf, float v)
            => tf.localRotation = Quaternion.Euler(v, tf.localRotation.eulerAngles.y, tf.localRotation.eulerAngles.z);

        public static void SetLocalRotationY(this Transform tf, float v)
            => tf.localRotation = Quaternion.Euler(tf.localRotation.eulerAngles.x, v, tf.localRotation.eulerAngles.z);

        public static void SetLocalRotationZ(this Transform tf, float v)
            => tf.localRotation = Quaternion.Euler(tf.localRotation.eulerAngles.x, tf.localRotation.eulerAngles.y, v);

        /// <summary>
        /// 스케일 값을 설정합니다.
        /// </summary>
        public static void SetScale(this Transform tf, float v)
            => tf.localScale = Vector3.one * v;

        /// <summary>
        /// 월드 기준으로 오브젝트를 특정 방향으로 일정 거리만큼 이동시킵니다.
        /// </summary>
        public static void Move(this Transform tf, Vector3 direction, float distance)
            => tf.position += direction.normalized * distance;

        /// <summary>
        /// 로컬 기준으로 오브젝트를 특정 방향으로 일정 거리만큼 이동시킵니다.
        /// </summary>
        public static void MoveLocal(this Transform tf, Vector3 direction, float distance)
            => tf.localPosition += direction.normalized * distance;

        /// <summary>
        /// 부모를 기준으로 로컬 위치를 리셋합니다.
        /// </summary>
        public static void ResetLocalPosition(this Transform tf)
            => tf.localPosition = Vector3.zero;

        /// <summary>
        /// 로컬 회전을 리셋합니다.
        /// </summary>
        public static void ResetLocalRotation(this Transform tf)
            => tf.localRotation = Quaternion.identity;

        /// <summary>
        /// 로컬 스케일을 (1,1,1)로 리셋합니다.
        /// </summary>
        public static void ResetLocalScale(this Transform tf)
            => tf.localScale = Vector3.one;

        /// <summary>
        /// 목표지점 방향을 가져옵니다.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector3 GetVectorTo(this Transform from, Transform to)
        {
            return to.position - from.position;
        }
        /// <summary>
        /// 정규화된 목표지점 방향을 가져옵니다.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector3 GetDirectionTo(this Transform from, Transform to)
        {
            return (to.position - from.position).normalized;
        }

        /// <summary>
        /// 특정 축을 무시하고 정규화된 목표지점 방향을 가져옵니다.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector3 GetDirectionToIgnoreY(this Transform from, Transform to)
        {
            Vector3 dir = to.position - from.position;
            return dir.NormalizedIgnoreY();
        }
        public static Vector3 GetDirectionToIgnoreZ(this Transform from, Transform to)
        {
            Vector3 dir = to.position - from.position;            
            return dir.NormalizedIgnoreZ();
        }

        /// <summary>
        /// 자식 오브젝트를 전부 제거합니다.
        /// </summary>
        public static void DestroyAllChildren(this Transform tf)
        {
            for (int i = tf.childCount - 1; i >= 0; i--)
                Object.Destroy(tf.GetChild(i).gameObject);
        }
    }
}
