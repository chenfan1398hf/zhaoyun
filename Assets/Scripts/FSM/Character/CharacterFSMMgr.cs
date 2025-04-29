using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFSMMgr
{
    public CharacterAnimationCtrl charactorCtrl=null;//��ɫ������
    public RoleState currStateEnum { get; private set; }//��ǰ��ɫ��״̬ö������
    public CharacterStateAbstract currStateAbstract = null;//��ǰ�Ľ�ɫ����״̬
    private Dictionary<RoleState, CharacterStateAbstract> mStateDic;//��ɫ�����ֵ�

    public CharacterFSMMgr(CharacterAnimationCtrl  _Ctrl)
    {
        charactorCtrl = _Ctrl;
        mStateDic = new Dictionary<RoleState, CharacterStateAbstract>();
        //ע��״̬�������ֵ���
        mStateDic[RoleState.Idle] = new CharacterStateIdle(this);
        mStateDic[RoleState.Attack] = new CharacterStateAttack(this);
        mStateDic[RoleState.Run] = new CharacterStateRun(this);
        mStateDic[RoleState.Death] = new CharacterStateDeath(this);
        mStateDic[RoleState.Hurt] = new CharacterStateHurt(this);

        //��ʼ����ǰ״̬
        if (mStateDic.ContainsKey(currStateEnum))
        {
            currStateAbstract = mStateDic[currStateEnum];
        }
    }

    #region OnUpdate() ��֤����ÿִ֡��
    public void OnUpdate()
    {
        if (currStateAbstract != null)
        {
            currStateAbstract.OnUpdate();
        }
    }
    #endregion

    #region  ChangeState(RoleState newState) �ı��ɫ״̬
    /// <summary>
    /// ChangeState(RoleState newState) �ı��ɫ״̬
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(RoleState newState)
    {
        if (currStateAbstract != null)//��ǰ״̬δ�������ý���
        {
            currStateAbstract.OnLeave();
        }
        currStateEnum = newState;//���µ�ǰ��״̬ö��
        currStateAbstract = mStateDic[newState];//�����µĶ���
        currStateAbstract.OnEnter();//��ʼִ���µĶ���
        //Debug.Log("��ɫ__"+roleCtrl.jobType+"__ִ���µĶ�����" + newState.ToString());
    }
    public void ChangeState(RoleState newState, RoleAnimatorName name)
    {
        if (currStateAbstract != null)//��ǰ״̬δ�������ý���
        {
            currStateAbstract.OnLeave();
        }
        currStateEnum = newState;//���µ�ǰ��״̬ö��
        currStateAbstract = mStateDic[newState];//�����µĶ���
        currStateAbstract.OnEnter(name);//��ʼִ���µĶ���
        //Debug.Log("ִ���µĶ�����" + newState.ToString());
    }
    #endregion
}
