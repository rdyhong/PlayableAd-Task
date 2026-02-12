using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Rendering;

public class IngameBoardSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float _grabScale = 1.2f;
    [SerializeField] private float _scaleDuration = 0.1f;
    [SerializeField] private float _returnDuration = 0.2f;

    public IngameBoardSlotCell SlotCell { get; private set; }
    public RectTransform Rt { get; private set; }

    public Vector2Int Coord { get; private set; }

    private Canvas _canvas;

    public void Initialize(Vector2Int coord)
    {
        Rt = GetComponent<RectTransform>();

        Coord = coord;
        
        CreateSlotCell();
    }

    public void CreateSlotCell()
    {
        IngameBoardSlotCell cell = ObjectPoolMgr.Inst.Spawn<IngameBoardSlotCell>("Project/Prefabs/UI/IngameBoardSlotCell");
        cell.Initialize();
        cell.Rt.SetParent(InGameMain.Inst.MainUI.InGameUI.BoardUI.RtCellsParent, false);
        cell.Rt.localScale = Vector3.one;
        cell.Rt.anchoredPosition3D = Rt.anchoredPosition3D;
        SlotCell = cell;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SlotCell.Rt == null) return;

        // 캔버스 캐싱 (스크린→로컬 변환용)
        if (_canvas == null)
            _canvas = GetComponentInParent<Canvas>();

        SlotCell.Rt.DOKill();
        SlotCell.Rt.DOScale(_grabScale, _scaleDuration).SetEase(Ease.OutBack);
        SlotCell.Rt.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SlotCell.Rt == null || _canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            SlotCell.Rt.parent as RectTransform,
            eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPoint);

        SlotCell.Rt.localPosition = localPoint;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (SlotCell.Rt == null) return;

        SlotCell.Rt.DOKill();
        SlotCell.Rt.DOLocalMove(Rt.anchoredPosition3D, _returnDuration).SetEase(Ease.OutBack);
        SlotCell.Rt.DOScale(1f, _returnDuration).SetEase(Ease.OutBack);
    }
}
