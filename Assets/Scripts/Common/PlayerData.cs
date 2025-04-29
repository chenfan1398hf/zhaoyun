using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

//�������
public class PlayerData
{
    public PlayerData()
    {
        isSave = true;
        playerLocalizeState = 1;
        palyerUpdateTime = Convert.ToDateTime("2021-9-25");
        playerLogoutTime =DateTime.Now;

        playerSyntheticBtnset = 0;
        
    }
    public static bool isSave;                          //�Ƿ�浵
    public DateTime palyerUpdateTime;                   //ÿ��״̬����ʱ�� 
    public DateTime palyerUpdateWeekTime;               //ÿ��״̬����ʱ�� 
    public int playerLocalizeState;                     //1����2Ӣ��
    public int playerSyntheticBtnset;                   //0-δѡ��  1-ѡ��
    public DateTime playerTlTime;                       //�ϴλָ�����ʱ��
    public int playerPower;                             //��Һϳ�С��Ϸ������

    public DateTime playerLoginTime;                    //��ҵ�¼��Ϸ��ʱ��
    public DateTime playerLogoutTime;                   //��ҵǳ���Ϸʱ��

    public bool playerAudio1;                           //��Ч����
    public bool playerAudio2;                           //���ֿ���


    public Dictionary<String, TaskInfo> palyerTaskDic = new Dictionary<String, TaskInfo>();                         //��������




    //��ȡ�����������
    public static PlayerData GetLocalData()
    {
        PlayerData newData = null;
        //��ȡ��������
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            string data = PlayerPrefs.GetString("PlayerData").ToString();
            newData = JsonMapper.ToObject<PlayerData>(data);
            if (GameManager.isDbugLog)
                Debug.Log(data);
        }
        else
        {
            if (GameManager.isDbugLog)
                Debug.Log("δ�ҵ������������------>�½�");
            newData = new PlayerData();
        }

