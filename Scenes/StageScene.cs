using System;
using System.Collections;
using UnityEngine;
using JYW.ArrowBattle.Managers;
using JYW.ArrowBattle.Characters;
using JYW.ArrowBattle.Commons;
using JYW.ArrowBattle.SOs;
using JYW.ArrowBattle.Skills;

namespace JYW.ArrowBattle.Scenes
{
    public class StageScene : MonoBehaviour
    {
        //public GameObject player; //현재 씬의 플레이어
        //public GameObject enemy; //현재 씬의 적

        [SerializeField]
        private GameStageDataSO gameStageDataSO;
        [SerializeField]
        private SkillDatasSO skillDatasSO;

        private int gameLeftTime = 99;
        private TimerRunner _timerRunner;




        ////////////////////// JYW TODO: SO로 옮기기(Editor사용하여 선택한 type 값에 따라 수치 칸 보이도록)
        private float headYOffset = 1.0f;

        // StraightThreeMultipleShot
        private int tripleShots = 3;
        private float tripleInterval = 0.12f;

        // StraightFiveMultipleShot
        private int fiveShots = 5;
        private float fiveInterval = 0.10f;

        // Parabola
        private float parabolaHeightBase = 3.5f;

        // Rain/Down
        private int rainDownCount = 6;
        private float downSpawnHeight = 5f;
        private float downWidth = 4f;
        private float downFallSpeedMul = 1f;

        // RandomDownRainDuration
        private float randomRainDuration = 5.0f;
        private float randomRainRatePerSec = 8.0f;
        private float randomRainWidth = 6.0f;
        private float randomRainSpawnH = 6.0f;
        private float randomRainFallMul = 1.0f;

        // SweepDownRainDuration
        private float sweepRainDuration = 5.0f;
        private float sweepRainRatePerSec = 8.0f;
        private float sweepRainWidth = 6.0f;
        private float sweepRainSpeed = 3.0f;
        private float sweepRainSpawnH = 6.0f;
        private float sweepRainFallMul = 1.0f;

        // FanBurst
        private int fanCount = 7;
        private float fanAngle = 60f;

        // RingBurst
        private int ringCount = 12;
        private float ringRadius = 7.0f;

        // LineForward
        private int lineCount = 5;
        private float lineSpacing = 0.6f;

        // Sine/ZigZag
        private float sineAmplitude = 0.6f;
        private float sineFrequencyHz = 2.5f;
        private float zigzagAmplitude = 0.6f;
        private float zigzagFrequencyHz = 3.0f;

        // ScatterSplit
        private float scatterDelaySec = 0.45f;
        private int scatterCount = 12;
        private float scatterFanAngle = 120f;

        // StraightSpin
        private float spinningStraightSpinSpeedDeg = -1440f; // 도/초

        //////////////////////

        // 코루틴 러너 (옵션)
        private MonoBehaviour _runner;

        private void Awake()
        {

            EventManager.Instance.ShootSkillEvent -= Shoot;
            EventManager.Instance.ShootSkillEvent += Shoot;
            EventManager.Instance.EndGameEvent -= EndGame; // ActionManager의 endGame 이벤트에 endGame 메서드 구독
            EventManager.Instance.EndGameEvent += EndGame;

        }

        private void Start()
        {
            Loadstage(EventManager.Instance.OnGetStageNumber()+1); //지금은 GetStageDatabyStageID(1) 로 스테이지 1만 실행, 클리어 시 GetStageDatabyStageID(2)로 스테이지 정보 불러오도록 함

            if (_timerRunner == null)
            {
                var go = new GameObject("__GameModeTimer");
                DontDestroyOnLoad(go);
                _timerRunner = go.AddComponent<TimerRunner>();
            }

            _timerRunner.StartRepeating(FlowTime, 1f);
            // _timerRunner.StartRepeatingRealtime(flowTime, 1f); //timeScale 무시

        }

