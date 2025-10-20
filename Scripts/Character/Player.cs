using UnityEngine.LowLevel;

public class Player : CharacterBase
{
    // SO ������ ��ü ����
    protected override CharacterTypeEnumByTag CharacterTypeEnum => CharacterTypeEnumByTag.Player;


    protected override void Awake()
    {
        base.Awake();
        ManagerObject.instance.actionManager.UseSkillEvent -= PrepareSkill; // InputManager�� attack �̺�Ʈ�� Attack �޼��� ����
        ManagerObject.instance.actionManager.UseSkillEvent += PrepareSkill; // InputManager�� attack �̺�Ʈ�� Attack �޼��� ����
        ManagerObject.instance.actionManager.LeftRightMoveEvent -= Move; // InputManager�� leftRightMove �̺�Ʈ�� Move �޼��� ����
        ManagerObject.instance.actionManager.LeftRightMoveEvent += Move; // InputManager�� leftRightMove �̺�Ʈ�� Move �޼��� ����
        ManagerObject.instance.actionManager.SetIdleEvent -= SetIdle;
        ManagerObject.instance.actionManager.SetIdleEvent += SetIdle;
        ManagerObject.instance.actionManager.GetCastingSkillEvent -= GetCastingKill;
        ManagerObject.instance.actionManager.GetCastingSkillEvent += GetCastingKill;
    }

    private void SetIdle()
    {
        SetState(CharacterStateEnum.Idle);
    }
    
    private Skill GetCastingKill()
    {
        return castingSkill;
    }

    private void OnDestroy()
    {
        ManagerObject.instance.actionManager.UseSkillEvent -= PrepareSkill; // InputManager�� attack �̺�Ʈ�� Attack �޼��� ����
        ManagerObject.instance.actionManager.LeftRightMoveEvent -= Move; // InputManager�� leftRightMove �̺�Ʈ�� Move �޼��� ����
        ManagerObject.instance.actionManager.SetIdleEvent -= SetIdle;
        ManagerObject.instance.actionManager.GetCastingSkillEvent -= GetCastingKill;
    }

    protected void Update()
    {
        if (state == CharacterStateEnum.Idle) //�ƹ� �ൿ ���� ���� �� �Ϲ� ����
        {
            if (skillCoroutine == null) PrepareSkill(Skill.Attack);
        }
    }

    public override void GetDamaged(float damageAmount)
    {
        base.GetDamaged(damageAmount); // CharacterBase�� getDamaged() ȣ��
        ManagerObject.instance.actionManager.OnSetPlayerHPInUI(Stat.Current.CurrentHP, Stat.Current.MaxHP);
        if (Stat.Current.CurrentHP <= 0)
        {
            ManagerObject.instance.actionManager.OnEndGame(ResultStateEnum.Defeat);
        }
    }

    protected override bool TryBeginCooldown(Skill skill)
    {
        if (base.TryBeginCooldown(skill)) //�θ� �����ϰ�
        {
            //UI �߰����� �� true ��ȯ
            ManagerObject.instance.actionManager.OnCooldownUI((int)skill, ManagerObject.instance.resourceManager.SkillDatas.Result.GetSkillDataById(skill).skillCoolTime);
            return true;
        }
        else
        {
            return false;
        }
    }

}
