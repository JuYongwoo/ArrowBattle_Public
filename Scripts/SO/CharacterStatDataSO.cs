using UnityEngine;


[CreateAssetMenu(fileName = "NewCharacterStatData", menuName = "Game/CharacterStatData")]
public class CharacterStatDataSO : ScriptableObject //�� SO�� �̿��Ͽ� �÷��̾�� ���� ���� �����͸� �Ҵ�
{
    public float MaxHP;
    public float CurrentHP;
    public float CurrentMoveSpeed;
    public AudioClip HitSound;

}