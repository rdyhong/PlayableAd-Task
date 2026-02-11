using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIWindow : MonoBehaviour
{
    // Buttons
    public List<Button> buttons = new List<Button>();
    // Texts
    public List<TextMeshProUGUI> textMeshPros = new List<TextMeshProUGUI>();
    // Main RectTransform
    public RectTransform mainTrans;

    public CanvasGroup CachedCanvasGroup
    {
        get
        {
            if (cachedCanvasGroup == null)
            {
                cachedCanvasGroup = GetComponent<CanvasGroup>();
            }
            return cachedCanvasGroup;
        }
    }
    private CanvasGroup cachedCanvasGroup;

    public bool isCloseESC = true;
    public bool isOpen = false;

    [HideInInspector]public bool isOpened = true;

    public Coroutine waitForCloseCoroutine;

    public Sequence splashSeq;
    public virtual void Start()
    {
        InitWindow();
    }

    public abstract void Init();
    public abstract void DeInit();
    public abstract void TextSetting();

    public virtual void InitWindow()
    {
        UIWindowMgr.AddTotalWindow(this);

        if (isOpen)
            Open(true);
        else
            Close(true);
    }
    public virtual void Open(bool force = false, float duration = 0.5f , Action callback = null , Action fallback = null)
    {
        if (isOpen == false || force)
        {
            transform.SetAsLastSibling();
            StopCoroutine();

            isOpen = true;

            CachedCanvasGroup.blocksRaycasts = true;
            UIWindowMgr.AddOpendWindow(this);
            splashSeq = DOTween.Sequence();
            //splashSeq.Append(CachedCanvasGroup.DOFade(1, 1));
            splashSeq.Append(DOTween.To(() => CachedCanvasGroup.alpha, x => CachedCanvasGroup.alpha = x, 1, 0));
            if (mainTrans != null)
                splashSeq.Join(mainTrans.DOScale(1, 0));
            splashSeq.OnComplete(() =>
            {
                CachedCanvasGroup.alpha = 1;
                CachedCanvasGroup.interactable = true;
                isOpened = true;
                callback?.Invoke();
            });
        }
    }

    public virtual void Close(bool force = false, float duration = 0.5f ,Action callback = null)
    {
        if (isOpen == true || force)
        {
            StopCoroutine();

            isOpen = false;

            UIWindowMgr.RemoveOpendWindow(this);
            splashSeq = DOTween.Sequence();
            splashSeq.Append(DOTween.To(() => CachedCanvasGroup.alpha, x => CachedCanvasGroup.alpha = x, 0, 0));
            if (mainTrans != null)
                splashSeq.Join(mainTrans.DOScale(0, 0));
            splashSeq.OnComplete(()=> {
                if (force == false)
                {
                    waitForCloseCoroutine = StartCoroutine(WaitForClose());
                }
                else
                {
                    //cachedGameObject.SetActive(false);
                    CachedCanvasGroup.alpha = 0;
                    CachedCanvasGroup.interactable = false;
                    CachedCanvasGroup.blocksRaycasts = false;
                    isOpened = false;
                    callback?.Invoke();
                }
            });
            
        }
        //if (UIWindowMgr.GetTopUIWindow() != null)
            //UIWindowMgr.GetTopUIWindow().CanvasGroupOn();
    }

    public void StopCoroutine()
    {
        if (waitForCloseCoroutine != null)
            StopCoroutine(waitForCloseCoroutine);

        waitForCloseCoroutine = null;
    }
    public IEnumerator WaitForClose()
    {
        yield return null;
        CachedCanvasGroup.alpha = 0;
        CachedCanvasGroup.interactable = false;
        CachedCanvasGroup.blocksRaycasts = false;
        isOpened = false;
    }

    // Binding Buttons
    public abstract void OnButtonBinding();

    //public virtual void AddDynamicTmp()
    [ContextMenu("ButtonBinding")]
    public void FindButtonBinding()
    {
        buttons.Clear();
        var temp = FindObjectsOfType<Button>();
        for (int i = 0; i < temp.Length; i++)
        {
            buttons.Add(temp[i]);
        }
    }

    public TextMeshProUGUI GetTmp(string name)
    {
        foreach(TextMeshProUGUI _tmp in textMeshPros)
        {
            if (name == _tmp.name)
                return _tmp;
        }
        return null;
    }
}