using UnityEngine;

public class InGameCachedPosition : MonoBehaviour
{
    public Vector3 CharacterPos => _characterPos.position;
    [SerializeField] private Transform _characterPos;


}