        return newData;
    }

    public static PlayerData GetCloudData(string receiveData)
    {
        var newData = JsonMapper.ToObject<PlayerData>(receiveData);
        return newData;
    }
    public  void UpdateLoginTime()
    {
        playerLoginTime = DateTime.Now;
    }

    public static void ClearLocalData()
    {
        isSave = false;
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            PlayerPrefs.DeleteKey("PlayerData");
            PlayerPrefs.Save();
            if (GameManager.isDbugLog)
                Debug.Log("������ش浵");
        }
    }
    public void InitData()
    {
        //foreach (var item in GameManager.instance.configMag.TaskInfoCfg)
        //{
        //    TaskInfo taskInfo1 = new TaskInfo();
        //    if (!palyerTaskDic.TryGetValue(item.ID.ToString(), out taskInfo1))
        //    {
        //        TaskInfo taskInfo = new TaskInfo();
        //        taskInfo.taskId = item.ID;
        //        taskInfo.tasktype = item.type;
        //        taskInfo.taskPrepositionTaskID = item.prepositionTaskID;
        //        if (item.needUnlock == 0)
        //        {
        //            taskInfo.isUnlock = true;
        //            taskInfo.taskState = 0;
        //        }
        //        else
        //        {
        //            taskInfo.isUnlock = false;
        //            taskInfo.taskState = -2;
        //        }
        //        palyerTaskDic.Add(item.ID.ToString(), taskInfo);
        //    }
        //}


    }


    //���汾���������
    public void Save()
    {
        if (isSave)
        {
            string data = JsonMapper.ToJson(this);
            PlayerPrefs.SetString("PlayerData", data);
            PlayerPrefs.Save();
            if (GameManager.isDbugLog)
                Debug.Log("���ش浵");
        }
    }
    //����������ݵ�����
    public void SaveLocal()
    {
        //string data = JsonMapper.ToJson(this);
        //PlayerPrefs.SetString("PlayerData", data);
        //PlayerPrefs.Save();
        //string data = JsonMapper.ToJson(this);
        //if (GameManager.instance.userInfo.isGoogle)
        //{
        //    PottingMobile._SavedGame(data);
        //}
        //else
        //{
        //    PlayerPrefs.SetString("PlayerData", data);
        //    PlayerPrefs.Save();
        //    Debug.Log("���ش浵");
        //}
    }
    //������Ч
    public void PlayerOpenAudio()
    {
        //AudioManager.SwitchSound(playerAudio1);
        //AudioManager.SwitchBgm(playerAudio2);
        return;
    }
    //����
    public void AwardGive(string award, bool isTipsAwardUi = true)
    {
        string strSkills = award;
        string[] list = strSkills.Split(new char[] { ';' });
        for (int i = 0; i < list.Length; i++)
        {
            string[] listGrade = list[i].Split(new char[] { ':' });
            if (listGrade.Length != 2)
            {
                if (GameManager.isDbugLog)
                    Debug.Log("�����쳣");
                return;
            }
            AwardToPlayer(int.Parse(listGrade[0]), (int)float.Parse(listGrade[1]), isTipsAwardUi);
        }
    }
    public string AwardGive(string award, float number, bool isTipsAwardUi = true)
    {
        string strSkills = award;
        string str = string.Empty;
        string[] list = strSkills.Split(new char[] { ';' });
        for (int i = 0; i < list.Length; i++)
        {
            string[] listGrade = list[i].Split(new char[] { ':' });
            if (listGrade.Length != 2)
            {
                if (GameManager.isDbugLog)
                    Debug.Log("�����쳣");
                return null;
            }
            int adNumber = (int)(float.Parse(listGrade[1]) * number);
            AwardToPlayer(int.Parse(listGrade[0]), adNumber, isTipsAwardUi);
            str += listGrade[0] + ":" + adNumber.ToString() + ";";
        }
        if (str != string.Empty)
        {
            str = str.Substring(0, str.Length - 1);
        }
        return str;
    }
    //�۳�����
    public void DeductProp(string str)
    {
        string strSkills = str;
        string[] list = strSkills.Split(new char[] { ';' });
        for (int i = 0; i < list.Length; i++)
        {
            string[] listGrade = list[i].Split(new char[] { ':' });
            if (listGrade.Length != 2)
            {
                if (GameManager.isDbugLog)
                    Debug.Log("�����쳣");
                return;
            }
            AwardToPlayer(int.Parse(listGrade[0]), -(int)float.Parse(listGrade[1]));
        }
    }
    //���Ż��ҿ۳�����
    public void AwardToPlayer(int id, int number,bool isTipsAwardUi = true)
    {
        //if (id == (int)PropId.Jb)
        //{
        //    playerJb += number;
        //}
        //else if (id == (int)PropId.Zs)
        //{
        //    playerZs += number;
        //}
        //else if (id == (int)PropId.Tl)
        //{
        //    playerPower += number;
        //    if (playerPower > 999)
        //    {
        //        playerPower = 999;
        //    }
        //}
 


        return;
    }
    public void AwardToPlayer(int id, long number, bool isTipsAwardUi = true)
    {
        //if (id == (int)PropId.Jb)
        //{
        //    playerJb += number;
        //}

     
        return;
    }
    //�����ҵ�������
    public bool CheckPlayerPropNumber(int id, long number)
    {
        //if (id == (int)PropId.Jb)
        //{
        //    if (playerJb >= number)
        //    {
        //        return true;
        //    }
        //}
        //else if (id == (int)PropId.Zs)
        //{
        //    if (playerZs >= number)
        //    {
        //        return true;
        //    }
        //}
        //else if (id == (int)PropId.Tl)
        //{
        //    if (playerPower >= number)
        //    {
        //        return true;
        //    }
        //}

        return false;
    }
    //�����ҵ�������
    public bool CheckPlayerPropNumber(string str)
    {
        string[] list = str.Split(new char[] { ';' });
        for (int i = 0; i < list.Length; i++)
        {
            string[] listGrade = list[i].Split(new char[] { ':' });
            if (listGrade.Length != 2)
            {
                if (GameManager.isDbugLog)
                    Debug.Log("�����쳣");
                return false;
            }
            bool state = CheckPlayerPropNumber(int.Parse(listGrade[0]), int.Parse(listGrade[1]));
            if (state == false)
            {
                return false;
            }
        }
        return true;
    }
    //������ø��ֻ״̬
    public void CheckUpdataDay()
    {
        DateTime dt = DateTime.Now;
        TimeSpan span = dt.Subtract(palyerUpdateWeekTime);
        if (dt.Day != palyerUpdateTime.Day || dt.Month != palyerUpdateTime.Month || dt.Year != palyerUpdateTime.Year)
        {
            RestPlayerDayState(dt);
        }
        //ÿ��������
        if (dt.DayOfWeek == DayOfWeek.Monday && span.Days > 1)
        {
            RestPlayerWeekState(dt);
        }
    }
    //��������ظ�
    public void CheckPlayerTl()
    {
        if (playerPower >= GameManager.TI_LI_MAX_NUMBER)
        {
            playerTlTime = DateTime.Now;
            return;
        }
        int number = (int)(DateTimeUtil.DateTimeToTimeStamp(DateTime.Now) - DateTimeUtil.DateTimeToTimeStamp(playerTlTime)) / GameManager.TI_LI_CD_NUMBER;
        int maxNumber = GameManager.TI_LI_MAX_NUMBER - playerPower;
        if (number > maxNumber)
        {
            number = maxNumber;
        }
        if (number > 0)
        {
            AwardToPlayer((int)PropId.Tl, number, false);
            long time = DateTimeUtil.DateTimeToTimeStamp(playerTlTime) + (number * GameManager.TI_LI_CD_NUMBER);
            playerTlTime = DateTimeUtil.TimeStampToDateTime(time);
        }
        return;
    }
    //ÿ��״̬����
    public void RestPlayerDayState(DateTime dt)
    {
        palyerUpdateTime = dt;
        //����ÿ������״̬
        foreach (var item in palyerTaskDic)
        {
            if (item.Value.tasktype == 1)
            {
                item.Value.taskState = 0;
                item.Value.taskTargetNumber = 0;
            }
        }
       


    }
    //ÿ��״̬����
    public void RestPlayerWeekState(DateTime dt)
    {
        palyerUpdateWeekTime = dt;

        //������ҵ�����ǩ������
        
    }
   
    //��ɵ�������
    public void TaskOnceRecord(string taskId, long number)
    {
        TaskInfo taskInfo;
        if (!palyerTaskDic.TryGetValue(taskId, out taskInfo))
        {
            return;
        }
        if (taskInfo.taskState == -1 || taskInfo.taskState == 1 || taskInfo.taskState == -2)
        {
            return;
        }
        if (taskInfo.taskTargetNumber < number)
        {
            taskInfo.taskTargetNumber = number;
        }

        TaskUpdateState(taskId, taskInfo);

        return;
    }
    //����ۼ�����
    public void TaskAddRecord(string taskId, long number)
    {
        TaskInfo taskInfo;
        if (!palyerTaskDic.TryGetValue(taskId, out taskInfo))
        {
            return;
        }
        if (taskInfo.taskState == -1 || taskInfo.taskState == 1 || taskInfo.taskState == -2)
        {
            return;
        }
        //���ǰ������
        //if (taskInfo.taskPrepositionTaskID != 0)
        //{
        //    TaskInfo taskInfo1;
        //    if (!palyerTaskDic.TryGetValue(taskInfo.taskPrepositionTaskID.ToString(), out taskInfo1))
        //    {
        //        return;
        //    }
        //    if (taskInfo1.taskState != -1)
        //    {
        //        return;
        //    }
        //}

        taskInfo.taskTargetNumber += number;
        TaskUpdateState(taskId, taskInfo);

        return;
    }
    //�����������״̬
    public void TaskUpdateState(string taskId, TaskInfo playerTaskInfo)
    {
        TaskInfo taskInfo;
        if (!palyerTaskDic.TryGetValue(taskId, out taskInfo))
        {
            return;
        }
        if (taskInfo.taskState != 0)
        {
            return;
        }
        long targetNumber = 0;
        foreach (var item in GameManager.instance.configMag.TaskInfoCfg)
        {
            if (item.ID.ToString() == taskId)
            {
                targetNumber = item.targetNumber;
            }
        }
        if (targetNumber == 0)
        {
            return;
        }
        long Number = targetNumber;
        if (playerTaskInfo.taskTargetNumber >= Number)
        {
            playerTaskInfo.taskState = 1;
        }

        return;
    }
    //��ȡ����������״̬
    public bool GetTaskState(string taskId)
    {
        TaskInfo taskInfo;
        if (palyerTaskDic.TryGetValue(taskId, out taskInfo))
        {
            if (taskInfo.taskState == 1 || taskInfo.taskState == -1)
            {
                return true;
            }
        }
        return false;
    }
    //��������
    public bool CheckTaskRedDot()
    {
        foreach (var item in palyerTaskDic)
        {
            if (item.Value.taskPrepositionTaskID == 0)
            {
                if (item.Value.taskState == 1)
                {
                    return true;
                }
            }
            else
            {
                //���ǰ������
                TaskInfo taskInfo = new TaskInfo();
                if (palyerTaskDic.TryGetValue(item.Value.taskPrepositionTaskID.ToString(), out taskInfo))
                {
                    if (taskInfo.taskState == -1)
                    {
                        if (item.Value.taskState == 1)
                        {
                            return true;
                        }
                    }
                }
            }
          
        }
        return false;
    }
}


