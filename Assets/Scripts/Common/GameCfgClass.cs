using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ��Ϸ����������
/// </summary>
public class LanguageCfg
{
    public string key="";                             //����ֵ
    public string content = "";                       //��������
}

/// <summary>
/// �������ñ�
/// </summary>
public class TaskInfoCfg
{
    public int ID;						    //ID
    public string system;					//����ϵͳ
    public string describe;				    //����
    public long targetNumber;			    //Ŀ����
    public int prepositionTaskID;		    //ǰ������ID
    public string award;					//����
    public int type;					    //���ͣ�1�ճ�2�ܳ�3�ɳ�4������
    public int open;					    //���أ�0�ر�1������
    public int jump;					    //��תĿ�ģ�0û����ת��
    public int iocn;					    //ͼ��
    public int gameID;					    //��Ӧ����Ϸid
    public int rank;					    //����
    public int frame;					    //���
    public int numberType;				    //�����ۼ����ͣ�1������2�ۻ����ͣ�
    public int taskType;				    //�������Ϳͻ��˷�����
    public string awardVip;					//ս���
    public int needUnlock;                  //�Ƿ���Ҫ������0����Ҫ1��Ҫ��
    public int vitality;                    //��Ծ��չʾ
}

