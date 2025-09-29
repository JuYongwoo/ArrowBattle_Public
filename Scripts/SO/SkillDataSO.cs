using UnityEngine;

[System.Serializable]




[CreateAssetMenu(fileName = "NewSkillData", menuName = "Game/SkillData")]
public class SkillDataSO : ScriptableObject //�� SO�� �̿��Ͽ� �� ���� ��ų �����͸� �Ҵ�
{

    public GameObject skillProjectile; //��ų ����ü ������
    public Sprite skillIcon; //��ų ������ �̹���
    public AudioClip skillSound; //��ų ����
    public float skillDamage; //��ų ������
    public float skillCoolTime; //��ų ��Ÿ��
    public float skillCastingTime; //��ų ���� �ð�
    public float projectileSpeed;// ����ü �ӵ�

    public SkillProjectileMovingType skillProjectileMovingType; //��ų ����ü �̵� Ÿ��, ������, ������

}