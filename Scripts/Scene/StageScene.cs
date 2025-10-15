using System;
using System.Collections;
using UnityEngine;

public class StageScene : MonoBehaviour
{
    GameObject player; //���� ���� �÷��̾�
    GameObject enemy; //���� ���� ��



    private int gameLeftTime = 99;
    private TimerRunner _timerRunner;

    public void Awake()
    {
        
        loadstage(1); //������ GetStageDatabyStageID(1) �� �������� 1�� ����, Ŭ���� �� GetStageDatabyStageID(2)�� �������� ���� �ҷ������� ��

        if (_timerRunner == null)
        {
            var go = new GameObject("__GameModeTimer");
            DontDestroyOnLoad(go);
            _timerRunner = go.AddComponent<TimerRunner>();
        }

        _timerRunner.StartRepeating(flowTime, 1f);
        // _timerRunner.StartRepeatingRealtime(flowTime, 1f); //timeScale ����


        ManagerObject.instance.actionManager.EndGameEvent -= endGame; // ActionManager�� endGame �̺�Ʈ�� endGame �޼��� ����
        ManagerObject.instance.actionManager.EndGameEvent += endGame;

    }

    private void OnDestroy()
    {
        ManagerObject.instance.actionManager.EndGameEvent -= endGame;

    }

    private void flowTime()
    {
        gameLeftTime--;

        ManagerObject.instance.actionManager.OnSetGameTimeUI(gameLeftTime); //TimePanel�� �ð��� �����ϴ� ��������Ʈ ȣ��
        if (gameLeftTime <= 0)
        {
            endGame(ResultStateEnum.Defeat); //�ð� ����� �й�
        }
    }

    public void loadstage(int stageID)
    {
        //�������� �� SO���� ���ǵ� ĳ����ID�� ���� ĳ���� ID �� SO �� ������ ���� ���� ����
        foreach (var spawnCharacters in ManagerObject.instance.resourceManager.gameModeData.Result.GetStageDatabyStageID(stageID).characterTypeEnum)
        {
            CharacterStatData stat = ManagerObject.instance.resourceManager.characterDatas.Result.GetCharacterDataById(spawnCharacters);
            GameObject go = MonoBehaviour.Instantiate(stat.characterPrefab, stat.startPosition, Quaternion.identity);
            if (go.CompareTag("Player")) player = go;
            else if (go.CompareTag("Enemy")) enemy = go;
        }

        //BGM ���
        ManagerObject.instance.audioM.PlayAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.GetStageDatabyStageID(stageID).bgm, 0.2f, true);

        gameLeftTime = ManagerObject.instance.resourceManager.gameModeData.Result.GetStageDatabyStageID(stageID).gameTime;
    }

    public void endGame(ResultStateEnum resultStateEnum)
    {
        // �ݺ� ����
        if (_timerRunner != null)
            _timerRunner.StopRepeating();

        if (resultStateEnum == ResultStateEnum.Victory) ManagerObject.instance.audioM.PlayAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.victoryMusic, 0.3f, false);
        else ManagerObject.instance.audioM.PlayAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.defeatMusic, 0.2f, false);

        Time.timeScale = 0f; //���� �Ͻ�����
        ManagerObject.instance.audioM.StopAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.GetStageDatabyStageID(1).bgm); //BGM ����
        ManagerObject.instance.actionManager.OnGameResultUI(resultStateEnum); //ResultPanel�� UI�� �����ϴ� ��������Ʈ ȣ��
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
            var wait = new WaitForSeconds(interval); // timeScale ���� ����
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
                yield return new WaitForSecondsRealtime(interval); // timeScale ����
            }
        }
    }
}


