using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InGameBoardUI : MonoBehaviour
{
    public Dictionary<Vector2Int, SlotData> SlotDict { get; private set; } = new Dictionary<Vector2Int, SlotData>();
    [SerializeField] private IngameBoardSlot[] _slots;

    public RectTransform RtCellsParent => _rtCellsParent;
    [SerializeField] private RectTransform _rtCellsParent;

    public RectTransform RtEffectParent => _rtEffectsParent;
    [SerializeField] private RectTransform _rtEffectsParent;

    [SerializeField] private Button _btnSpawn;
    [SerializeField] private Button _btnSort;

    private const int ROW_COUNT = 4;
    private const int COL_COUNT = 10;

    private const string SPAWN_EFFECT_TRAIL_PATH = "Project/Prefabs/UI/Effects/SpawnCellEffectUI";

    public void Initialize()
    {
        for(int i = 0; i < _slots.Length; i++)
        {
            Vector2Int coord = new Vector2Int(i % COL_COUNT, i / COL_COUNT);
            SlotDict.Add(coord, new SlotData(_slots[i], coord));
            SlotDict[coord].SetLock(COL_COUNT <= i);
        }

        SetUnlockableSlot();

        _btnSpawn.onClick.AddListener(OnClickSpawn);
        _btnSort.onClick.AddListener(OnClickSort);
    }

    private void OnClickSpawn()
    {
        SlotData emptySlot = null;

        bool breakFlag = false;

        for (int y = 0; y < ROW_COUNT; y++)
        {
            for(int x = 0; x < COL_COUNT; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);

                if (SlotDict[coord].IsLock)
                {
                    breakFlag = true;
                }
                else if( SlotDict[coord].SlotCell == null)
                {
                    emptySlot = SlotDict[coord];
                    breakFlag = true;
                }

                if (breakFlag) break;
            }

            if (breakFlag) break;
        }

        if(emptySlot != null)
        {
            emptySlot.CreateSlotCell();

            EffectBase eff = ObjectPoolMgr.Inst.Spawn<EffectBase>(SPAWN_EFFECT_TRAIL_PATH);
            eff.Play(emptySlot.BoardSlot.Rt.anchoredPosition3D, true);
        }
    }

    public void OnClickSort()
    {
        // 1. 셀이 있는 슬롯의 grade 수집
        List<int> grades = new List<int>();

        for (int y = 0; y < ROW_COUNT; y++)
        {
            for (int x = 0; x < COL_COUNT; x++)
            {
                var slot = SlotDict[new Vector2Int(x, y)];
                if (!slot.IsLock && slot.SlotCell != null)
                {
                    grades.Add(slot.Grade);
                    slot.ClearSlot();
                }
            }
        }

        // 2. grade 내림차순 정렬
        grades.Sort((a, b) => b.CompareTo(a));

        // 3. 왼쪽 위부터 순서대로 재배치
        int idx = 0;
        for (int y = 0; y < ROW_COUNT && idx < grades.Count; y++)
        {
            for (int x = 0; x < COL_COUNT && idx < grades.Count; x++)
            {
                var slot = SlotDict[new Vector2Int(x, y)];
                if (slot.IsLock) continue;

                slot.SetSlotCell(grades[idx]);
                idx++;
            }
        }
    }

    public void SetUnlockableSlot()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            Vector2Int coord = new Vector2Int(i % COL_COUNT, i / COL_COUNT);
            if(SlotDict[coord].IsLock)
            {
                SlotDict[coord].SetUnlockable();
                break;
            }
        }
    }
}


public class SlotData
{
    public IngameBoardSlot BoardSlot { get; private set; } = null;
    public IngameBoardSlotCell SlotCell { get; private set; }
    public Vector2Int Coord { get; private set; } = Vector2Int.zero;
    public int Grade { get; private set; } = 0;
    public bool IsLock { get; private set; } = false;

    public static int HighstGrade = 1;

    public SlotData(IngameBoardSlot boardSlot, Vector2Int coord)
    {
        BoardSlot = boardSlot;
        Coord = coord;
        BoardSlot.Initialize(this);
    }

    public void SetLock(bool isLock)
    {
        IsLock  = isLock;
        BoardSlot.SetLock(isLock);
    }

    public void SetUnlockable()
    {
        IsLock = true;
        BoardSlot.SetUnlockable();
    }

    public void Unclock()
    {
        IsLock = false;
        BoardSlot.SetLock(false);
        InGameMain.Inst.MainUI.InGameUI.BoardUI.SetUnlockableSlot();
    }

    public void ClearSlot()
    {
        if (SlotCell != null)
        {
            ObjectPoolMgr.Inst.Recycle(SlotCell.gameObject);
            SlotCell = null;
        }

        Grade = 0;

        BoardSlot.ClearSlot();
    }

    public void CreateSlotCell()
    {
        if (SlotCell != null) ObjectPoolMgr.Inst.Recycle(SlotCell.gameObject);
        
        Grade = 1;
        BoardSlot.CreateSlotCell(Grade);
        
        IngameBoardSlotCell cell = ObjectPoolMgr.Inst.Spawn<IngameBoardSlotCell>("Project/Prefabs/UI/IngameBoardSlotCell");
        cell.Initialize();
        cell.Rt.SetParent(InGameMain.Inst.MainUI.InGameUI.BoardUI.RtCellsParent, false);
        cell.Rt.localScale = Vector3.one;
        cell.Rt.anchoredPosition3D = BoardSlot.Rt.anchoredPosition3D;
        SlotCell = cell;
    }

    public void SetSlotCell(int grade)
    {
        if (SlotCell != null) ObjectPoolMgr.Inst.Recycle(SlotCell.gameObject);

        Grade = grade;
        BoardSlot.CreateSlotCell(Grade);

        IngameBoardSlotCell cell = ObjectPoolMgr.Inst.Spawn<IngameBoardSlotCell>("Project/Prefabs/UI/IngameBoardSlotCell");
        cell.Initialize();
        cell.Rt.SetParent(InGameMain.Inst.MainUI.InGameUI.BoardUI.RtCellsParent, false);
        cell.Rt.localScale = Vector3.one;
        cell.Rt.anchoredPosition3D = BoardSlot.Rt.anchoredPosition3D;
        SlotCell = cell;
    }

    public void Upgrade()
    {
        if (SlotCell != null) ObjectPoolMgr.Inst.Recycle(SlotCell.gameObject);
        SlotCell = ObjectPoolMgr.Inst.Spawn<IngameBoardSlotCell>("Project/Prefabs/UI/IngameBoardSlotCell");
        SlotCell.Initialize();
        SlotCell.Rt.SetParent(InGameMain.Inst.MainUI.InGameUI.BoardUI.RtCellsParent, false);
        SlotCell.Rt.localScale = Vector3.one;
        SlotCell.Rt.anchoredPosition3D = BoardSlot.Rt.anchoredPosition3D;
        Grade++;

        if(HighstGrade < Grade) HighstGrade = Grade;

        BoardSlot.UpgradeSlot(Grade);

        EffectBase eff = ObjectPoolMgr.Inst.Spawn<EffectBase>("Project/Prefabs/UI/Effects/MergeEffectUI");
        eff.Play(SlotCell.Rt.anchoredPosition3D, true);
    }
}

