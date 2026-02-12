using Unity.Cinemachine;
using UnityEngine;

public class InGameCamController : MonoBehaviour
{
    public CinemachineCamera IngameCam => _ingameCam;
    [SerializeField] private CinemachineCamera _ingameCam;

    public void Initialize()
    {
        
    }

    public void SetCameraTarget(Transform tf)
    {
        IngameCam.Target.TrackingTarget = tf;
    }
}