        private void OnDestroy()
        {
            EventManager.Instance.ShootSkillEvent -= Shoot;
            EventManager.Instance.EndGameEvent -= EndGame;

        }

        private void FlowTime()
        {
            gameLeftTime--;

            EventManager.Instance.OnSetGameTimeUI(gameLeftTime); //TimePanel의 시간을 세팅하는 델리게이트 호출
            if (gameLeftTime <= 0)
            {
                EndGame(ResultStateEnum.Defeat); //시간 종료로 패배
            }
        }

        private void Loadstage(int stageID) //캐릭터들을 생성하고 캐릭터들의 스탯을 SO 값에따라 setStatus해준다.
        {

            //////////////SO 내의 playerData를 통해 적 생성
            GameObject player = Instantiate(gameStageDataSO.GetStageDatabyStageID(stageID).PlayerData.CharacterPrefab);
            player.GetComponent<CharacterBase>().SetCharacterStat(gameStageDataSO.GetStageDatabyStageID(stageID).PlayerData);

            GameObject enemy = Instantiate(gameStageDataSO.GetStageDatabyStageID(stageID).EnemyData.CharacterPrefab);
            enemy.GetComponent<CharacterBase>().SetCharacterStat(gameStageDataSO.GetStageDatabyStageID(stageID).EnemyData);

            //////////////SO 내의 enemyData를 통해 적 생성


            /////////////

            //BGM 재생
            EventManager.Instance.OnPlayAudioClip(gameStageDataSO.GetStageDatabyStageID(stageID).Bgm, 0.2f, true);

            gameLeftTime = gameStageDataSO.GetStageDatabyStageID(stageID).GameTime;
        }

        private void EndGame(ResultStateEnum resultStateEnum)
        {
            EventManager.Instance.OnStopAllAudioClip();
            // 반복 중지
            if (_timerRunner != null)
                _timerRunner.StopRepeating();

            if (resultStateEnum == ResultStateEnum.Victory) EventManager.Instance.OnPlayAudioClip(gameStageDataSO.GetVictoryMusic(), 0.3f, false);
            else EventManager.Instance.OnPlayAudioClip(gameStageDataSO.GetDefeatMusic(), 0.2f, false);

            Time.timeScale = 0f; //게임 일시정지
            EventManager.Instance.OnGameResultUI(resultStateEnum); //ResultPanel의 UI를 세팅하는 델리게이트 호출
        }

        public sealed class TimerRunner : MonoBehaviour
        {
            private Coroutine _loop;

            public void StartRepeating(Action callback, float intervalSeconds)
            {
                StopRepeating();
                _loop = StartCoroutine(Co_Repeat(callback, intervalSeconds));
            }

            public void StartRepeatingRealtime(Action callback, float intervalSeconds)
            {
                StopRepeating();
                _loop = StartCoroutine(Co_RepeatRealtime(callback, intervalSeconds));
            }

            public void StopRepeating()
            {
                if (_loop != null)
                {
                    StopCoroutine(_loop);
                    _loop = null;
                }
            }

            private IEnumerator Co_Repeat(Action callback, float interval)
            {
                var wait = new WaitForSeconds(interval); // timeScale 영향 받음
                while (true)
                {
                    callback?.Invoke();
                    yield return wait;
                }
            }

            private IEnumerator Co_RepeatRealtime(Action callback, float interval)
            {
                while (true)
                {
                    callback?.Invoke();
                    yield return new WaitForSecondsRealtime(interval); // timeScale 무시
                }
            }
        }





        public MonoBehaviour GetRunner()
        {
            if (_runner != null) return _runner;
            return SkillCoroutineHub.Instance;
        }



        static Vector3 DirFromTo(Vector3 from, Vector3 to)
        {
            var v = to - from;
            return (v.sqrMagnitude > 1e-6f) ? v.normalized : Vector3.right;
        }

        static float TriWave(float t) // -1..1
        {
            float v = Mathf.PingPong(t, 1f); // 0..1..0
            return (v * 2f - 1f);            // -1..1..-1
        }

