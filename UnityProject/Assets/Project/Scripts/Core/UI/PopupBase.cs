using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    [SerializeField] private Transform _rootTf;
    [SerializeField] private Button _btnClose;
    [SerializeField] private bool _openCloseAnimation = false;

    private Sequence openSeq;
    private Sequence closeSeq;

    private float _animDuration = 0.3f;

    public virtual void Initialize()
    {
        _btnClose?.onClick.AddListener(OnClickCloseButton);

        gameObject.SetActive(false);
    }

    protected virtual void OnClickCloseButton()
    {
        Close();
    }

    public virtual void Open()
    {
        if (openSeq != null) openSeq.Kill();

        gameObject.SetActive(true);

        if(_openCloseAnimation)
        {
            _rootTf.localScale = Vector3.zero;

            openSeq = DOTween.Sequence()
                .Append(_rootTf.DOScale(1, _animDuration));
            
        }
        else
        {
            _rootTf.localScale = Vector3.one;
        }
    }

    public virtual void Close()
    {
        if (closeSeq != null) closeSeq.Kill();

        _rootTf.localScale = Vector3.one;

        if (_openCloseAnimation)
        {
            closeSeq = DOTween.Sequence()
                .Append(_rootTf.DOScale(0, _animDuration))
                .AppendCallback(() => gameObject.SetActive(false));

        }
        else
        {
            _rootTf.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
