using JYW.ArrowBattle.Commons;
using JYW.ArrowBattle.Managers;
using JYW.ArrowBattle.SOs;
namespace JYW.ArrowBattle.Characters
{
    public class Player : CharacterBase
    {
        // SO 값으로 교체 예정

        protected override void Awake()
        {
            base.Awake();
            EventManager.Instance.UseSkillEvent -= PrepareSkill; // InputManager의 attack 이벤트에 Attack 메서드 구독
            EventManager.Instance.UseSkillEvent += PrepareSkill; // InputManager의 attack 이벤트에 Attack 메서드 구독
            EventManager.Instance.LeftRightMoveEvent -= Move; // InputManager의 leftRightMove 이벤트에 Move 메서드 구독
            EventManager.Instance.LeftRightMoveEvent += Move; // InputManager의 leftRightMove 이벤트에 Move 메서드 구독
            EventManager.Instance.SetIdleEvent -= SetIdle;
            EventManager.Instance.SetIdleEvent += SetIdle;
            EventManager.Instance.GetCastingSkillEvent -= GetCastingKill;
            EventManager.Instance.GetCastingSkillEvent += GetCastingKill;
        }


        protected override void Start()
        {
            base.Start();
            EventManager.Instance.OnSetPlayerHPInUI(characterStat.CurrentHP, characterStat.MaxHP);

        }
        private void SetIdle()
        {
            SetState(CharacterStateEnum.Idle);
        }

        private SkillType GetCastingKill()
        {
            return castingSkill;
        }

        private void OnDestroy()
        {
            EventManager.Instance.UseSkillEvent -= PrepareSkill; // InputManager의 attack 이벤트에 Attack 메서드 구독
            EventManager.Instance.LeftRightMoveEvent -= Move; // InputManager의 leftRightMove 이벤트에 Move 메서드 구독
            EventManager.Instance.SetIdleEvent -= SetIdle;
            EventManager.Instance.GetCastingSkillEvent -= GetCastingKill;
        }

        protected void Update()
        {
            if (state == CharacterStateEnum.Idle) //아무 행동 하지 않을 시 일반 공격
            {
                if (skillCoroutine == null) PrepareSkill(SkillType.Attack);
            }
        }

        public override void GetDamaged(float damageAmount)
        {
            base.GetDamaged(damageAmount);
            EventManager.Instance.OnSetPlayerHPInUI(characterStat.CurrentHP, characterStat.MaxHP);

            if (characterStat.CurrentHP <= 0)
            {
                EventManager.Instance.OnEndGame(ResultStateEnum.Defeat);
            }
        }


        protected override bool TryBeginCooldown(SkillType skill)
        {
            if (base.TryBeginCooldown(skill)) //부모 실행하고
            {
                //UI 추가실행 후 true 반환
                EventManager.Instance.OnCooldownUI((int)skill, skillDatasSO.GetSkillDataById(skill).SkillCoolTime);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}