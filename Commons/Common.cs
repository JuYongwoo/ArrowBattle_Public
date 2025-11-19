using UnityEngine;

namespace JYW.ArrowBattle.Commons
{
    //asmdef 상호 참조 방지를 위해 공통으로 쓰이는 자료형은 이곳에서 public으로 존재

    public enum Scenes
    {
        Load,
        StageSelect,
        Main
    }
    public class CharacterStat
    {
        public float MaxHP;
        public float CurrentHP;
        public float CurrentMoveSpeed;
        public AudioClip HitSound;

    }
    public enum SkillType
    {
        Attack,
        Skill1, Skill2, Skill3, Skill4, Skill5
    }


    public enum SkillProjectileMovingType
    {
        // 기본
        Straight,
        Parabola,
        Rain,                      // Down 여러개 스폰
        Down,                      // 머리 위 고정 스폰, 아래로 낙하만
        StraightThreeMultipleShot, // 3연속
        StraightFiveMultipleShot,  // 5연속
        FanBurst,             // 시작 지점에서 부채꼴 N발 확산(직선)
        RingBurstOut,         // 시작 지점에서 원형 N발 바깥으로
        RingBurstInToTarget,  // 타깃 머리 주변 원형 N발 안쪽으로
        LineVerticalForward,  // 시작 지점에서 상하 라인 N개 → 직선 돌진
        LineHorizontalForward,// 시작 지점에서 좌우 라인 N개 → 직선 돌진

        RandomDownRainDuration, // 랜덤 낙하
        SweepDownRainDuration,  // 좌↔우 낙하 

        SineStraight,
        ZigZagStraight,
        ScatterSplit,

        StraightSpin          // 직선 이동 + 스프라이트 지속 회전
    }

    public enum CharacterStateEnum
    {
        Idle,
        Moving,
        UsingSkill
    }



    public enum ResultStateEnum
    {
        Victory,
        Defeat
    }

    public enum CharacterTypeEnum
    {
        Player,
        Enemy
    }

}