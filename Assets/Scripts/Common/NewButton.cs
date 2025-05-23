
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine;

public class NewButton :Selectable, IPointerClickHandler, ISubmitHandler
{
    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    protected NewButton()
    { }


    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }


    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonLongPressEvent : UnityEvent { }

    [FormerlySerializedAs("onLongPress")]
    [SerializeField]
    private ButtonLongPressEvent m_onLongPress = new ButtonLongPressEvent();
    public ButtonLongPressEvent onLongPress
    {
        get { return m_onLongPress; }
        set { m_onLongPress = value; }
    }

    [FormerlySerializedAs("OnDoubleClick")]
    public ButtonClickedEvent m_onDoubleClick = new ButtonClickedEvent();
    public ButtonClickedEvent OnDoubleClick
    {
        get { return m_onDoubleClick; }
        set { m_onDoubleClick = value; }
    }

    private bool my_isStartPress = false;

    private float my_curPointDownTime = 0f;

    private float my_longPressTime = 0.1f;

    private bool my_longPressTrigger = false;
    private float invokeTime;
    private  float interval = 0.1f;


    void Update()
    {
        CheckIsLongPress();
        TriggerlongPress();
    }

    void CheckIsLongPress()
    {
        if (my_isStartPress && !my_longPressTrigger)
        {
            if (Time.time > my_curPointDownTime + my_longPressTime)
            {
                my_longPressTrigger = true;
                my_isStartPress = false;
                invokeTime = Time.time;
                if (m_onLongPress != null)
                {
                    m_onLongPress.Invoke();
                }
            }
        }
    }

    void TriggerlongPress()
    {
        if (my_longPressTrigger)
        {
            if (Time.time - invokeTime >= interval)
            {
                invokeTime = Time.time;
                m_onLongPress.Invoke();
            }
        }
    }



    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //(避免已經點擊進入長按后，擡起的情況)
        if (!my_longPressTrigger)
        {
            // 正常單擊 
            if (eventData.clickCount == 2)
            {

                if (m_onDoubleClick != null)
                {
                    m_onDoubleClick.Invoke();
                }

            }// 雙擊
            else if (eventData.clickCount == 1)
            {
                onClick.Invoke();
            }
        }
    }



    public virtual void OnSubmit(BaseEventData eventData)
    {
        Press();

        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // 按下刷新當前時間
        base.OnPointerDown(eventData);
        my_curPointDownTime = Time.time;
        my_isStartPress = true;
        my_longPressTrigger = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // 指針擡起，結束開始長按
        base.OnPointerUp(eventData);
        my_isStartPress = false;
        my_longPressTrigger = false;

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 指針移出，結束開始長按，計時長按標志
        base.OnPointerExit(eventData);
        my_isStartPress = false;

    }
}
