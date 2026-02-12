using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class SlotData
{
    public IngameBoardSlot BoardSlot { get; private set; } = null;
    public IngameBoardSlotCell SlotCell { get; private set; }
    public Vector2Int Coord { get; private set; } = Vector2Int.zero;
    public int Grade { get; private set; } = 0;

    public SlotData(IngameBoardSlot boardSlot, Vector2Int coord, int grade)
    {
        BoardSlot = boardSlot;
        Coord = coord;
        Grade = grade;
        BoardSlot.Initialize(this);
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
        BoardSlot.UpgradeSlot(Grade);
    }
}

public class IngameBoardSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI _txtGrade;

    private SlotData _cachedSlotData;

    public RectTransform Rt { get; private set; }

    private float _grabScale = 1.2f;
    private float _scaleDuration = 0.1f;
    private float _returnDuration = 0.2f;

    public void Initialize(SlotData slotData)
    {
        Rt = GetComponent<RectTransform>();
        _cachedSlotData = slotData;
        _txtGrade.text = string.Empty;
    }

    public void CreateSlotCell(int grade)
    {
        _txtGrade.text = grade.ToString();
    }

    public void ClearSlot()
    {
        _txtGrade.text = string.Empty;
    }

    public void UpgradeSlot(int grade)
    {
        _txtGrade.text = grade.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_cachedSlotData?.SlotCell == null) return;

        _txtGrade.text = string.Empty;
        _cachedSlotData.SlotCell.Rt.DOKill();
        _cachedSlotData.SlotCell.Rt.DOScale(_grabScale, _scaleDuration).SetEase(Ease.OutBack);
        _cachedSlotData.SlotCell.Rt.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_cachedSlotData?.SlotCell == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _cachedSlotData.SlotCell.Rt.parent as RectTransform,
            eventData.position,
            InGameMain.Inst.MainUI.Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : InGameMain.Inst.MainUI.Canvas.worldCamera,
            out Vector2 localPoint);

        _cachedSlotData.SlotCell.Rt.localPosition = localPoint;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_cachedSlotData?.SlotCell == null) return;

        // 드롭 위치에서 다른 슬롯 찾기
        IngameBoardSlot targetSlot = FindSlotUnderPointer(eventData);

        if (targetSlot != null
            && targetSlot != this
            && targetSlot._cachedSlotData?.SlotCell != null
            && targetSlot._cachedSlotData.Grade == _cachedSlotData.Grade)
        {
            // 합성: 드래그한 슬롯 제거 → 타겟 슬롯 업그레이드
            _cachedSlotData.ClearSlot();
            targetSlot._cachedSlotData.Upgrade();
        }
        else
        {
            // 원래 위치로 복귀
            _txtGrade.text = _cachedSlotData.Grade.ToString();
            _cachedSlotData.SlotCell.Rt.DOKill();
            _cachedSlotData.SlotCell.Rt.DOLocalMove(Rt.anchoredPosition3D, _returnDuration).SetEase(Ease.OutBack);
            _cachedSlotData.SlotCell.Rt.DOScale(1f, _returnDuration).SetEase(Ease.OutBack);
        }
    }

    private IngameBoardSlot FindSlotUnderPointer(PointerEventData eventData)
    {
        Camera cam = InGameMain.Inst.MainUI.Canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : InGameMain.Inst.MainUI.Canvas.worldCamera;

        foreach (var pair in InGameMain.Inst.MainUI.InGameUI.BoardUI.SlotDict)
        {
            if (pair.Value.BoardSlot == this) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(
                pair.Value.BoardSlot.Rt, eventData.position, cam))
            {
                return pair.Value.BoardSlot;
            }
        }

        return null;
    }
}
