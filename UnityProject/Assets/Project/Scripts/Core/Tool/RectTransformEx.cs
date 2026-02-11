using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RR95
{
    public static class RectTransformEx
    {
        /// <summary>
        /// 월드 좌표 상의 오브젝트의 위치로 이동합니다.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="worldCam"></param>
        /// <param name="uiCam"></param>
        /// <param name="worldTf"></param>
        /// <param name="parentRt"></param>
        public static void SetAnchoredPositionByWorldTransform(this RectTransform rt, Camera worldCam, Camera uiCam, Transform worldTf, RectTransform parentRt)
        {
            // 월드 오브젝트의 위치를 화면 좌표로 변환
            Vector3 screenPos = worldCam.WorldToScreenPoint(worldTf.position);

            // 카메라 뒤쪽에 있으면 기본값 반환
            //if (screenPos.z < 0) screenPos.z = 1;

            // UI 카메라 기준으로 스크린 좌표를 UI 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRt, screenPos, uiCam, out Vector2 uiPos);

            rt.anchoredPosition = uiPos;
        }

    }
}