        private Transform FindTargetForCaster(CharacterTypeEnum casterType)
        {
            var values = Enum.GetValues(typeof(CharacterTypeEnum));
            string enemyTag = Enum.GetName(
                typeof(CharacterTypeEnum),
                ((int)casterType + 1) % values.Length
            );
            var go = GameObject.FindGameObjectWithTag(enemyTag);
            return go != null ? go.transform : null;
        }

        // 매니저 전용 생성기 (유일한 Instantiate 경로)
        private SkillProjectile SpawnProjectile(CharacterTypeEnum casterType, Vector3 pos, SkillData so, Action<SkillProjectile, object[]> init, Action<SkillProjectile, object[]> tick, params object[] args)
        {

            var p = PoolManager.Instance.Spawn(so.SkillProjectile, pos, Quaternion.identity).GetComponent<SkillProjectile>();

            p.SetProjectile(casterType, so.SkillDamage, so.ProjectileSpeed, init, tick, args);
            return p;
        }

        public void Shoot(CharacterTypeEnum casterType, Vector3 startPosition, SkillType skill)
        {
            var so = skillDatasSO.GetSkillDataById(skill);
            var targetTr = FindTargetForCaster(casterType);
            var targetPos = (targetTr != null) ? targetTr.position : (startPosition + Vector3.right * 8f);
            var forward = DirFromTo(startPosition, targetPos);

            EventManager.Instance.OnPlayAudioClip(skillDatasSO.GetSkillDataById(skill).SkillSound, 0.2f, false);

            switch (so.SkillProjectileMovingType)
            {
                case SkillProjectileMovingType.Straight:
                    Spawn_Straight(casterType, so, startPosition, forward);
                    break;

                case SkillProjectileMovingType.Parabola:
                    Spawn_Parabola(casterType, so, startPosition, parabolaHeightBase);
                    break;

                case SkillProjectileMovingType.StraightThreeMultipleShot:
                    Spawn_StraightBurst(casterType, so, startPosition, forward, tripleShots, tripleInterval);
                    break;

                case SkillProjectileMovingType.StraightFiveMultipleShot:
                    Spawn_StraightBurst(casterType, so, startPosition, forward, fiveShots, fiveInterval);
                    break;

                case SkillProjectileMovingType.Rain:
                    Spawn_Rain(casterType, so, startPosition, targetPos);
                    break;

                case SkillProjectileMovingType.Down:
                    Spawn_DownSingle(casterType, so, targetPos);
                    break;

                case SkillProjectileMovingType.FanBurst:
                    Spawn_FanBurst(casterType, so, startPosition, forward);
                    break;

                case SkillProjectileMovingType.RingBurstOut:
                    Spawn_RingBurstOut(casterType, so, startPosition);
                    break;

                case SkillProjectileMovingType.RingBurstInToTarget:
                    Spawn_RingBurstInToTarget(casterType, so, targetPos);
                    break;

                case SkillProjectileMovingType.LineVerticalForward:
                    Spawn_LineVerticalForward(casterType, so, startPosition, forward);
                    break;

                case SkillProjectileMovingType.LineHorizontalForward:
                    Spawn_LineHorizontalForward(casterType, so, startPosition, forward);
                    break;

                case SkillProjectileMovingType.RandomDownRainDuration:
                    GetRunner().StartCoroutine(Co_RandomDownRainDuration(casterType, so, targetPos));
                    break;

                case SkillProjectileMovingType.SweepDownRainDuration:
                    GetRunner().StartCoroutine(Co_SweepDownRainDuration(casterType, so, targetPos));
                    break;

                case SkillProjectileMovingType.SineStraight:
                    Spawn_SineStraight(casterType, so, startPosition, forward);
                    break;

                case SkillProjectileMovingType.ZigZagStraight:
                    Spawn_ZigZagStraight(casterType, so, startPosition, forward);
                    break;

                case SkillProjectileMovingType.ScatterSplit:
                    Spawn_ScatterSplit(casterType, so, startPosition, forward);
                    break;

                case SkillProjectileMovingType.StraightSpin:
                    Spawn_StraightSpin(casterType, so, startPosition, forward, spinningStraightSpinSpeedDeg);
                    break;

                default:
                    Spawn_Straight(casterType, so, startPosition, forward);
                    break;
            }
        }
        void Spawn_Straight(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 dir)
        {
            SpawnProjectile(caster, pos, so, init_Straight, tick_Straight, dir);
        }

