using System.Collections.Generic;
using UnityEngine;
using JYW.ArrowBattle.Managers;
using JYW.ArrowBattle.Utils;

namespace JYW.ArrowBattle.UIs
{

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
            EventManager.Instance.SetEnemyHPInUIEvent -= SetHPEnemyUI;
            EventManager.Instance.SetEnemyHPInUIEvent += SetHPEnemyUI;
        }

        private void OnDestroy()
        {
            EventManager.Instance.SetEnemyHPInUIEvent -= SetHPEnemyUI;

        }

        private void SetHPEnemyUI(float hp, float maxHP)
        {
            EnemyPanelmap.TryGetValue(EnemyPanelEnum.EnemyHPSlider, out GameObject hpSliderObj);
            hpSliderObj.GetComponent<UnityEngine.UI.Slider>().value = hp / maxHP;

            EnemyPanelmap.TryGetValue(EnemyPanelEnum.EnemyHPSliderFillAreaTxt, out GameObject hpTxt);
            hpTxt.GetComponent<UnityEngine.UI.Text>().text = $"{(int)hp}";

        }

    }
}