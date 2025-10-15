using UnityEngine;


[CreateAssetMenu(fileName = "NewCharacterStatData", menuName = "Game/CharacterStatData")]
public class CharacterStatDataSO : ScriptableObject //�� SO�� �̿��Ͽ� �÷��̾�� ���� ���� �����͸� �Ҵ�
{
    public CharacterStatData[] characterStatDatas;

    public CharacterStatData GetCharacterDataById(CharacterTypeEnumByTag characterTypeEnum)
    {
        foreach (var data in characterStatDatas)
        {
            if (data.characterTypeEnum == characterTypeEnum)
                return data;
        }
        return null;
    }
}

[System.Serializable]
public class CharacterStatData
{
    public CharacterTypeEnumByTag characterTypeEnum;
    public GameObject characterPrefab;
    public float MaxHP;
    public float CurrentHP;
    public float CurrentMoveSpeed;
    public AudioClip HitSound;
    public Vector2 startPosition;                  // ���� ��ġ
}