public class Bait
{
    public Sprite sprite;
    public int id;
    public int price;
}

/// <summary>
/// profit1 ���ӵ�ǰ��ʩ��������
/// profit2 ���ӵ����ٶ�
/// profit3 ������������
/// profit4 ��������ʱ��
/// profit5 ���̹˿����ü��ʱ��
/// profit6 ������������Ͷ������
/// profit7 ÿ���������ӣ�11.22������
/// profit8 ����������棨�ٷֱ����� 1.14������
/// profit9 �������л�����λ���棨�ٷֱ����� 1.14������
/// ����ֵ����int ������ֵ��ô���ʲ߻�
/// </summary>
public class FacilitiesProfit
{
    public int profit1 = 0;     
    public int profit2 = 0;
    public int profit3 = 0;
    public int profit4 = 0;
    public int profit5 = 0;
    public int profit6 = 0;
    public int profit7 = 0;
    public int profit8 = 0;
    public int profit9 = 0;
}


//�þ�����
public class CatPropData : IComparable<CatPropData>
{
    public int id = 0;
    private int state = 0;   //0δ����1�ѽ���-1�ѹ���-2��ʹ��
    public int State
    {
        set
        {
            state = value;
        }
        get
        {
            return state;
        }
    }
    public int sort = 0;
    public int useNumber = 0;
    public int useCatId = 0;        //���ڱ���ֻèռ��

