using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ������
/// �ҵ������Լ�����µĿؼ�����
/// �ṩ��ʾ������Ϊ
/// </summary>
public class BasePanel : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
    private void Awake()
    {
        FindChildControl<Button>();
        FindChildControl<Image>();
        FindChildControl<Text>();
        FindChildControl<Toggle>();
        FindChildControl<Slider>();
        FindChildControl<ScrollRect>();
    }

    /// <summary>
    /// ��ʾ�Լ�
    /// </summary>
    public virtual void ShowMe()
    { 
    }
    /// <summary>
    /// Ӱ���Լ�
    /// </summary>
    public virtual void HideMe()
    {
    }
    /// <summary>
    /// �õ���Ӧ���ֵĶ�Ӧ�ؼ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_controName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string _controName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(_controName))
        {
            for (int i = 0; i < controlDic[_controName].Count; ++i)
            {
                if (controlDic[_controName][i] is T)
                {
                    return controlDic[_controName][i] as T;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// ��ȡ�����Ӧ�ؼ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildControl<T>() where T: UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();

        string objName = string.Empty;
        for (int i = 0; i < controls.Length; i++)
        {
            objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add(controls[i]);
            }
            else
            {
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            }
        }
        return;
    }
}
