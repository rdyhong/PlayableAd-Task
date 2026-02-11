using UnityEngine;

/// <summary>
/// 플레이어 추적 카메라 (2D)
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private Transform _target;

    [SerializeField] private Vector3 _offset = GameConfig.CAMERA_OFFSET;
    [SerializeField] private float _followSpeed = GameConfig.CAMERA_FOLLOW_SPEED;

    private void LateUpdate()
    {
        if (_target == null)
        {
            if (InGameCharacter.Instance != null)
                _target = InGameCharacter.Instance.transform;
            else
                return;
        }

        Vector3 targetPos = _target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, _followSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
