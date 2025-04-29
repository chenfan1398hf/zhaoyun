using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �¼�����
/// �۲���ģʽ
/// </summary>
public class EventManager : BaseManager<EventManager>
{
    //kye�¼���
    //value ��������¼���Ӧ��ί�к�����
    private Dictionary<string, UnityAction<object>> eventDic = new Dictionary<string, UnityAction<object>>();

    /// <summary>
    /// ����¼�����
    /// </summary>
    public void AddEventListener(string _name, UnityAction<object> _action)
    {
        if (eventDic.ContainsKey(_name))
        {
            eventDic[_name] += _action;
        }
        else
        {
            eventDic.Add(_name, _action);
        }
    }
    /// <summary>
    /// �Ƴ��¼�����
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_action"></param>
    public void RemoveEventListener(string _name, UnityAction<object> _action)
    {
        if (eventDic.ContainsKey(_name))
        {
            eventDic[_name] -= _action;
        }
    }

    /// <summary>
    /// �¼�����
    /// </summary>
    public void EventTrigger(string _name, object _obj)
    {
        if (eventDic.ContainsKey(_name))
        {
            eventDic[_name].Invoke(_obj);
        }
    }
    /// <summary>
    /// ���
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
