using UnityEngine;

public enum EAnimType
{
    Idle,
    Walk,
    Attack,
    Hit,
    Death
}

public class AnimController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void Initialize()
    {

    }

    public void PlayAnim(EAnimType animType)
    {
        string animName = string.Empty;

        switch(animType)
        {
            case EAnimType.Idle:
                animName = "Idle";
                break;
            case EAnimType.Walk:
                animName = "Walk";
                break;
            case EAnimType.Attack:
                animName = "Attack";
                break;
            case EAnimType.Hit:
                animName = "Hit";
                break;
            case EAnimType.Death:
                animName = "Death";
                break;
            default:
                animName = "Idle";
                break;
        }

        _animator.Play(animName);
    }
}
