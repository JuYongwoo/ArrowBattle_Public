using System;
using System.Collections;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    // Other methods and variables...
    GameObject player;
    Player playerComp;
    GameObject enemy;



    private int gameLeftTime = 99;
    // ����: MonoBehaviour monoBehaviourforInvoke = new MonoBehaviour();  // �� ������
    private TimerRunner _timerRunner; // ������ �ݺ� ȣ���� ���� ����

    // Start is called before the first frame update
    public void Awake()
    {
        //GameMode���� ���ǵ� ĳ���� �����յ��� SO �� ������ ���� ���� ����
        foreach (var character in ManagerObject.instance.resourceManager.gameModeData.Result.Characters)
        {
            GameObject go = MonoBehaviour.Instantiate(character.characterPrefab, character.characterStartPosition, Quaternion.identity);
            if (go.CompareTag("Player")) player = go;
            else if (go.CompareTag("Enemy")) enemy = go;
        }


        //BGM ���
        ManagerObject.instance.audioM.PlayAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.BGM, 0.2f, true);

        gameLeftTime = ManagerObject.instance.resourceManager.gameModeData.Result.GameTime;

        if (_timerRunner == null)
        {
            var go = new GameObject("__GameModeTimer");
            UnityEngine.Object.DontDestroyOnLoad(go);
            _timerRunner = go.AddComponent<TimerRunner>();
        }

        _timerRunner.StartRepeating(flowTime, 1f);
        // _timerRunner.StartRepeatingRealtime(flowTime, 1f); // �� timeScale ���� ����
        playerComp = player.GetComponent<Player>();
        // Other initialization code...


        ManagerObject.instance.actionManager.endGame = endGame; // ActionManager�� endGame �̺�Ʈ�� endGame �޼��� ����

    }

    private void flowTime()
    {
        gameLeftTime--;

        ManagerObject.instance.actionManager.setGameTimeUI?.Invoke(gameLeftTime); //TimePanel�� �ð��� �����ϴ� ��������Ʈ ȣ��
        if (gameLeftTime <= 0)
        {
            endGame(ResultStateEnum.Defeat); //�ð� ����� �й�
        }
    }

    public void endGame(ResultStateEnum resultStateEnum)
    {
        // �ݺ� ����
        if (_timerRunner != null)
            _timerRunner.StopRepeating();

        if (resultStateEnum == ResultStateEnum.Victory) ManagerObject.instance.audioM.PlayAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.VictoryMusic, 0.3f, false);
        else ManagerObject.instance.audioM.PlayAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.DefeatMusic, 0.2f, false);

        Time.timeScale = 0f; //���� �Ͻ�����
        ManagerObject.instance.audioM.StopAudioClip(ManagerObject.instance.resourceManager.gameModeData.Result.BGM); //BGM ����
        ManagerObject.instance.actionManager.gameResultUI?.Invoke(resultStateEnum); //ResultPanel�� UI�� �����ϴ� ��������Ʈ ȣ��
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