        void Spawn_Parabola(CharacterTypeEnum caster, SkillData so, Vector3 pos, float heightBase)
        {
            SpawnProjectile(caster, pos, so, init_Parabola, tick_Parabola, heightBase);
        }

        void Spawn_StraightSpin(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 dir, float spinDegPerSec)
        {
            SpawnProjectile(caster, pos, so, init_StraightSpin, tick_StraightSpin, dir, spinDegPerSec);
        }

        void Spawn_StraightBurst(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 dir, int shots, float interval)
        {
            shots = Mathf.Max(1, shots);
            // 첫 발
            Spawn_Straight(caster, so, pos, dir);
            // 나머지
            if (shots > 1)
                GetRunner().StartCoroutine(Co_StraightBurst(caster, so, pos, dir, shots - 1, interval));
        }

        IEnumerator Co_StraightBurst(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 dir, int remain, float interval)
        {
            for (int i = 0; i < remain; i++)
            {
                yield return new WaitForSeconds(interval);
                Spawn_Straight(caster, so, pos, dir);
            }
        }

        void Spawn_Rain(CharacterTypeEnum caster, SkillData so, Vector3 startPos, Vector3 targetPos)
        {
            int count = Mathf.Max(1, rainDownCount);
            float width = downWidth;
            float spawnH = downSpawnHeight;

            Vector3 centerBase = targetPos + Vector3.up * headYOffset;
            for (int i = 0; i < count; i++)
            {
                float u = (i + 0.5f) / count;
                float xOff = Mathf.Lerp(-width * 0.5f, width * 0.5f, u);
                Vector3 spawnPos = new Vector3(centerBase.x + xOff, centerBase.y + spawnH, startPos.z);
                SpawnProjectile(caster, spawnPos, so, init_Down, tick_Down, centerBase.y, centerBase.x + xOff, downFallSpeedMul);
            }
        }

        void Spawn_DownSingle(CharacterTypeEnum caster, SkillData so, Vector3 targetPos)
        {
            Vector3 centerBase = targetPos + Vector3.up * headYOffset;
            Vector3 spawnPos = new Vector3(centerBase.x, centerBase.y + downSpawnHeight, targetPos.z);
            SpawnProjectile(caster, spawnPos, so, init_Down, tick_Down, centerBase.y, centerBase.x, downFallSpeedMul);
        }

        void Spawn_FanBurst(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 forward)
        {
            int N = Mathf.Max(1, fanCount);
            float total = fanAngle;
            float step = (N <= 1) ? 0f : total / (N - 1);
            float start = -total * 0.5f;

            for (int i = 0; i < N; i++)
            {
                float ang = start + step * i;
                Vector3 dir = Quaternion.AngleAxis(ang, Vector3.forward) * forward;
                Spawn_Straight(caster, so, pos, dir);
            }
        }

        void Spawn_RingBurstOut(CharacterTypeEnum caster, SkillData so, Vector3 center)
        {
            int N = Mathf.Max(1, ringCount);
            for (int i = 0; i < N; i++)
            {
                float theta = (i / (float)N) * Mathf.PI * 2f;
                Vector3 pos = center + new Vector3(Mathf.Cos(theta) * ringRadius, Mathf.Sin(theta) * ringRadius, 0f);
                Vector3 dir = (pos - center).normalized;
                Spawn_Straight(caster, so, pos, dir);
            }
        }

