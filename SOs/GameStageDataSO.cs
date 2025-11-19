using JYW.ArrowBattle.Commons;
using UnityEngine;

namespace JYW.ArrowBattle.SOs
{

    [CreateAssetMenu(fileName = "NewGameStageData", menuName = "Game/GameStageData")]
    public class GameStageDataSO : ScriptableObject
    {
        [SerializeField]
        private StageData[] stageDatas; //스테이지 1, 2, 3, ...
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
        private int stageId; //스테이지 별 ID
        [SerializeField]
        private int gameTime; //스테이지마다 다른 게임 시간
        [SerializeField]
        private AudioClip bgm; //스테이지마다 다른 BGM
        [SerializeField]
        CharacterStatData playerData;
        [SerializeField]
        CharacterStatData enemyData;



        public int StageId => stageId;
        public int GameTime => gameTime;
        public AudioClip Bgm => bgm;
        public CharacterStatData PlayerData => playerData;
        public CharacterStatData EnemyData => enemyData;
    }


    [System.Serializable]
    public class CharacterStatData //TODO JYW 어떤 스킬을 사용할 것인지 리스트로 추가할 수 있음(그 전에 여러가지 사용 가능한 스킬들을 미리 정의해놔야)
    {
        [SerializeField]
        private CharacterTypeEnum characterTypeEnum;
        [SerializeField]
        private GameObject characterPrefab;
        [SerializeField]
        private float maxHP;
        [SerializeField]
        private float currentHP;
        [SerializeField]
        private float currentMoveSpeed;
        [SerializeField]
        private AudioClip hitSound;
        [SerializeField]
        private Vector2 startPosition;                  // 시작 위치

        public CharacterTypeEnum CharacterTypeEnum => characterTypeEnum;
        public GameObject CharacterPrefab => characterPrefab;
        public float MaxHP => maxHP;
        public float CurrentHP => currentHP;
        public float CurrentMoveSpeed => currentMoveSpeed;
        public AudioClip HitSound => hitSound;
        public Vector2 StartPosition => startPosition;
    }
}