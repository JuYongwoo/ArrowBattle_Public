using System.Collections.Generic;
using UnityEngine;
using JYW.ArrowBattle.Managers;
using JYW.ArrowBattle.Utils;

namespace JYW.ArrowBattle.UIs
{
    public class PlayerPanel : MonoBehaviour
    {
        private enum PlayerPanelEnum
        {
            PlayerHPSlider,
            PlayerHPSliderFillAreaTxt
        }
        private Dictionary<PlayerPanelEnum, GameObject> PlayerPanelmap;


        private void Awake()
        {
            PlayerPanelmap = Util.MapEnumChildObjects<PlayerPanelEnum, GameObject>(this.gameObject);
            EventManager.Instance.SetPlayerHPInUIEvent -= SetHPPlayerUI;
            EventManager.Instance.SetPlayerHPInUIEvent += SetHPPlayerUI;
        }

        private void OnDestroy()
        {
            EventManager.Instance.SetPlayerHPInUIEvent -= SetHPPlayerUI;

        }

        private void SetHPPlayerUI(float hp, float maxHP)
        {
            PlayerPanelmap.TryGetValue(PlayerPanelEnum.PlayerHPSlider, out GameObject hpSliderObj);
            hpSliderObj.GetComponent<UnityEngine.UI.Slider>().value = hp / maxHP;

            PlayerPanelmap.TryGetValue(PlayerPanelEnum.PlayerHPSliderFillAreaTxt, out GameObject hpTxt);
            hpTxt.GetComponent<UnityEngine.UI.Text>().text = $"{(int)hp}";

        }

    }
}
