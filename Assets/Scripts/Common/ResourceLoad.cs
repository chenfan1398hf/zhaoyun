using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResourcesLoad :Singleton<ResourcesLoad>
{
    [Tooltip("Ԥ�����ϣ��")]
    private Hashtable m_PrefabTable;

    #region ResourcesLoad ���캯��
    /// <summary>
    /// ���캯��
    /// </summary>
    public ResourcesLoad()
    {
        m_PrefabTable = new Hashtable();
    }
    #endregion

    #region Load Ԥ���嶯̬���ط���
    /// <summary>
    /// Load Ԥ���嶯̬���ط���
    /// </summary>
    /// <param name="type"> Ԥ��������</param>
    /// <param name="path">Ԥ�������ƣ�·����</param>
    /// <param name="cache">�Ƿ��л���</param>
    /// <returns>Ԥ����ʵ��</returns>
    public T Load<T>(string AllPath, bool cache = false)where T : Object
    {
        StringBuilder m_Builder = new StringBuilder();
        T prefab = null;// default(T);
        if (m_PrefabTable.ContainsKey(AllPath))
        {
            // Debug.Log(path + "����Դ���Ի���");
            prefab = m_PrefabTable[AllPath] as T;
            cache = false;
        }
        else
        {
            m_Builder.Append(AllPath);
            prefab = Resources.Load<T>(m_Builder.ToString());
            //if (prefab is GameObject)
            //{
            //    prefab = GameObject.Instantiate(prefab);
            //}
            if (!cache)
            {
                m_PrefabTable.Add(AllPath, prefab);
            }
        }
        if (prefab == null)
        {
            Debug.Log("Load ===>" + AllPath);
        }
        return  prefab;
    }
    #endregion

    #region Dispose() �ͷ���Դ
    /// <summary>
    /// Dispose() �ͷ���Դ
    /// </summary>
    public void Dispose()
    {
        m_PrefabTable.Clear();
        Resources.UnloadUnusedAssets();
    }
    #endregion
}
