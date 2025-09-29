using System.Collections.Generic;
using UnityEngine;

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
        ManagerObject.instance.actionManager.setPlayerHPinUI = setHPInUI;
    }
    private void setHPInUI(float hp, float maxHP)
    {
        PlayerPanelmap.TryGetValue(PlayerPanelEnum.PlayerHPSlider, out GameObject hpSliderObj);
        hpSliderObj.GetComponent<UnityEngine.UI.Slider>().value = hp / maxHP;

        PlayerPanelmap.TryGetValue(PlayerPanelEnum.PlayerHPSliderFillAreaTxt, out GameObject hpTxt);
        hpTxt.GetComponent<UnityEngine.UI.Text>().text = $"{(int)hp}";

    }

}
