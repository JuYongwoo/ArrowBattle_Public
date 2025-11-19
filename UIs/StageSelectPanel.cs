using JYW.ArrowBattle.Managers;
using JYW.ArrowBattle.Utils;
using System.Collections.Generic;
using UnityEngine;
using JYW.ArrowBattle.Commons;

namespace JYW.ArrowBattle.UIs
{
    public class StageSelectPanel : MonoBehaviour
    {
        private enum TitlePanelObj
        {
            StageSelectBGImg,
            StageSelectGrid
        }

        [SerializeField]
        private GameObject stageSelectButton;

        private int stageNumber = 0;

        private Dictionary<TitlePanelObj, GameObject> titlePanelObjs;

        private void Awake()
        {
            titlePanelObjs = Util.MapEnumChildObjects<TitlePanelObj, GameObject>(this.gameObject);
            EventManager.Instance.GetStageNumberEvent -= GetStageNumber;
            EventManager.Instance.GetStageNumberEvent += GetStageNumber;
            EventManager.Instance.SetStageSelectionEvent -= SetStageSelect;
            EventManager.Instance.SetStageSelectionEvent += SetStageSelect;

        }
        private void Start()
        {
            SetStageSelect(5); //임시로 5 스테이지 설정
        }

        private void OnDestroy()
        {
            //EventManager.Instance.GetStageNumberEvent -= GetStageNumber;

        }
        private void SetStageSelect(int stageSize)
        {


            for (int i = 0; i < stageSize; i++)
            {
                int index = i;
                GameObject btn = Instantiate(stageSelectButton);
                
                RectTransform btnRect = btn.GetComponent<RectTransform>();
                btnRect.SetParent(titlePanelObjs[TitlePanelObj.StageSelectGrid].transform, false); // false로 해야 로컬 스케일/앵커 유지

                btn.GetComponentInChildren<UnityEngine.UI.Text>().text = (i + 1).ToString();
                btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    stageNumber = index;
                    EventManager.Instance.OnChangeScene(Commons.Scenes.Main);
                });


            }

        }

        private int GetStageNumber()
        {
            return stageNumber;
        }


        //스테이지 선택 버튼 배치
        //EventHub에서 스테이지값 가져갈 수 있는 GetStageNumberEvent 바인딩
        //SO 구조, 스테이지 별로 되도록 수정
        //게임 시작할때 StageNumber에 따라서 so 내의 인덱스 n번째 값을 사용하여 게임 설정하도록 수정
    }
}