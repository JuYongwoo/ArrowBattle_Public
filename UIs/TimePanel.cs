using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JYW.ArrowBattle.Managers;
using JYW.ArrowBattle.Utils;

namespace JYW.ArrowBattle.UIs
{
    public class TimePanel : MonoBehaviour
    {
        private enum TimePanelEnum
        {
            TimeTxt
        }
        private Dictionary<TimePanelEnum, GameObject> TimePanelmap;

        private void Awake()
        {
            TimePanelmap = Util.MapEnumChildObjects<TimePanelEnum, GameObject>(gameObject);
            EventManager.Instance.SetGameTimeUIEvent -= SetTime;
            EventManager.Instance.SetGameTimeUIEvent += SetTime;
        }

        private void OnDestroy()
        {
            EventManager.Instance.SetGameTimeUIEvent -= SetTime;

        }

        private void SetTime(int time)
        {
            string timeString = $"{time}";
            TimePanelmap[TimePanelEnum.TimeTxt].GetComponent<Text>().text = timeString;
        }
    }
}