        void Spawn_RingBurstInToTarget(CharacterTypeEnum caster, SkillData so, Vector3 targetPos)
        {
            int N = Mathf.Max(1, ringCount);
            Vector3 center = targetPos + Vector3.up * headYOffset;
            for (int i = 0; i < N; i++)
            {
                float theta = (i / (float)N) * Mathf.PI * 2f;
                Vector3 pos = center + new Vector3(Mathf.Cos(theta) * ringRadius, Mathf.Sin(theta) * ringRadius, 0f);
                Vector3 dir = (center - pos).normalized;
                Spawn_Straight(caster, so, pos, dir);
            }
        }

        void Spawn_LineVerticalForward(CharacterTypeEnum caster, SkillData so, Vector3 start, Vector3 forward)
        {
            int N = Mathf.Max(1, lineCount);
            int mid = (N - 1) / 2;
            for (int i = 0; i < N; i++)
            {
                int off = i - mid;
                Vector3 pos = start + Vector3.up * (off * lineSpacing);
                Spawn_Straight(caster, so, pos, forward);
            }
        }

        void Spawn_LineHorizontalForward(CharacterTypeEnum caster, SkillData so, Vector3 start, Vector3 forward)
        {
            int N = Mathf.Max(1, lineCount);
            int mid = (N - 1) / 2;
            for (int i = 0; i < N; i++)
            {
                int off = i - mid;
                Vector3 pos = start + Vector3.right * (off * lineSpacing);
                Spawn_Straight(caster, so, pos, forward);
            }
        }

        IEnumerator Co_RandomDownRainDuration(CharacterTypeEnum caster, SkillData so, Vector3 targetPos)
        {
            float tLeft = Mathf.Max(0.01f, randomRainDuration);
            float interval = 1f / Mathf.Max(0.01f, randomRainRatePerSec);

            Vector3 center = targetPos + Vector3.up * headYOffset;

            while (tLeft > 0f)
            {
                float xOff = UnityEngine.Random.Range(-randomRainWidth * 0.5f, randomRainWidth * 0.5f);
                Vector3 spawnPos = new Vector3(center.x + xOff, center.y + randomRainSpawnH, targetPos.z);
                SpawnProjectile(caster, spawnPos, so, init_Down, tick_Down, center.y, center.x + xOff, randomRainFallMul);

                yield return new WaitForSeconds(interval);
                tLeft -= interval;
            }
        }

        IEnumerator Co_SweepDownRainDuration(CharacterTypeEnum caster, SkillData so, Vector3 targetPos)
        {
            float tLeft = Mathf.Max(0.01f, sweepRainDuration);
            float interval = 1f / Mathf.Max(0.01f, sweepRainRatePerSec);

            Vector3 center = targetPos + Vector3.up * headYOffset;
            float baseX = center.x - sweepRainWidth * 0.5f;

            float elapsed = 0f;
            while (tLeft > 0f)
            {
                float traveled = (elapsed * sweepRainSpeed) % (sweepRainWidth * 2f);
                float offset = (traveled <= sweepRainWidth) ? traveled : (2f * sweepRainWidth - traveled);
                float cx = baseX + offset;

                float jitter = UnityEngine.Random.Range(-sweepRainWidth * 0.25f, sweepRainWidth * 0.25f);
                Vector3 spawnPos = new Vector3(cx + jitter, center.y + sweepRainSpawnH, targetPos.z);
                SpawnProjectile(caster, spawnPos, so, init_Down, tick_Down, center.y, cx + jitter, sweepRainFallMul);

                yield return new WaitForSeconds(interval);
                elapsed += interval;
                tLeft -= interval;
            }
        }

        void Spawn_SineStraight(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 forward)
        {
            Vector3 right = new Vector3(-forward.y, forward.x, 0f).normalized;
            float maxDist = 9999f; // 실제 종료는 목표 거리 기반으로 tick에서 처리
            SpawnProjectile(caster, pos, so, init_SineStraight, tick_SineStraight,
                pos, forward, right, maxDist, sineAmplitude, sineFrequencyHz);
        }

