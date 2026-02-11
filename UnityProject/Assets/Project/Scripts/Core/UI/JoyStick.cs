using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // 뒷판
    [SerializeField] RectTransform _rectBack;
    // 스틱
    [SerializeField] RectTransform _rectJoystick;

    // 뒷판 초기 위치
    Vector2 _baseJSPos;

    // 뒷판 반지름
    private float _fRadius;
    // 터시 시간 (터치를 유지한 시간)
    private float _touchTime = 0;

    // 터치 시작지점 뒷판 위치
    private Vector2 _backStartPos;

    void Start()
    {
        _baseJSPos = _rectBack.localPosition;
        _fRadius = _rectBack.rect.width * 0.5f;
    }

    // 터치 중
    void OnTouch(PointerEventData eventData)
    {
        // 현재 터치 위치
        Vector2 stickPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectBack.parent as RectTransform, // 부모 RectTransform 사용
            eventData.position,                // 스크린 좌표
            eventData.pressEventCamera,        // UI 카메라
            out stickPoint                     // 변환된 로컬 좌표
        );

        // 방향
        Vector2 vec = stickPoint - _backStartPos;
        vec = Vector2.ClampMagnitude(vec, _fRadius);

        // Do 0 ~ 1, 스틱 당김 정도, 데드존 역할도 가능
        float _weight = (_backStartPos - stickPoint).sqrMagnitude / (_fRadius * _fRadius);
        _weight = _weight < 0.02f ? 0 : _weight;
        _weight = _weight > 0.98f ? 1 : _weight;

        _rectJoystick.localPosition = vec;

        // ------ 여기 아래부터 값 필요한데로 보내면 됨 -------

        // Input 클래스로 넘김
        InputMgr.StickWeight = _weight;

        if (_weight > 0)
        {
            InputMgr.StickDir = vec.normalized;
        }
        else
        {
            InputMgr.StickDir = Vector2.zero;
        }
    }

    // 터치 시작
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectBack.parent as RectTransform, // 부모 RectTransform 사용
            eventData.position,                // 스크린 좌표
            eventData.pressEventCamera,        // UI 카메라
            out localPoint                     // 변환된 로컬 좌표
        );

        _backStartPos = localPoint;
        _rectBack.localPosition = localPoint; // 변환된 좌표를 적용
    }

    // 터치 중
    public void OnDrag(PointerEventData eventData)
    {
        OnTouch(eventData);
    }

    // 터치 종료
    public void OnPointerUp(PointerEventData eventData)
    {
        _rectBack.localPosition = _baseJSPos;
        _rectJoystick.localPosition = Vector2.zero;
        InputMgr.StickDir = Vector2.zero;
    }

    private void Update()
    {
        _touchTime += Time.deltaTime;
    }
}