    public int type = 1;            //�þ�����1��ͨ�þ�2è��
    public int catFoodId = 0;       //�����è��è��id
    public int catFoodNumber = 0;   //ʣ��ʳ�ô���

    public int CompareTo(CatPropData other)
    {
        //return other.sort.CompareTo(this.sort);
        if (other.State > this.State)
        {
            return 1;
        }
        else if (other.State == this.State)
        {
            return -other.sort.CompareTo(this.sort);
        }
        else
        {
            return -1;
        }
    }
}

//��ʩ����
public class FacilitiesData
{
    public int id = 0;              //id
    public int unlockState = 0;     //����״̬(0δ����1�ѽ���2�ѹ���3ʹ����)
    public int grade = 0;           //�ȼ�
    public int typeId;              //����id��ͬ������ʩֻ�������ߵȼ��ģ�
}
//��ʩ��������
public class FacilitiesTypeData
{
    public int typeId = 0;              //��ʩ����id
    public int state = 0;               //��ʩ����״̬��0δ����1�ѽ���2�ѹ���
    public int showFacilitiesId = 0;        //���չʾ��ʩid
}
//��ʩ�������
public class FacilitiesPlayerData
{
    public Dictionary<String, FacilitiesData> facilitiesDataDic = new Dictionary<String, FacilitiesData>();                     //��ʩ����
    public Dictionary<String, FacilitiesTypeData> facilitiesTypeDataDic = new Dictionary<String, FacilitiesTypeData>();         //��ʩ��������
}
//��������
public class TaskInfo : IComparable<TaskInfo>
{
    public TaskInfo()
    {
        taskId = 0;
        taskState = 0;
        taskTargetNumber = 0;
        tasktype = 0;
        taskPrepositionTaskID = 0;
        isUnlock = false;
    }
    public int taskId;                     //����id
    public int taskState;                  //���״̬��0δ���1�����-1���콱-2δ������
    public long taskTargetNumber;          //�����Ŀ������
    public int tasktype;                   //�������ͣ�1�ճ�2�ܳ�3�ɳ�4������
    public int taskPrepositionTaskID;      //ǰ������ID
    public bool isUnlock;                  //�Ƿ����

