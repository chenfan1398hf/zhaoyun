
#region RoleState��ɫ����״̬
public enum RoleState
{
    None = 0,//δ����
    Idle = 1,//����
    Run=2,//��
    Attack = 3,//����
    Hurt = 4,//����
    Death = 5//����
}
#endregion

#region RoleAnimatorName��ɫ״̬����
public enum RoleAnimatorName
{
    Idle,
    Attack,
    Hurt,
    Death,
    Run,
}
#endregion

#region RoleAnimatorChangeName�л���ɫ��������
public enum ToAnimatorCondition
{
    ToIdle,
    ToAttack,
    ToDeath,
    ToHurt,
    ToRun,
    CurrState
}
#endregion

#region ��ɫ
//ְҵ����
public enum RoleJobType
{
    ignis=1001,//��ϵ
    ice=1002,//��ϵ
    thunder=1003,//��ϵ

}
//λ��
public enum RolePosType
{
    left=1,//���
    center=2,//�м�
    right=3,//�Ҳ�
}
#endregion


#region MonsterState����״̬
public enum MonsterState
{
    None = 0,//δ����
    Idle = 1,//����
    Walk = 2,//��
    Hurt = 3,//����
    Attack = 4,//����
    Die = 5//����
}
#endregion

#region MonsterAnimatorName״̬����
public enum MonsterAnimatorName
{
    Idle,
    Walk,
    Attack,
    Hurt,
    Die,
}
#endregion

#region MonsterToAnimatorCondition�л���������
public enum MonsterToAnimatorCondition
{
    ToIdle,
    ToAttack,
    ToWalk,
    ToHurt,
    ToDie,
    CurrState
}
#endregion