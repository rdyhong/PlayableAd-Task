using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameBoardUI : MonoBehaviour
{
    public Dictionary<Vector2Int, SlotData> SlotDict { get; private set; } = new Dictionary<Vector2Int, SlotData>();
    [SerializeField] private IngameBoardSlot[] _slots;

    public RectTransform RtCellsParent => _rtCellsParent;
    [SerializeField] private RectTransform _rtCellsParent;

    [SerializeField] private Button _btnSpawn;

    private const int ROW_COUNT = 4;
    private const int COL_COUNT = 10;

    public void Initialize()
    {
        for(int i = 0; i < _slots.Length; i++)
        {
            Vector2Int coord = new Vector2Int(i % COL_COUNT, i / COL_COUNT);
            SlotDict.Add(coord, new SlotData(_slots[i], coord, 1));
        }

        _btnSpawn.onClick.AddListener(OnClickSpawn);
    }

    private void OnClickSpawn()
    {
        SlotData emptySlot = null;

        for (int y = 0; y < ROW_COUNT; y++)
        {
            for(int x = 0; x < COL_COUNT; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);

                if (SlotDict[coord].SlotCell == null)
                {
                    emptySlot = SlotDict[coord];
                    break;
                }
            }

            if (emptySlot != null) break;
        }

        if(emptySlot != null)
        {
            emptySlot.CreateSlotCell();
        }
    }
}