    public int CompareTo(TaskInfo other)
    {
        //return other.taskState.CompareTo(this.taskState);
        if (other.taskState > this.taskState)
        {
            return 1;
        }
        else if (other.taskState == this.taskState)
        {
            return -other.taskId.CompareTo(this.taskId);
        }
        else
        {
            return -1;
        }
    }
}

//Ա������ 
public class StaffData
{
    public int id = 0;                //Ա��ID
    public int is_unlock = 0;         //����״̬(0δ����1�ѽ���2�ѹ���3ʹ����)
    public int level = 1;             //Ա���ȼ���Ĭ�ϵȼ�1���ȼ�����Ϊ10����
    public int Sw=0;                  //����
}

// Ա���������
public class StaffPlayerData
{
    public Dictionary<string, StaffData> staffDataDic = new Dictionary<string, StaffData>();//Ա������ 
}

// ����ǩ������
public class SignInData
{
    public int data;                   //1-7  һ������
    public int is_unlock;              //����״̬(0δ����1�ѽ���)
    public int is_double;              //�Ƿ�˫����ȡ(0δ��ȡ1��ȡ1�� 2��ȡ����)
    public bool isReward;              //�Ƿ���ȡ����
}

//ǩ����¼
public class SignInRecord
{
    public int getRewardTime;           //����Ѿ���ȡ�˼���Ľ���
    public int currTime;                //����Ľ�������
    public int isRush;                  // ����Ľ����Ƿ���ȡ
}

//����ǩ���������
public class SignInPlayerData
{
    public Dictionary<string, SignInData> signInDataDic = new Dictionary<string, SignInData>();
}

//�̵꿴���ˢ����Ʒ���������
public class ShopItem_adv
{
    public int id;//��ƷID
    public int advTime;//�����ˢ�µĴ���
    public int isBuy;//�Ƿ񱻹���
}

//�̵����Ļ��ҹ������Ʒ���������
public class ShopItem_other
{
    public int id;//��ƷID
    public int isBuy;//�Ƿ񱻹���
}

//�̵굱�յ���Ʒ
public class ShopItemCurrRushItem
{
    public int pos;
    public int id;
}

//�̵���������
public class ShopItemData
{
    public Dictionary<string,ShopItem_adv> shopItem_advDataDic=new Dictionary<string, ShopItem_adv>();
    public Dictionary<string, ShopItem_other> shopItem_otherDataDic = new Dictionary<string, ShopItem_other>();
}
//���ӵ�еĵ���
public class Player_Materials
{
    public int id;
    public int num;
}
//���ӵ�еĵ������ݱ�
public class Player_MaterialsData
{
    public Dictionary<string, Player_Materials> player_MaterialsDataDic = new Dictionary<string, Player_Materials>();
}

//���ӵ�еĵ���
public class PlayerShopItem
{
    public int id;//����ID 
    public long num;//��������
}
//��Ҿ��еĵ�������
public class PlayerShopItemData
{
  public  Dictionary<string, PlayerShopItem> playerShopItemData = new Dictionary<string, PlayerShopItem>();
}
//�㡪����������
public class Fish_Item
{
    public int id;//id 
    public int unlock;//�Ƿ����0--δ���� 1--����
    public int currGrade; //��ǰ��ĵȼ�
    public int Sw=0;//����
    public int qidiaoNum = 0;
    public int siyangNum = 0;   //��������  �ۼƼ�¼
    public int toufangNum = 0;  //Ͷ������  �ۼƼ�¼
}
//���������--�������
public class Fish_ItemPlayerData
{
    public Dictionary<string, Fish_Item> fish_ItemPlayerDataDic = new Dictionary<string, Fish_Item>();
}
//�㡪������
public class FishItem_fry
{
    public int id;//id 
    public int currGrade; //��ǰ��ĵȼ�
    public int number;//���������
}
//�� ---�����������
public class FishItem_fryPlayerData
{
    public Dictionary<string, FishItem_fry> fishItem_fryPlayerDataDic = new Dictionary<string, FishItem_fry>();
}

//�㡪������Ͷ�ŵ���
public class FishItem_big
{
    public int id;//id 
    public int currGrade; //��ǰ��ĵȼ�
    public int number;//��Ʒ�������
}