        void Spawn_ZigZagStraight(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 forward)
        {
            Vector3 right = new Vector3(-forward.y, forward.x, 0f).normalized;
            float maxDist = 9999f;
            SpawnProjectile(caster, pos, so, init_ZigZagStraight, tick_ZigZagStraight,
                pos, forward, right, maxDist, zigzagAmplitude, zigzagFrequencyHz);
        }

        void Spawn_ScatterSplit(CharacterTypeEnum caster, SkillData so, Vector3 pos, Vector3 dir)
        {
            var parent = SpawnProjectile(caster, pos, so, init_Straight, tick_Straight, dir);
            GetRunner().StartCoroutine(Co_ScatterSplit(caster, so, parent, dir, scatterDelaySec, scatterCount, scatterFanAngle));
        }

        IEnumerator Co_ScatterSplit(CharacterTypeEnum caster, SkillData so, SkillProjectile parent, Vector3 forward, float delay, int count, float fan)
        {
            yield return new WaitForSeconds(delay);

            if (parent == null) yield break; // 이미 사라짐

            Vector3 pos = parent.transform.position;
            // 분열 생성(매니저만 Instantiate)
            count = Mathf.Max(1, count);
            float total = fan;
            float step = (count <= 1) ? 0f : total / (count - 1);
            float start = -total * 0.5f;
            for (int i = 0; i < count; i++)
            {
                float ang = start + step * i;
                Vector3 dir = Quaternion.AngleAxis(ang, Vector3.forward) * forward;
                Spawn_Straight(caster, so, pos, dir);
            }

            if (parent != null)
                PoolManager.Instance.DestroyPooled(parent.gameObject);

        }

        void init_Straight(SkillProjectile p, object[] args)
        {
            Vector3 dir = (args != null && args.Length > 0 && args[0] is Vector3 v) ? v.normalized
                             : DirFromTo(p.transform.position, p.GetCurrentTargetPos(false));

            float maxDist = Mathf.Max(0.05f, Vector3.Distance(p.transform.position, p.GetCurrentTargetPos(false)));
            p.State["dir"] = dir;
            p.State["travel"] = 0f;
            p.State["maxDist"] = maxDist;
        }

        void tick_Straight(SkillProjectile p, object[] args)
        {
            Vector3 dir = (Vector3)p.State["dir"];
            float travel = (float)p.State["travel"];
            float maxDist = (float)p.State["maxDist"];

            float step = p.speed * Time.deltaTime;
            travel += step;

            Vector3 next = p.transform.position + dir * step;
            p.ApplyMove(next, dir);

            if (travel >= maxDist + 0.01f) { PoolManager.Instance.DestroyPooled(p.gameObject); return; }
            p.State["travel"] = travel;
        }

        void init_Parabola(SkillProjectile p, object[] args)
        {
            float heightBase = (args != null && args.Length > 0) ? Convert.ToSingle(args[0]) : 3.5f;

            Vector3 from = p.transform.position;
            Vector3 target = p.GetCurrentTargetPos(false);

            p.State["from"] = from;
            p.State["targetSnap"] = target;
            p.State["t"] = 0f;
            p.State["initDist"] = Vector3.Distance(from, target);
            p.State["heightBase"] = heightBase;
        }

