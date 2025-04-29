using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Person
{
    public string name;
    public int level;
    public Person(string name, int level)
    {
        this.name = name;
        this.level = level;
    }
}

public class SVController : MonoBehaviour
{
    /// <summary>
    /// scroll view����
    /// </summary>
    public MyScrollView myScrollView;
    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public int columnCount;
    public float mScale=1;
    void Start()
    {
        //���Դ���
        List<Person> personList = new List<Person>();
        for (int i = 0; i < 100; i++)
        {
            personList.Add(new Person("����" + i, i));
        }
        myScrollView = GetComponent<MyScrollView>();
        myScrollView.SetParam(personList, SetEnement, columnCount,mScale);
    }

    /// <summary>
    /// ����ÿ��Ԫ�ص�����
    /// </summary>
    /// <param name="item">scroll view�ڵ�Ԫ�ض���</param>
    /// <param name="itemData">��Ӧ������</param>
    /// <returns></returns>
    public Transform SetEnement(Transform item, Person itemData)
    {
        item.Find("Name").GetComponent<Text>().text = itemData.name;
        item.Find("Level").GetComponent<Text>().text = itemData.level + "";
        return item.transform;
    }
}