// �泡�ȼ�����
public class GradeData
{
    public int currGrade;//��ҵ�ǰ�ȼ�
    public int currGrade_id;//��ҵ�ǰ�ȼ���ID
    public int nextGrade_id;//��һ�ȼ���ID
}

//�������ص�״̬
public class FishFarmingStateData
{
    public int id;//����صı�� 1,2��Ĭ�Ͻ���  3,4,5��������
    public bool isUnlock;//�Ƿ����
    public int fish_id;//û����IDΪ0
    public int fish_num;//�������
    public DateTime CloseTime;// �뿪��ص�ʱ��
}
/// <summary>
/// AdultFishData ��������
/// </summary>
public class AdultFishData
{
    public int id;//��� 
    public int number;//�������
}

/// <summary>
/// ���������
/// </summary>
public class FishSettlementData
{
    public int id  = 0;             //��� 
    public int smallNumber = 0;     //�������
    public int bigNumber = 0;       //�������
}
/// <summary>
/// ��Ϸʱ����
/// </summary>
public class GameTime
{
    public float allTime=160;//�ܵ�����ʱ��1440��=1��  480��=һ��   20��=һСʱ
    public int month=1;//��
    public int day=1;//��
    public int hour=8;//Сʱ
    public int minute=0;//����
    public int week = 1;
    public DayType type = DayType.daytime;
}
//��������
public class ToDayFishInfo
{
    public int fish_1_id=0;
    public int fish_2_id;
    public int fish_3_id;
    public int fish_4_id;
}

public enum PropId : int
{
    Jb = 8000,                        //���
    Zs = 8001,                        //��ʯ
    Tl = 8002,                          //����
    Cl1 = 8003,                          //����

    JP = 2026,                           //Ա������
    YG2027 = 2027,                       //�������
    YX2028 = 2028,                       //��������
    YG2029 = 2029,                       //�����㹳
    YE2030 = 2030,                       //�������
    HL2031 = 2031,                       //��������
    //��ʩ��id ��10000��ʼ��11000 ���ǲ�Ҫ�ظ���
    //Ա��id ��1001��ʼ��1999
    XiaoRong =1001,                    //С��
    GuZai =1002,                      //����
    HuaQiang =1003,                   //��ǿ
    OuYangXiaoXu =1004,               //ŷ������
    DengGang =1005,                   //�˸�
    LaoZhang =1006,                   //����
    ChunTao =1007,                    //����
    XiaHe =1008,                      //�ĺ�
}
public enum TaskId
{
    Task_30001 = 30001,                           //
    Task_30002 = 30002,                           //
    Task_30003 = 30003,                           //
    Task_30004 = 30004,                           //
    Task_30005 = 30005,                           //
    Task_30006 = 30006,                           //
    Task_30007 = 30007,                           //
    Task_30008 = 30008,                           //
    Task_30009 = 30009,                           //
    Task_40001 = 40001,                           //
    Task_40002 = 40002,                           //
    Task_40003 = 40003,                           //
    Task_40004 = 40004,                           //
    Task_40005 = 40005,                           //
    Task_40006 = 40006,                           //
    Task_40007 = 40007,                           //
    Task_40008 = 40008,                           //
    Task_40009 = 40009,                           //
    Task_40010 = 40010,                           //
    Task_40011 = 40011,                           //
    Task_40012 = 40012,                           //
    Task_40013 = 40013,                           //
    Task_40014 = 40014,                           //
    Task_40015 = 40015,                           //
    Task_40016 = 40016,                           //
    Task_40017 = 40017,                           //
    Task_40018 = 40018,                           //
    Task_40019 = 40019,                           //
    Task_40020 = 40020,                           //
    Task_40021 = 40021,                           //
    Task_40022 = 40022,                           //
    Task_40023 = 40023,                           //
    Task_40024 = 40024,                           //
    Task_40025 = 40025,                           //
    Task_40026 = 40026,                           //
    Task_40027 = 40027,                           //
    Task_40028 = 40028,                           //
    Task_40029 = 40029,                           //
    Task_40030 = 40030,                           //
    Task_40031 = 40031,                           //
    Task_40032 = 40032,                           //
    Task_40033 = 40033,                           //
    Task_40034 = 40034,                           //    
    Task_40035 = 40035,                           //
    Task_40036 = 40036,                           //
    Task_40037 = 40037,                           //
    Task_40038 = 40038,                           //
    Task_40039 = 40039,                           //
    Task_40040 = 40040,                           //
}
//��ʩ����IDö��
public enum FacilitiesTypeId : int
{
    FacilitiesType_1 = 1,               //1�ŵ���̨
    FacilitiesType_2 = 2,               //2�ŵ���̨
    FacilitiesType_3 = 3,               //3�ŵ���̨
    FacilitiesType_4 = 4,               //4�ŵ���̨
    FacilitiesType_5 = 5,               //5�ŵ���̨
    FacilitiesType_6 = 6,               //6�ŵ���̨
    FacilitiesType_7 = 7,               //·��
    FacilitiesType_8 = 8,               //�̻���ľ
    FacilitiesType_9 = 9,               //����
    FacilitiesType_10 = 10,             //·��
    FacilitiesType_11 = 11,             //����
    FacilitiesType_12 = 12,             //ǽ��
    FacilitiesType_13 = 13,             //����
    FacilitiesType_14 = 14,             //�����
    FacilitiesType_15 = 15,             //������
    FacilitiesType_16 = 16,             //����
    FacilitiesType_17 = 17,             //ˮ��ֲ��
    FacilitiesType_18 = 18,             //����
    FacilitiesType_19 = 19,             //Ĭ�ϵ�λ
}

