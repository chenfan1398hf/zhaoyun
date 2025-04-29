using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyScrollView : MonoBehaviour
{
    public float rowDistance;               //�о�
    public float columnDistance;            //�о�
    /// <summary>
    /// ��������scroll view�ڵ�����Ԫ�ص���ʾ��λ�õĽ��뷽��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="setElement"></param>
    public void SetParam<T>(List<T> list, ScrollViewSetter<T>.SetItemData setElement, int columnCount,float scale)
    {
        ScrollViewSetter<T> baseList = new ScrollViewSetter<T>();
        baseList.SetParam(list, setElement, transform, rowDistance, columnDistance, columnCount, scale);
    }
}
