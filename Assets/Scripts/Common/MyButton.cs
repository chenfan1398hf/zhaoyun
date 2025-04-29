using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : Button
{
    //���캯��
    protected MyButton()
    {

    }

    //����
    public ButtonClickedEvent my_onLongPress;
    public ButtonClickedEvent OnLongPress
    {
        get { return my_onLongPress; }
        set { my_onLongPress = value; }
    }


    //��������
    private bool my_IsStartPress = false;//�Ƿ�ʼ����
    private float my_currPointDownTime = 0.0f;//��ǰ���µ�ʱ��
    private float my_longPressTiem = 0.5f;//�����Ĵ���ʱ��
    private bool my_longPressTrigger = false;//�Ƿ����ڴ���

    /// <summary>
    /// CheckIsLongPress() ��鳤���Ƿ񴥷�
    /// </summary>
    private void CheckIsLongPress()
    {
        if (my_IsStartPress && !my_longPressTrigger)
        {
            if (Time.time > my_currPointDownTime + my_longPressTiem)
            {
                my_longPressTrigger=true;
                my_IsStartPress = false;
                if (my_onLongPress != null)
                {
                    my_onLongPress.Invoke();
                }
            }
        }

    }


    public void Update()
    {
        CheckIsLongPress();
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        my_currPointDownTime = Time.time;
        my_IsStartPress = true;
        my_longPressTrigger = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        my_IsStartPress=false;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        my_IsStartPress = false;
    }
}
