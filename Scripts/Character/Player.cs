using UnityEngine.LowLevel;

public class Player : CharacterBase
{
    // SO ������ ��ü ����
    protected override CharacterTypeEnumByTag CharacterTypeEnum => CharacterTypeEnumByTag.Player;


    protected override void Awake()
    {
        base.Awake();
        ManagerObject.instance.eventManager.UseSkillEvent -= PrepareSkill; // InputManager�� attack �̺�Ʈ�� Attack �޼��� ����
        ManagerObject.instance.eventManager.UseSkillEvent += PrepareSkill; // InputManager�� attack �̺�Ʈ�� Attack �޼��� ����
        ManagerObject.instance.eventManager.LeftRightMoveEvent -= Move; // InputManager�� leftRightMove �̺�Ʈ�� Move �޼��� ����
        ManagerObject.instance.eventManager.LeftRightMoveEvent += Move; // InputManager�� leftRightMove �̺�Ʈ�� Move �޼��� ����
        ManagerObject.instance.eventManager.SetIdleEvent -= SetIdle;
        ManagerObject.instance.eventManager.SetIdleEvent += SetIdle;
        ManagerObject.instance.eventManager.GetCastingSkillEvent -= GetCastingKill;
        ManagerObject.instance.eventManager.GetCastingSkillEvent += GetCastingKill;
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
        ManagerObject.instance.eventManager.UseSkillEvent -= PrepareSkill; // InputManager�� attack �̺�Ʈ�� Attack �޼��� ����
        ManagerObject.instance.eventManager.LeftRightMoveEvent -= Move; // InputManager�� leftRightMove �̺�Ʈ�� Move �޼��� ����
        ManagerObject.instance.eventManager.SetIdleEvent -= SetIdle;
        ManagerObject.instance.eventManager.GetCastingSkillEvent -= GetCastingKill;
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
        ManagerObject.instance.eventManager.OnSetPlayerHPInUI(Stat.Current.CurrentHP, Stat.Current.MaxHP);
        if (Stat.Current.CurrentHP <= 0)
        {
            ManagerObject.instance.eventManager.OnEndGame(ResultStateEnum.Defeat);
        }
    }

    protected override bool TryBeginCooldown(Skill skill)
    {
        if (base.TryBeginCooldown(skill)) //�θ� �����ϰ�
        {
            //UI �߰����� �� true ��ȯ
            ManagerObject.instance.eventManager.OnCooldownUI((int)skill, ManagerObject.instance.resourceManager.SkillDatas.Result.GetSkillDataById(skill).skillCoolTime);
            return true;
        }
        else
        {
            return false;
        }
    }

}