        void tick_Parabola(SkillProjectile p, object[] args)
        {
            float t = (float)p.State["t"];
            float initDist = (float)p.State["initDist"];
            float heightBase = (float)p.State["heightBase"];
            Vector3 from = (Vector3)p.State["from"];
            Vector3 target = (Vector3)p.State["targetSnap"];

            float baseDist = (initDist > 0.01f) ? initDist : Vector3.Distance(from, target);
            t = Mathf.Clamp01(t + (p.speed / baseDist) * Time.deltaTime);
            p.State["t"] = t;

            Vector3 ctrl = ComputeCtrl(heightBase, from, target);
            Vector3 a = Vector3.Lerp(from, ctrl, t);
            Vector3 b = Vector3.Lerp(ctrl, target, t);
            Vector3 pos = Vector3.Lerp(a, b, t);

            float t2 = Mathf.Clamp01(t + 0.02f);
            Vector3 a2 = Vector3.Lerp(from, ctrl, t2);
            Vector3 b2 = Vector3.Lerp(ctrl, target, t2);
            Vector3 fut = Vector3.Lerp(a2, b2, t2);
            Vector3 dir = (fut - pos); if (dir.sqrMagnitude < 1e-6f) dir = (target - pos);

            p.ApplyMove(pos, dir.normalized);

            if (Mathf.Approximately(t, 1f)) PoolManager.Instance.DestroyPooled(p.gameObject);
        }

        Vector3 ComputeCtrl(float heightBase, Vector3 from, Vector3 to)
        {
            Vector3 mid = (from + to) * 0.5f;
            float dist = Vector3.Distance(from, to);
            float h = heightBase + 0.35f * dist;
            return new Vector3(mid.x, mid.y + h, mid.z);
        }

        void init_Down(SkillProjectile p, object[] args)
        {
            float stopY = Convert.ToSingle(args[0]);
            float xFixed = Convert.ToSingle(args[1]);

            p.State["stopY"] = stopY;
            p.State["x"] = xFixed;
            // 위치는 매니저가 스폰 시 이미 y=spawnH로 맞춰둠
        }

        void tick_Down(SkillProjectile p, object[] args)
        {
            float fallMul = (args != null && args.Length >= 3) ? Convert.ToSingle(args[2]) : 1f;
            float stopY = (float)p.State["stopY"];
            float x = (float)p.State["x"];

            Vector3 cur = p.transform.position;
            float nextY = cur.y - (p.speed * fallMul) * Time.deltaTime;

            Vector3 next = new Vector3(x, nextY, cur.z);
            p.ApplyMove(next, Vector3.down);

            if (p.transform.position.y <= stopY - 0.2f) PoolManager.Instance.DestroyPooled(p.gameObject);
        }

        void init_SineStraight(SkillProjectile p, object[] args)
        {
            p.State["origin"] = (Vector3)args[0];
            p.State["forward"] = (Vector3)args[1];
            p.State["right"] = (Vector3)args[2];
            p.State["dist"] = 0f;
            p.State["maxDist"] = (float)args[3];
            p.State["amp"] = (float)args[4];
            p.State["freqHz"] = (float)args[5];
        }

        void tick_SineStraight(SkillProjectile p, object[] args)
        {
            Vector3 origin = (Vector3)p.State["origin"];
            Vector3 forward = (Vector3)p.State["forward"];
            Vector3 right = (Vector3)p.State["right"];
            float dist = (float)p.State["dist"];
            float maxDist = (float)p.State["maxDist"];
            float amp = (float)p.State["amp"];
            float freqHz = (float)p.State["freqHz"];

            float dt = Time.deltaTime;
            dist += p.speed * dt;

            float phase = p.Elapsed * freqHz * Mathf.PI * 2f;
            Vector3 pos = origin + forward * dist + right * (Mathf.Sin(phase) * amp);

            float d2 = dist + p.speed * 0.02f;
            float ph2 = phase + freqHz * Mathf.PI * 2f * 0.02f;
            Vector3 p2 = origin + forward * d2 + right * (Mathf.Sin(ph2) * amp);
            Vector3 dir = (p2 - pos).normalized;

            p.ApplyMove(pos, dir);

            if (dist >= maxDist + 0.01f) PoolManager.Instance.DestroyPooled(p.gameObject);
            p.State["dist"] = dist;
        }

