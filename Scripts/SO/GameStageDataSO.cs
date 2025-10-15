using UnityEngine;

[CreateAssetMenu(fileName = "NewGameStageData", menuName = "Game/GameStageData")]
public class GameStageDataSO : ScriptableObject
{
    public StageData GetStageDatabyStageID(int stageId)
    {
        foreach (var data in stageDatas)
        {
            if (data.stageId == stageId)
                return data;
        }
        return null;
    }

    public StageData[] stageDatas; //�������� 1, 2, 3, ...
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;
}

[System.Serializable]
public class StageData
{
    public int stageId; //�������� �� ID
    public int gameTime; //������������ �ٸ� ���� �ð�
    public AudioClip bgm; //������������ �ٸ� BGM
    public CharacterTypeEnumByTag[] characterTypeEnum; // ������������ ��ȯ�Ǵ� ĳ���� ������(player, enemy1), (player, enemy2), ...
}