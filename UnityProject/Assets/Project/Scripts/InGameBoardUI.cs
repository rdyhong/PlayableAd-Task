using System.Collections.Generic;
using UnityEngine;

public class InGameBoardUI : MonoBehaviour
{
    public Dictionary<Vector2Int, IngameBoardSlot> SlotDict { get; private set; } = new Dictionary<Vector2Int, IngameBoardSlot>();
    [SerializeField] private IngameBoardSlot[] _slots;

    private const int ROW_COUNT = 4;
    private const int COL_COUNT = 10;

    public void Initialize()
    {
        for(int i = 0; i < _slots.Length; i++)
        {
            Vector2Int coord = new Vector2Int(i % COL_COUNT, i / COL_COUNT);
            _slots[i].Initialize(coord);
            SlotDict.Add(coord, _slots[i]);
        }
    }
}
