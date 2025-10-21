using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    protected override CharacterTypeEnumByTag CharacterTypeEnum => CharacterTypeEnumByTag.Enemy;

    private float moveDuration = 3f;
    private float moveInterval = 3.1f;

    private Coroutine moveLoop;


    protected override void Start()
    {
        base.Start();
        moveLoop = StartCoroutine(CoMove());
        InvokeRepeating(nameof(RandomSkill), 0f, 1.1f); //���� ��ų ���
    }
    protected void Update()
    {
        if (state == CharacterStateEnum.Idle) //�ƹ� �ൿ ���� ���� �� �Ϲ� ����
        {
            if (skillCoroutine == null) PrepareSkill(Skill.Attack);
        }
    }
    private void OnDestroy()
    {
        if (moveLoop != null) StopCoroutine(moveLoop);
    }
    
    private void RandomSkill()
    {
        if( state == CharacterStateEnum.Moving) return; //�����϶� ��ų ��� ����
        List<int> skillsList = new List<int>() { 1, 2, 3, 4, 5 }; //Attack, Skill1, Skill2
        System.Random random = new System.Random();
        Skill randomSkill = (Skill)random.Next(skillsList.Count);
        PrepareSkill(randomSkill);
    }

    public override void GetDamaged(float damageAmount)
    {
        base.GetDamaged(damageAmount); // CharacterBase�� getDamaged() ȣ��
        ManagerObject.instance.eventManager.OnSetEnemyHPInUI(Stat.Current.CurrentHP, Stat.Current.MaxHP);
        if (Stat.Current.CurrentHP <= 0)
        {
            ManagerObject.instance.eventManager.OnEndGame(ResultStateEnum.Victory);
        }
    }

    private IEnumerator CoMove()
    {
        float dir = -1f;
        WaitForFixedUpdate waitFixed = new WaitForFixedUpdate();

        while (true)
        {
            float t = 0f;
            while (t < moveDuration)
            {
                Move(dir);
                yield return waitFixed;
                t += Time.fixedDeltaTime;
            }
            SetState(CharacterStateEnum.Idle);
            yield return new WaitForSeconds(moveInterval);
            dir = -dir;
        }
    }

}
