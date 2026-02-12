using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class IngameBoardSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI _txtGrade;

    [SerializeField] private GameObject _lockGo;
    [SerializeField] private Button _btnUnlock;

    private SlotData _cachedSlotData;

    public RectTransform Rt { get; private set; }

    private float _grabScale = 1.2f;
    private float _scaleDuration = 0.1f;
    private float _returnDuration = 0.2f;

    private float _lastClickTime;
    private const float DOUBLE_CLICK_THRESHOLD = 0.3f;

    public void Initialize(SlotData slotData)
    {
        Rt = GetComponent<RectTransform>();
        _btnUnlock.onClick.AddListener(OnClickUnlock);

        _cachedSlotData = slotData;
        _txtGrade.text = string.Empty;
    }

    private void OnClickUnlock()
    {
        _cachedSlotData.Unclock();
    }

    public void SetLock(bool isLock)
    {
        _lockGo.SetActive(isLock);
        _btnUnlock.gameObject.SetActive(false);
    }

    public void SetUnlockable()
    {
        _lockGo.SetActive(false);
        _btnUnlock.gameObject.SetActive(true);
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

        float now = Time.unscaledTime;
        if (now - _lastClickTime < DOUBLE_CLICK_THRESHOLD)
        {
            _lastClickTime = 0f;
            TryDoubleClickMerge();
            return;
        }
        _lastClickTime = now;

        _txtGrade.text = string.Empty;
        _cachedSlotData.SlotCell.Rt.DOKill();
        _cachedSlotData.SlotCell.Rt.DOScale(_grabScale, _scaleDuration).SetEase(Ease.OutBack);
        _cachedSlotData.SlotCell.Rt.SetAsLastSibling();
    }

    private void TryDoubleClickMerge()
    {
        var boardUI = InGameMain.Inst.MainUI.InGameUI.BoardUI;

        foreach (var pair in boardUI.SlotDict)
        {
            SlotData other = pair.Value;
            if (other == _cachedSlotData) continue;
            if (other.SlotCell == null) continue;
            if (other.Grade != _cachedSlotData.Grade) continue;

            // 같은 등급 찾음 → 상대 제거 + 내 슬롯 업그레이드
            other.ClearSlot();
            _cachedSlotData.Upgrade();
            return;
        }
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
