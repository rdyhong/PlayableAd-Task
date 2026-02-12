using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IngameBoardSlotCell : MonoBehaviour
{
    [SerializeField] private Image _imgIcon;

    public RectTransform Rt { get; private set; }

    public void Initialize()
    {
        if(Rt == null) Rt = GetComponent<RectTransform>();
    }

    public void OnPointerDown()
    {
    }
}
