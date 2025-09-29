using UnityEngine;


[CreateAssetMenu(fileName = "NewGameModeData", menuName = "Game/GameModeData")]
public class GameModeDataSO : ScriptableObject //�ܼ� ���� ���� �� ������ ��� SO ����
{
    [System.Serializable]
    public class CharacterDatas
    {
        public GameObject characterPrefab;
        public Vector2 characterStartPosition;
    }

    public CharacterDatas[] Characters;
    public int GameTime; //���� �ð�
    public AudioClip BGM;
    public AudioClip VictoryMusic;
    public AudioClip DefeatMusic;

}