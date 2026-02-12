using UnityEngine;

public class IngameBoardSlot : MonoBehaviour
{
    public Vector2Int Coord { get; private set; }

    public void Initialize(Vector2Int coord)
    {
        Coord = coord;
    }
}
