using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Rendering;
using TMPro;

public class SlotData
{
    public int Grade { get; private set; } = 0;


}

public class IngameBoardSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI _txtGrade;

    public SlotData SlotData { get; private set; } = null;


    public IngameBoardSlotCell SlotCell { get; private set; }
    public RectTransform Rt { get; private set; }

    public Vector2Int Coord { get; private set; }

    //private Canvas _canvas;

    private float _grabScale = 1.2f;
    private float _scaleDuration = 0.1f;
    private float _returnDuration = 0.2f;

    public void Initialize(Vector2Int coord)
    {
        Rt = GetComponent<RectTransform>();
        Coord = coord;
        SlotData = new SlotData();
        _txtGrade.text = string.Empty;

        CreateSlotCell(new SlotData());
    }

    public void CreateSlotCell(SlotData data)
    {
        SlotData = data;

        IngameBoardSlotCell cell = ObjectPoolMgr.Inst.Spawn<IngameBoardSlotCell>("Project/Prefabs/UI/IngameBoardSlotCell");
        cell.Initialize();
        cell.Rt.SetParent(InGameMain.Inst.MainUI.InGameUI.BoardUI.RtCellsParent, false);
        cell.Rt.localScale = Vector3.one;
        cell.Rt.anchoredPosition3D = Rt.anchoredPosition3D;

        SlotCell = cell;

        _txtGrade.text = SlotData.Grade.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SlotCell.Rt == null) return;

        _txtGrade.text = string.Empty;

        SlotCell.Rt.DOKill();
        SlotCell.Rt.DOScale(_grabScale, _scaleDuration).SetEase(Ease.OutBack);
        SlotCell.Rt.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SlotData == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            SlotCell.Rt.parent as RectTransform,
            eventData.position,
            InGameMain.Inst.MainUI.Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : InGameMain.Inst.MainUI.Canvas.worldCamera,
            out Vector2 localPoint);

        SlotCell.Rt.localPosition = localPoint;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (SlotCell.Rt == null) return;

        _txtGrade.text = SlotData.Grade.ToString();

        SlotCell.Rt.DOKill();
        SlotCell.Rt.DOLocalMove(Rt.anchoredPosition3D, _returnDuration).SetEase(Ease.OutBack);
        SlotCell.Rt.DOScale(1f, _returnDuration).SetEase(Ease.OutBack);
    }
}