//�����ҹ
public enum DayType
{
    daytime,
    night,
}

public class SyntheticPos
{
    public int id;
    public int pos;
}

//������
public class QiFuData
{
    public int day = 0;
}
//ʰȡ���ּ�¼��¼
public class PropPickTextData
{
    public int PropID = 0;//����ID
    public int TextID = 0 ;//����ID
    public int PropNum = 0;//��������
    public string TimeStr = "";//ʱ��
}
//ʰȡ��¼
public class PropPickData
{
    public int id;//����ID 
    public int num = 0;//��������
    public int ascription=0;      //ʰȡ���� 1���ϳ�С��Ϸʰȡ 2������ʰȡ����������ʯ�����ң�3������ʰȡ
}
//ʰȡ�б�
public class PropPickList
{
    //���ּ�¼�б�
    public  List<PropPickTextData> propPickTextList = new List<PropPickTextData>();
    //���߼�¼�б�
    public  List<PropPickData> propPickList = new List<PropPickData>();
    public bool AddPropPick(int PropID, int PropNum,int ascription, int TextID,string TimeStr = "")
    {
        if (propPickList.Count < 10)
        {
            PropPickData propPickData = propPickList.Find(c => c.id == PropID);
            if (propPickData != null)
            {
                propPickData.num += PropNum;
                if (propPickData.ascription == 0)
                {
                    propPickData.ascription = ascription;
                }
                AddPropPickText(PropID, PropNum, TextID,TimeStr);
                return true;
            }
            else
            {
                propPickData = new PropPickData();
                if (propPickData != null)
                {
                    propPickData.id = PropID;
                    propPickData.ascription = ascription;
                    propPickData.num = PropNum;
                }
                propPickList.Add(propPickData);
                AddPropPickText(PropID, PropNum, TextID, TimeStr);
                return true;
            }

        }
        else
        {
            PropPickData propPickData =  propPickList.Find(c => c.id == PropID);
            if (propPickData != null )
            {
                propPickData.num += PropNum;
                if (propPickData.ascription == 0)
                {
                    propPickData.ascription = ascription;
                }
                AddPropPickText(PropID, PropNum, TextID, TimeStr);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private void AddPropPickText(int PropID, int PropNum, int TextID, string TimeStr = "")
    {
        if (propPickTextList.Count < 20)
        {
            PropPickTextData propPickData = new PropPickTextData();
            propPickData.PropID = PropID;
            propPickData.PropNum = PropNum;
            propPickData.TextID = TextID;
            propPickData.TimeStr = TimeStr;
            propPickTextList.Add(propPickData);
        }
        else
        {
            PropPickTextData propPickData = propPickTextList[0];
            propPickData.PropID = PropID;
            propPickData.PropNum = PropNum;
            propPickData.TextID = TextID;
            propPickData.TimeStr = TimeStr;
            propPickTextList.RemoveAt(0);
            propPickTextList.Add(propPickData);
        }
    }
}

//�Զ��ϳ�����
public class AutoBuffData
{
    public float AutoBuffTime = 0;                           //�Զ��ϳ�buff��ʱ��
    public long SartTime = 0;                             //��ʼʱ��
    public float currTime = 0;
}

//��װ����
public class HomeChangeData
{
    public int FurnitureTypeId=0;       //��װ��������(��װ��ʩ)
    public int PropId = 0;              //��װ����id��Ƥ��id��
    public int Floor = 0;               //¥��
    public int state = 0;               //0δ���� 1�ѽ���- 1�ѹ���-2��ʹ��
}
//��װ����
public class HomeChangeDataList
{
    public List<HomeChangeData> homeChangeDataList = new List<HomeChangeData>();

    //����¥���ȡ¥���еĻ�װ����
    public List<HomeChangeData> GetHomeChangeDataListByFloor(int Floor)
    {
        return homeChangeDataList.FindAll(c => c.Floor == Floor);
    }
    //���ݻ�װ��������(��װ��ʩ) ��ȡ��װ����
    public List<HomeChangeData> GetHomeChangeDataByFurnituresTypeId(int FurnitureTypeId)
    {
        return homeChangeDataList.FindAll(c => c.FurnitureTypeId == FurnitureTypeId);
    }
    //���ݻ�װ��������(��װ��ʩ) ��ȡ��ǰʹ�û�װ����
    public HomeChangeData GetHomeChangeDataByFurnitureTypeId(int FurnitureTypeId)
    {
        return homeChangeDataList.Find(c => c.FurnitureTypeId == FurnitureTypeId&& c.state == 2);
    }
    //���ݻ�װ����ID(��װ��ʩ) ��ȡ��װ����
    public HomeChangeData GetHomeChangeDataByPropId(int PropId)
    {
        return homeChangeDataList.Find(c => c.PropId == PropId);
    }
    //��װ����
    public void HomeChangeBuy(int FurnitureTypeId, int PropId, int Floor)
    {
        HomeChangeData homeChangeData;
        bool ischange = false;
        for (int i = 0; i < homeChangeDataList.Count; i++)
        {
            homeChangeData = homeChangeDataList[i];
            if (homeChangeData.PropId == PropId)  // if (homeChangeData.FurnitureTypeId == FurnitureTypeId)//&& homeChangeData.Floor == Floor
            {
                ischange = true;
                homeChangeData.state = 2;
            }
        }
        if (ischange == false)
        {
            AddHomeChangeData(FurnitureTypeId, PropId, Floor, 2);
        }
        UpdateHomeChangeData(FurnitureTypeId, PropId);

    }

    //ˢ�»�װ����
    public void UpdateHomeChangeData(int FurnitureTypeId, int PropId)
    {
        HomeChangeData homeChangeData;
        for (int i = 0; i < homeChangeDataList.Count; i++)
        {
            homeChangeData = homeChangeDataList[i];
            if (homeChangeData.FurnitureTypeId ==  FurnitureTypeId && homeChangeData.PropId == PropId)
            {
                homeChangeData.state = 2;
            }
            else if(homeChangeData.FurnitureTypeId == FurnitureTypeId)
            {
                homeChangeData.state = 1;
            }

        }       
    }
    //������װ����
    private void AddHomeChangeData(int FurnitureTypeId, int PropId, int Floor, int state)
    {
        HomeChangeData homeChangeData = new HomeChangeData();
        homeChangeData.FurnitureTypeId = FurnitureTypeId;
        homeChangeData.PropId = PropId;
        homeChangeData.Floor = Floor;
        homeChangeData.state = state;
        homeChangeDataList.Add(homeChangeData);
    }
}
//����������������
public class SheepGameData
{
    public int currLevelID = 1;//��ǰ�ؿ�
    public int gameTime = 3;//��Ϸ����
    public int rushTime = 1;//ˢ�´���
    public int returnTime = 1;//ɾ����������
    public int withdrawTime = 1;//������һ������
    public int reviewTime = 1;//�������
}

public class GuideData
{
    public int guidetype;
    public bool guidetype_isTrue = false;//�ý׶ε����������Ƿ���� true -��� FALSE-δ���
    public bool isExecution = false;//�Ƿ�ִ�й�
}