using UnityEngine;

public class InputManager
{

    public void OnUpdate()
    {


        if (ManagerObject.instance.actionManager.getCastingSkill?.Invoke() != Skill.Attack) return; //�Ϲ� ���� �� �ٸ� ��ų ĳ���� ���̶�� ��� �����ӵ� X
                                                                                            // ��ų �Է� �켱
        if (Input.GetKeyDown(KeyCode.Q)) {
            ManagerObject.instance.actionManager.useSkill?.Invoke(Skill.Skill1); return;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            ManagerObject.instance.actionManager.useSkill?.Invoke(Skill.Skill2); return;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            ManagerObject.instance.actionManager.useSkill?.Invoke(Skill.Skill3); return;
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ManagerObject.instance.actionManager.useSkill?.Invoke(Skill.Skill4); return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            ManagerObject.instance.actionManager.useSkill?.Invoke(Skill.Skill5); return;
        }

        // �̵�/Idle
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX != 0f) {
            ManagerObject.instance.actionManager.leftRightMove?.Invoke(moveX); return;
        }

        ManagerObject.instance.actionManager.idle?.Invoke();


    }
}
