using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateAbstract 
{
    public CharacterFSMMgr currFSMMgr { get; private set; } //��ǰ��ɫ��״̬������
    public AnimatorStateInfo currAnimatorStateInfo { get; set; }//��ǰ����״̬��Ϣ
    //���캯��
    public CharacterStateAbstract(CharacterFSMMgr currFSMMgr)
    {
        this.currFSMMgr = currFSMMgr;
    }
    public virtual void OnEnter() { }//����״̬

    public virtual void OnEnter(RoleAnimatorName name) { }//����״̬

    public virtual void OnUpdate() { }//ִ��״̬

    public virtual void OnLeave() { }//�뿪״̬
}
