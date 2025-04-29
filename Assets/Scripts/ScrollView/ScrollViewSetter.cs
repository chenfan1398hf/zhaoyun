using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSetter<T>
{
    /// <summary>
    /// ί��
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public delegate Transform SetItemData(Transform transform, T itemData);
    /// <summary>
    /// ��ʾ����
    /// </summary>
    public int columnCount = 1;
    /// <summary>
    /// �о�
    /// </summary>
    public float rowDistance;
    /// <summary>
    /// �о�
    /// </summary>
    public float columnDistance;
    /// <summary>
    /// ����ʾ����
    /// </summary>
    private int rowCount;
    /// <summary>
    /// List�Ļ������
    /// </summary>
    private Transform content;
    /// <summary>
    /// List�Ŀ��
    /// </summary>
    private float parentWidth = 0;
    /// <summary>
    /// List�ĸ߶�
    /// </summary>
    private float parentHeight = 0;
    /// <summary>
    /// ��ʾ��ITem
    /// </summary>
    private Transform itemTransform;
    /// <summary>
    /// ��������
    /// </summary>
    private float itemWidth = 0;
    /// <summary>
    /// ������߶�
    /// </summary>
    private float itemHeight = 0;
    /// <summary>
    /// ��ʾ������������
    /// </summary>
    private int showCount = 0;
    /// <summary>
    /// ���ӷ�Χ
    /// </summary>
    private float viewScope = 0;
    /// <summary>
    /// �ܳ���
    /// </summary>
    private float overAllLength = 0;
    /// <summary>
    /// ��ʼλ��
    /// </summary>
    private float contentStartPos = 0;

    /// <summary>
    /// ScrollView����
    /// </summary>
    private Transform rootTransform = null;

    /// <summary>
    /// �����б�
    /// </summary>
    private List<T> dataList;
    /// <summary>
    /// ί��
    /// </summary>
    private SetItemData setItemData;

    //private Stack<Transform> itemStack; //���ջ�洢Item�������ȡ
    /// <summary>
    /// �洢��ʾ��Ԫ�أ�ÿ�λ���ʱ�����޸�
    /// </summary>
    private Dictionary<int, Transform> itemDictionary = new Dictionary<int, Transform>();


    /// <summary>
    /// ���û�������
    /// </summary>
    /// <param name="list">������ʾ������</param>
    /// <param name="setElement">���õ���ÿ��Ԫ�����ݵķ���</param>
    /// <param name="root">scroll view����</param>
    /// <param name="rowDistance">�о�</param>
    /// <param name="columnDistance">�о�</param>
    /// <param name="columnCount">��ʾ����</param>
    public void SetParam(List<T> list, SetItemData setElement, Transform root, float rowDistance, float columnDistance, int columnCount,float scale)
    {
        this.rowDistance = rowDistance;
        this.columnCount = columnCount;
        this.columnDistance = columnDistance;
        rootTransform = root;
        dataList = list;
        this.setItemData = setElement;
        InitParameter();
        SetData(scale);
    }


    /// <summary>
    /// ������ʼ��
    /// </summary>
    private void InitParameter()
    {
        content = rootTransform.Find("Viewport/Content");                           //��ȡContent
        contentStartPos = content.localPosition.y;                                  //�õ���ʼλ��
        viewScope = rootTransform.GetComponent<RectTransform>().rect.height;      //�õ�scroll view�Ŀɼ�����߶�

        itemTransform = rootTransform.Find("Viewport/Item").transform;              //��ʾ��Ԫ�ر���
        itemTransform.gameObject.SetActive(false);                                  //��ԭʼ�Ĺر���ʾ
        Rect itemRect = itemTransform.GetComponent<RectTransform>().rect;           //�õ�Ԫ�صĿ��
        itemWidth = itemRect.width;
        itemHeight = itemRect.height;

        parentWidth = rootTransform.GetComponent<RectTransform>().rect.width;
        parentHeight = rootTransform.GetComponent<RectTransform>().rect.height;

        int maxcolumnCount = (int)(parentWidth / (itemWidth + columnDistance));                        //�õ�������ʾ�����������������õ��������������޸�Ϊ�������
        if (maxcolumnCount < (columnCount))
            columnCount = maxcolumnCount;

        rowCount = (int)((parentHeight - (rowDistance + itemHeight / 2)) / (rowDistance + itemHeight) + 2);     //�õ�����ʾ������
        if (columnCount == 0) columnCount = 1;
        showCount = columnCount * rowCount;                                                                     //�õ����������ͬʱ��ʾ��Ԫ������
    }

    /// <summary>
    /// ����Ԫ��
    /// </summary>
    /// <param name="list"></param>
    public void SetData(float scale)
    {
        int maxCount = dataList.Count;
        int index = 0;
        overAllLength = (itemHeight + rowDistance) * maxCount / columnCount;        //����õ�Ҫ��ʾ�������ݵĸ߶ȣ��������������Ļ�������
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, overAllLength + itemHeight);  //���¼���Content�Ŀ������װItem,�������x��Ŀ�Ⱥ�������0��ok�ĵ��ǲ��淶���ȴ����������е��Է�����������ʱû�н����
        ScrollRect scrollRect = rootTransform.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnValueChange);                                       //��������ֵ�ĸı䣬�������λ����Ϣ 
        }

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                if (index >= maxCount)
                {
                    return;
                }
                Transform itemTrans = setItemData(GameObject.Instantiate(itemTransform.gameObject).transform, dataList[index]);
                itemTrans.parent = content.transform;
                itemTrans.gameObject.SetActive(true);
                itemTrans.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);                  //����Ԫ��ê��
                itemTrans.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);

                float x = (column * (itemWidth + columnDistance) + itemWidth / 2 + columnDistance);
                //ǰ������ľ���=����*������߶�+������룩
                //�Լ��������˰��+���
                float y = -(row * (itemHeight + rowDistance) + itemHeight / 2 + rowDistance);
                itemTrans.localPosition = new Vector2(x, y);   //����λ��
                itemTrans.localScale = Vector3.one*scale;
                itemDictionary.Add(index, itemTrans);
                index++;
            }
            if (index >= maxCount)
            {
                return;
            }
        }
    }


    /// <summary>
    /// ����List����
    /// </summary>
    /// <param name="vector"></param>
    public void OnValueChange(Vector2 vector)
    {
        int startIndex = GetShowIndex();

        //����С�ڵ�һ�������ܴ�ʣ�µ�item��������������һҳ
        if (startIndex < 1) startIndex = 1;
        //else if (startIndex - (dataList.Count - showCount) < 0) startIndex = (dataList.Count - showCount);

        float overallProp = viewScope / overAllLength;    //���ӱ���
        float maxY = content.transform.InverseTransformPoint(new Vector3(0, vector.y * overAllLength, 0)).y;              //���߶�
        float minY = content.transform.InverseTransformPoint(new Vector3(0, vector.y * overallProp * overAllLength, 0)).y;                //��С�߶�

        int index = startIndex - 1; //��Ȼ�ҵ��˿�ʼλ�ã����������±��Ǵ�0��ʼ��

        int endIndex = index + showCount;

        List<int> uplist = new List<int>();
        List<int> downList = new List<int>();
        //��ղ��ڷ�Χ�ڵ����ݴ浽������
        foreach (int key in itemDictionary.Keys)
        {
            //��ǰ�����ڿ��ӷ�Χ֮��
            if (key < index && key + showCount < dataList.Count)
            {
                uplist.Add(key);
            }

            //��ǰ�����ڿ��ӷ�Χ֮��
            if (key >= endIndex/* + (columnCount * (rowCount + 1)) - columnCount - 1*/)
            {
                downList.Add(key);
            }
        }
        //ɾ������ı�ʾ�������»��ˣ�
        //����Ҫ�����Ǹ�λ�����������ӷ�Χ����
        foreach (int cursor in uplist)
        {

            Transform trans;
            if (itemDictionary.TryGetValue(cursor, out trans))
            {
                itemDictionary.Remove(cursor);
                int row = cursor / columnCount + rowCount;  //�����ڼ���
                int pos = cursor + showCount;
                float colum = -(row * (itemHeight + rowDistance) + itemHeight / 2 + rowDistance);   //���������λ��
                if (showCount + cursor < dataList.Count)
                {
                    trans = setItemData(trans, dataList[showCount + cursor]);
                    trans.localPosition = new Vector2(trans.localPosition.x, colum);
                    itemDictionary.Add(pos, trans);
                }

            }
        }

        //ɾ������ı�ʾ�������ϻ��ˣ�
        //����Ҫ�����Ǹ�λ�����ϻ����ӷ�Χ����
        foreach (int cursor in downList)
        {
            Transform trans;
            if (itemDictionary.TryGetValue(cursor, out trans))
            {
                itemDictionary.Remove(cursor);
                int row = cursor / columnCount - rowCount;  //�����ڼ���
                int pos = cursor - showCount;
                float colum = -(row * (itemHeight + rowDistance) + itemHeight / 2 + rowDistance);  //���������λ��
                trans = setItemData(trans, dataList[cursor - showCount]);
                trans.localPosition = new Vector2(trans.localPosition.x, colum);
                itemDictionary.Add(pos, trans);
            }
        }

    }

    /// <summary>
    /// ��ȡ��Ҫ�ӵڼ���λ�ÿ�ʼ��ʾ
    /// ��������������ʼ��λ�ü�ȥ��ǰλ��
    /// ���»�ֵԽ�ͣ����ϻ�ֵԽ�ߡ�
    /// (��ʼλ��-��ǰλ��)/(item��ֱ����+item�߶�)+1 = �ӵڼ��п�ʼ��ʾ
    /// ����*3+1
    /// </summary>
    public int GetShowIndex()
    {
        float startPos = contentStartPos;
        float currentPos = content.localPosition.y;
        int line = ((int)((currentPos - startPos) / (itemHeight + rowDistance)) + 1);
        int startIndex = line * columnCount - columnCount + 1;
        return startIndex;
    }
}
