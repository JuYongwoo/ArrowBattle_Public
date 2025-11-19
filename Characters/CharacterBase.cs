using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYW.ArrowBattle.Managers;
using JYW.ArrowBattle.Utils;
using JYW.ArrowBattle.Commons;
using JYW.ArrowBattle.SOs;

namespace JYW.ArrowBattle.Characters
{

    public abstract class CharacterBase : MonoBehaviour
    {
        [SerializeField]
        protected SkillDatasSO skillDatasSO;
        protected CharacterStateEnum state = CharacterStateEnum.Moving;
        protected Animator anim;
        private SpriteRenderer sr;
        protected Coroutine skillCoroutine;

        private readonly Dictionary<SkillType, float> _cooldownEnd = new(); //각 캐릭터마다 가지는 쿨다운 정보 딕셔너리

        protected CharacterTypeEnum characterTypeEnum;
        protected CharacterStat characterStat;
        protected SkillType castingSkill;


        public void SetCharacterStat(CharacterStatData characterStatData)
        {
            characterStat = new CharacterStat();
            characterStat.MaxHP = characterStatData.MaxHP;
            characterStat.CurrentHP = characterStatData.CurrentHP;
            characterStat.CurrentMoveSpeed = characterStatData.CurrentMoveSpeed;
            characterStat.HitSound = characterStatData.HitSound;

            gameObject.transform.position = characterStatData.StartPosition;

            characterTypeEnum = characterStatData.CharacterTypeEnum;
        }


        protected virtual void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            sr = Util.GetObjectInChildren(gameObject, "Cat").GetComponent<SpriteRenderer>();
            _cooldownEnd.Clear();

        }

        protected virtual void Start()
        {
            SetState(CharacterStateEnum.Idle);
        }

        public virtual void GetDamaged(float damageAmount)
        {
            characterStat.CurrentHP -= damageAmount;
            EventManager.Instance.OnPlayAudioClip(characterStat.HitSound, 0.3f, false);
        }

        protected virtual void SetState(CharacterStateEnum s)
        {
            if (state == s) return;
            state = s;

            if (state == CharacterStateEnum.Moving || state == CharacterStateEnum.UsingSkill)

            {
                if (skillCoroutine != null)
                {
                    StopCoroutine(skillCoroutine);
                    skillCoroutine = null;
                }
            }

            anim.SetInteger("State", (int)s);
        }

        protected virtual void Move(float moveX)
        {
            Vector2 dir = new Vector2(moveX, 0f).normalized;
            transform.Translate(dir * characterStat.CurrentMoveSpeed * Time.deltaTime, Space.World);

            if (moveX > 0.01f) sr.flipX = false;
            else if (moveX < -0.01f) sr.flipX = true;

            SetState(CharacterStateEnum.Moving);
        }

        public void PrepareSkill(SkillType skill)
        {

            sr.flipX = ABUtil.IsOpponentOnLeft(gameObject, GameObject.FindGameObjectsWithTag(ABUtil.GetOpponentEnum<CharacterTypeEnum>(characterTypeEnum).ToString()));

            if (TryBeginCooldown(skill) == false) return;

            _cooldownEnd[skill] = Time.time + skillDatasSO.GetSkillDataById(skill).SkillCoolTime;

            castingSkill = skill;

            if (skillCoroutine != null)
            {
                StopCoroutine(skillCoroutine);
                skillCoroutine = null;
            }

            SetState(CharacterStateEnum.UsingSkill);

            skillCoroutine = StartCoroutine(CastSkill(skill));
        }

        private IEnumerator CastSkill(SkillType skill)
        {

            yield return new WaitForSeconds(skillDatasSO.GetSkillDataById(skill).SkillCastingTime);

            EventManager.Instance.OnShootSkill(characterTypeEnum, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f), skill);

            skillCoroutine = null;
            PrepareSkill(SkillType.Attack);
            castingSkill = SkillType.Attack;

        }

        protected virtual bool TryBeginCooldown(SkillType skill)
        {

            if (_cooldownEnd.TryGetValue(skill, out var end) && Time.time < end) return false; //아직 쿨타임이 안끝남
            else return true;

        }

    }
}