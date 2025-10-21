using UnityEngine;

[CreateAssetMenu(fileName = "NewGameStageData", menuName = "Game/GameStageData")]
public class GameStageDataSO : ScriptableObject
{
    [SerializeField]
    private StageData[] stageDatas; //�������� 1, 2, 3, ...
    [SerializeField]
    private AudioClip victoryMusic;
    [SerializeField]
    private AudioClip defeatMusic;

    public StageData GetStageDatabyStageID(int stageId)
    {
        foreach (var data in stageDatas)
        {
            if (data.StageId == stageId)
                return data;
        }
        return null;
    }
    public AudioClip GetVictoryMusic()
    {
        return victoryMusic;
    }
    public AudioClip GetDefeatMusic()
    {
        return defeatMusic;
    }
}

[System.Serializable]
public class StageData
{
    [SerializeField]
    private int stageId; //�������� �� ID
    [SerializeField]
    private int gameTime; //������������ �ٸ� ���� �ð�
    [SerializeField]
    private AudioClip bgm; //������������ �ٸ� BGM
    [SerializeField]
    private CharacterTypeEnumByTag[] characterTypeEnum; // ������������ ��ȯ�Ǵ� ĳ���� ������(player, enemy1), (player, enemy2), ...

    public int StageId => stageId;
    public int GameTime => gameTime;
    public AudioClip Bgm => bgm;
    public CharacterTypeEnumByTag[] CharacterTypeEnum => characterTypeEnum;
}