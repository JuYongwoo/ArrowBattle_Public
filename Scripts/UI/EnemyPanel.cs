using System.Collections.Generic;
using UnityEngine;

public class EnemyPanel : MonoBehaviour
{
    private enum EnemyPanelEnum
    {
        EnemyHPSlider,
        EnemyHPSliderFillAreaTxt
    }
    private Dictionary<EnemyPanelEnum, GameObject> EnemyPanelmap;


    private void Awake()
    {
        EnemyPanelmap = Util.MapEnumChildObjects<EnemyPanelEnum, GameObject>(gameObject);
        ManagerObject.instance.eventManager.SetEnemyHPInUIEvent -= SetHPEnemyUI;
        ManagerObject.instance.eventManager.SetEnemyHPInUIEvent += SetHPEnemyUI;
    }

    private void OnDestroy()
    {
        ManagerObject.instance.eventManager.SetEnemyHPInUIEvent -= SetHPEnemyUI;

    }

    private void SetHPEnemyUI(float hp, float maxHP)
    {
        EnemyPanelmap.TryGetValue(EnemyPanelEnum.EnemyHPSlider, out GameObject hpSliderObj);
        hpSliderObj.GetComponent<UnityEngine.UI.Slider>().value = hp / maxHP;

        EnemyPanelmap.TryGetValue(EnemyPanelEnum.EnemyHPSliderFillAreaTxt, out GameObject hpTxt);
        hpTxt.GetComponent<UnityEngine.UI.Text>().text = $"{(int)hp}";

    }

}
