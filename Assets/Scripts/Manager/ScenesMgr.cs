using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �����л�ģ���õ�
/// �����첽����
/// Э��
/// ί��
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
    /// <summary>
    /// ����ͬ������
    /// </summary>
    /// <param name="_name">������</param>
    /// <param name="_fun">�ص�����</param>
    public void LoadScene(string _name, UnityAction _fun)
    {
        SceneManager.LoadScene(_name);
        _fun.Invoke();
    }
    /// <summary>
    /// �첽����Я��
    /// </summary>
    /// <param name="_name">������</param>
    /// <param name="_fun">�ص�����</param>
    public void LoadSceneAsyn(string _name, UnityAction _fun)
    {
        GameManager.instance.StartCoroutine(ReallyLoadSceneAsyn(_name, _fun));
    }

    private IEnumerator ReallyLoadSceneAsyn(string _name, UnityAction _fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(_name);
        while (!ao.isDone)
        {
            //�¼���������ַ�����������ⲿ��ʹ���������½�����
            //EventManager.GetInstance().EventTrigger("����������", ao.progress);

            yield return ao.progress;
        }
        _fun.Invoke();
    }
}
