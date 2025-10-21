using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    private enum ResultPanelEnum
    {
        ResultPopupBGImg,
        ResultPopupTxt
    }

    private Dictionary<ResultPanelEnum, GameObject> ResultPanelmap;
    private Coroutine _bgFadeRoutine;

    private void Awake()
    {
        ResultPanelmap = Util.MapEnumChildObjects<ResultPanelEnum, GameObject>(this.gameObject);
        ManagerObject.instance.eventManager.GameResultUIEvent -= StartUI;
        ManagerObject.instance.eventManager.GameResultUIEvent += StartUI;
    }

    private void OnDestroy()
    {
        ManagerObject.instance.eventManager.GameResultUIEvent -= StartUI;

    }

    private void StartUI(ResultStateEnum resultStateEnum)
    {

        //TODO ResultPopupBGImg ���̵� ����
        var bgImg = ResultPanelmap[ResultPanelEnum.ResultPopupBGImg].GetComponent<Image>();

        bgImg.sprite = ManagerObject.instance.resourceManager.ResultImgmap[resultStateEnum].Result;
        bgImg.enabled = true;
        var c = bgImg.color;
        bgImg.color = new Color(c.r, c.g, c.b, 0f); // ���� 0���� ����
        if (_bgFadeRoutine != null) StopCoroutine(_bgFadeRoutine);
        _bgFadeRoutine = StartCoroutine(FadeImageAlpha(bgImg, 0f, 1f, 0.3f)); // 0.3�� ���̵�
        ResultPanelmap[ResultPanelEnum.ResultPopupTxt].GetComponent<Text>().enabled = true;
        //TODO ResultPopupBGImg ���̵� ����

        if (resultStateEnum == ResultStateEnum.Victory)
        {
            ResultPanelmap[ResultPanelEnum.ResultPopupTxt].GetComponent<Text>().text = "�¸�";
        }
        else
        {
            ResultPanelmap[ResultPanelEnum.ResultPopupTxt].GetComponent<Text>().text = "�й�";
        }

        //

    }

    private System.Collections.IEnumerator FadeImageAlpha(Image img, float from, float to, float duration)
    {
        if (img == null) yield break;
        float t = 0f;
        // r,g,b�� �����ϰ� ���ĸ� ����
        Color baseColor = img.color;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // ������ �Ͻ��������� ���̵��� unscaled ���
            float a = Mathf.Lerp(from, to, t / duration);
            img.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            yield return null;
        }
        img.color = new Color(baseColor.r, baseColor.g, baseColor.b, to);
        _bgFadeRoutine = null;
    }



}
