using UnityEngine;
using Unity.Cinemachine;

public static class UIUtil
{
    /// <summary>
    /// 월드 오브젝트의 UI상 위치를 리턴 (화면 밖으로 나가지 않도록 보정 포함)
    /// 시네머신 카메라 버전
    /// </summary>
    /// <param name="cinemachineBrain">시네머신 브레인이 있는 카메라</param>
    /// <param name="uiCam">UI를 렌더링하는 카메라</param>
    /// <param name="worldTf">월드 오브젝트의 Transform</param>
    /// <param name="parentRt">UI가 속한 부모 RectTransform</param>
    /// <returns>UI상 위치</returns>
    public static Vector2 GetUIPosFromWorldTf(CinemachineBrain cinemachineBrain, Camera uiCam, Transform worldTf, RectTransform parentRt)
    {
        // 시네머신 브레인에서 실제 카메라를 가져옴
        Camera worldCam = cinemachineBrain.OutputCamera;

        // 월드 오브젝트의 위치를 화면 좌표로 변환
        Vector3 screenPos = worldCam.WorldToScreenPoint(worldTf.position);

        // 카메라 뒤쪽에 있으면 처리 (주석 해제시 사용)
        //if (screenPos.z < 0) screenPos.z = 1;

        // UI 카메라 기준으로 스크린 좌표를 UI 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRt, screenPos, uiCam, out Vector2 uiPos);

        return uiPos;
    }

    /// <summary>
    /// 월드 오브젝트의 UI상 위치를 리턴 (가상 카메라를 직접 사용하는 버전)
    /// </summary>
    /// <param name="virtualCamera">시네머신 카메라 (Cinemachine 3.x)</param>
    /// <param name="uiCam">UI를 렌더링하는 카메라</param>
    /// <param name="worldTf">월드 오브젝트의 Transform</param>
    /// <param name="parentRt">UI가 속한 부모 RectTransform</param>
    /// <returns>UI상 위치</returns>
    public static Vector2 GetUIPosFromWorldTf(CinemachineCamera virtualCamera, Camera uiCam, Transform worldTf, RectTransform parentRt)
    {
        // 가상 카메라의 상태를 가져와서 실제 카메라 위치와 회전을 계산
        CameraState state = virtualCamera.State;

        // 임시 카메라 설정으로 월드 좌표를 화면 좌표로 변환
        Vector3 cameraPos = state.GetFinalPosition();
        Quaternion cameraRot = state.GetFinalOrientation();

        // 가상 카메라의 뷰 매트릭스와 프로젝션 매트릭스를 사용하여 변환
        Matrix4x4 viewMatrix = Matrix4x4.TRS(cameraPos, cameraRot, Vector3.one).inverse;
        Matrix4x4 projMatrix = Matrix4x4.Perspective(
            state.Lens.FieldOfView,
            (float)Screen.width / Screen.height,
            state.Lens.NearClipPlane,
            state.Lens.FarClipPlane);

        Vector3 worldPos = worldTf.position;
        Vector4 clipPos = projMatrix * (viewMatrix * new Vector4(worldPos.x, worldPos.y, worldPos.z, 1.0f));

        if (clipPos.w > 0)
        {
            Vector3 ndcPos = new Vector3(clipPos.x / clipPos.w, clipPos.y / clipPos.w, clipPos.z / clipPos.w);
            Vector2 screenPos = new Vector2(
                (ndcPos.x + 1.0f) * 0.5f * Screen.width,
                (ndcPos.y + 1.0f) * 0.5f * Screen.height
            );

            // UI 카메라 기준으로 스크린 좌표를 UI 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRt, screenPos, uiCam, out Vector2 uiPos);

            return uiPos;
        }

        return Vector2.zero;
    }

    /// <summary>
    /// 간단한 버전: 현재 활성화된 시네머신 카메라를 자동으로 찾아서 사용
    /// </summary>
    /// <param name="uiCam">UI를 렌더링하는 카메라</param>
    /// <param name="worldTf">월드 오브젝트의 Transform</param>
    /// <param name="parentRt">UI가 속한 부모 RectTransform</param>
    /// <returns>UI상 위치</returns>
    public static Vector2 GetUIPosFromWorldTf(Camera uiCam, Transform worldTf, RectTransform parentRt)
    {
        // 시네머신 브레인을 찾아서 사용
        CinemachineBrain brain = Object.FindAnyObjectByType<CinemachineBrain>();
        if (brain == null)
        {
            Debug.LogWarning("CinemachineBrain을 찾을 수 없습니다.");
            return Vector2.zero;
        }

        return GetUIPosFromWorldTf(brain, uiCam, worldTf, parentRt);
    }

    /// <summary>
    /// UI 요소가 화면 밖으로 나가지 않도록 보정
    /// </summary>
    public static Vector2 ClampToScreen(RectTransform parentRt, Vector2 uiPos, Vector2 padding)
    {
        Vector3[] corners = new Vector3[4];
        parentRt.GetWorldCorners(corners);

        // 부모 RectTransform의 크기 가져오기
        Vector2 min = parentRt.rect.min;
        Vector2 max = parentRt.rect.max;

        // UI 좌표 보정
        if (uiPos.x < min.x) uiPos.x = min.x + padding.x;
        if (uiPos.x > max.x) uiPos.x = max.x - padding.x;
        if (uiPos.y < min.y) uiPos.y = min.y + padding.y;
        if (uiPos.y > max.y) uiPos.y = max.y - padding.y;

        return uiPos;
    }

    /// <summary>
    /// UI 위치가 화면 밖인지 확인
    /// </summary>
    /// <param name="parentRt">부모 RectTransform</param>
    /// <param name="uiPos">UI 위치</param>
    /// <returns>화면 안에 있으면 true</returns>
    public static bool IsInScreen(RectTransform parentRt, Vector2 uiPos)
    {
        Vector3[] corners = new Vector3[4];
        parentRt.GetWorldCorners(corners);

        // 부모 RectTransform의 크기 가져오기
        Vector2 min = parentRt.rect.min;
        Vector2 max = parentRt.rect.max;

        // UI 좌표 확인
        if (uiPos.x < min.x) return false;
        if (uiPos.x > max.x) return false;
        if (uiPos.y < min.y) return false;
        if (uiPos.y > max.y) return false;

        return true;
    }
}