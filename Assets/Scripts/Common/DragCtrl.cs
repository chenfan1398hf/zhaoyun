using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCtrl : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, ICanvasRaycastFilter
{
    public Camera mCamera;
    private Transform nowparent;
    private bool isRaycastLocationValid = true;//Ĭ�����߲��ܴ�͸����
    public Transform IconList;
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return isRaycastLocationValid;
    }
    /// <summary>
    /// ��ʼ�϶�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        OClickBtn();
        this.transform.SetAsLastSibling();//������ק����������Ϊ�����Ⱦ
        nowparent = this.transform.parent;//�������ʼ��λ��
        isRaycastLocationValid = false;
        transform.SetParent(IconList);//����ק��������ڹ������ڵ���
        transform.Find("Select").gameObject.SetActive(true);
    }

    /// <summary>
    /// �϶���
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 v2 = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(IconList.GetComponent<RectTransform>(), Input.mousePosition,mCamera, out v2);
        transform.localPosition = v2;
    }

    /// <summary>
    /// �϶�����
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnEndDrag(PointerEventData eventData)
    {
        //transform.Find("Select").gameObject.SetActive(false);
        
        GameObject go = eventData.pointerCurrentRaycast.gameObject;
        if(go != null)
        {
                transform.SetParent(nowparent);
                transform.position = nowparent.position;
        }
        else
        {
            transform.SetParent(nowparent);
            transform.position = nowparent.position;
        }
        isRaycastLocationValid = true;//���߲����Դ�͸����
    }


    public void OClickBtn()
    {

    }
}
