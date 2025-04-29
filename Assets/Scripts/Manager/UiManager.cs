using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI���������
public class UiManager : MonoSingleton<UiManager>
{

    public List<GameObject> UiObj = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //����һ��UI����
    public GameObject CreatUi(string name)
    {
        string path = "UI/" + name;
        CloseUiByName(name);
        GameObject obj = GameManager.instance.AddPrefab(path, this.transform);
        UiObj.Add(obj);
        TipsMaxUp();
        return obj;
    }
    //�ر����ϲ�UI����
    public void CloseUiUp()
    {
        if (UiObj.Count <= 0)
        {
            Debug.Log("û��UI��");
            return;
        }
        //HallManager.instance.DestroyPrefab(UiObj[UiObj.Count - 1], UiObj[UiObj.Count - 1], "UI/");
        Destroy(UiObj[UiObj.Count - 1]);
        UiObj.Remove(UiObj[UiObj.Count - 1]);
        return;
    }
    //����������UI����
    public GameObject GetUiByName(string name)
    {
        name += "(Clone)";
        foreach (var item in UiObj)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
    }
    //�������ƹر�UI
    public void CloseUiByName(string name)
    {
        foreach (var item in UiObj)
        {
            if (item.name == name+"(Clone)")
            {
                //HallManager.instance.DestroyPrefab(item, item, "UI/");
                Destroy(item);
                UiObj.Remove(item);
                return;
            }
        }
        return;
    }
    //����һ����ʱTips
    public void CreatTipsUi(string _msg, float _time)
    {
        var obj = GetUiByName("Panel_Tips");
        if (obj != null)
        {
            var awardObj = GameManager.instance.AddPrefab("UI/Tips", obj.transform.Find("Scroll View").Find("Viewport").Find("Content"));
            //awardObj.GetComponent<TipsAward>().InitTips(_msg, _time);
        }
        return;
    }
    //����һ����ʱ����Tips
    public void CreatTipsAwardUi(int _awardId, int _number, float _time)
    {
        var obj = UiManager.instance.GetUiByName("Panel_Tips");
        if (obj != null)
        {
            var awardObj = GameManager.instance.AddPrefab("UI/TipsAward", obj.transform.Find("Scroll View").Find("Viewport").Find("Content"));
            //awardObj.GetComponent<TipsAward>().InitTipsAward(_awardId, _number, _time);
        }
        return;
    }
    //��TiPS��������Ϊ���ϲ�
    public void TipsMaxUp()
    {
        var obj = GetUiByName("Panel_Tips");
        if (obj != null)
        {
            obj.GetComponent<RectTransform>().SetAsLastSibling();
        }
        return;
    }
    //������ʾUI����
    public void ShowRulersUi(string _name, bool _isBool)
    {
        GameObject obj = GetUiByName(_name);
        if (obj != null)
        {
            obj.SetActive(_isBool);
        }
        return;
    }
    //�رճ�����UI�����н���
    public void CloseAllUi()
    {
        for (int i = 0; i < UiObj.Count; i++)
        {
            if (UiObj[i].gameObject.name != "Panel_hall_ui(Clone)"&& UiObj[i].gameObject.name != "Panel_Tips(Clone)")
            {
               Destroy(UiObj[i]);
               UiObj.Remove(UiObj[i]);
               i--;
            }
        }
        return;
    }
}