        void init_ZigZagStraight(SkillProjectile p, object[] args)
        {
            p.State["origin"] = (Vector3)args[0];
            p.State["forward"] = (Vector3)args[1];
            p.State["right"] = (Vector3)args[2];
            p.State["dist"] = 0f;
            p.State["maxDist"] = (float)args[3];
            p.State["amp"] = (float)args[4];
            p.State["freqHz"] = (float)args[5];
        }

        void tick_ZigZagStraight(SkillProjectile p, object[] args)
        {
            Vector3 origin = (Vector3)p.State["origin"];
            Vector3 forward = (Vector3)p.State["forward"];
            Vector3 right = (Vector3)p.State["right"];
            float dist = (float)p.State["dist"];
            float maxDist = (float)p.State["maxDist"];
            float amp = (float)p.State["amp"];
            float freqHz = (float)p.State["freqHz"];

            float dt = Time.deltaTime;
            dist += p.speed * dt;

            float wave = TriWave(p.Elapsed * freqHz);
            Vector3 pos = origin + forward * dist + right * (wave * amp);

            float d2 = dist + p.speed * 0.02f;
            float w2 = TriWave((p.Elapsed + 0.02f) * freqHz);
            Vector3 p2 = origin + forward * d2 + right * (w2 * amp);
            Vector3 dir = (p2 - pos).normalized;

            p.ApplyMove(pos, dir);

            if (dist >= maxDist + 0.01f) PoolManager.Instance.DestroyPooled(p.gameObject);
            p.State["dist"] = dist;
        }

        void init_StraightSpin(SkillProjectile p, object[] args)
        {
            Vector3 dir = (args != null && args.Length > 0 && args[0] is Vector3 v) ? v.normalized
                             : DirFromTo(p.transform.position, p.GetCurrentTargetPos(false));
            float spin = (args != null && args.Length > 1) ? Convert.ToSingle(args[1]) : spinningStraightSpinSpeedDeg;

            float maxDist = Mathf.Max(0.05f, Vector3.Distance(p.transform.position, p.GetCurrentTargetPos(false)));

            p.State["dir"] = dir;
            p.State["travel"] = 0f;
            p.State["maxDist"] = maxDist;
            p.State["spinSpeed"] = spin;   // 도/초
            p.State["spinAccum"] = 0f;     // 누적 각
                                           // baseRot은 첫 틱에서 ApplyMove가 만든 회전을 캡처
        }

        void tick_StraightSpin(SkillProjectile p, object[] args)
        {
            Vector3 dir = (Vector3)p.State["dir"];
            float travel = (float)p.State["travel"];
            float maxDist = (float)p.State["maxDist"];
            float spinSpd = (float)p.State["spinSpeed"];
            float dt = Time.deltaTime;

            float step = p.speed * dt;
            travel += step;
            Vector3 next = p.transform.position + dir * step;

            // 위치/기본 방향 회전
            p.ApplyMove(next, dir);

            // 첫 프레임에 기준 회전 저장
            if (!p.State.ContainsKey("baseRot"))
                p.State["baseRot"] = p.transform.rotation;

            // 누적 회전 적용
            float accum = (float)p.State["spinAccum"];
            accum += spinSpd * dt;
            p.State["spinAccum"] = accum;

            Quaternion baseRot = (Quaternion)p.State["baseRot"];
            p.transform.rotation = baseRot * Quaternion.Euler(0f, 0f, accum);

            if (travel >= maxDist + 0.01f) { PoolManager.Instance.DestroyPooled(p.gameObject); return; }
            p.State["travel"] = travel;
        }
    }

    //Monobehaviour가 필요해서 작성
    internal class SkillCoroutineHub : MonoBehaviour
    {
        private static SkillCoroutineHub _inst;
        public static SkillCoroutineHub Instance
        {
            get
            {
                if (_inst == null)
                {
                    var go = new GameObject("~SkillCoroutineHub");
                    DontDestroyOnLoad(go);
                    _inst = go.AddComponent<SkillCoroutineHub>();
                }
                return _inst;
            }
        }


    }

}
