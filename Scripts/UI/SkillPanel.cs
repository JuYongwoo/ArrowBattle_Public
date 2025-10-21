using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    private enum SkillPanelEnum
    {
        Skill1CoolFadeImg, Skill2CoolFadeImg, Skill3CoolFadeImg, Skill4CoolFadeImg, Skill5CoolFadeImg,
        Skill1CoolTxt, Skill2CoolTxt, Skill3CoolTxt, Skill4CoolTxt, Skill5CoolTxt,
        Skill1IconImg, Skill2IconImg, Skill3IconImg, Skill4IconImg, Skill5IconImg
    }

    private Dictionary<SkillPanelEnum, GameObject> map;

    // ��ų�� ����(1~5 ���)
    private readonly float[] remain = new float[6];
    private readonly Dictionary<int, Coroutine> imgCo = new();
    private readonly Dictionary<int, Coroutine> txtCo = new();
    private readonly Dictionary<int, float> baseHeight = new();   // FadeImg ���� ����

    private void Awake()
    {
        map = Util.MapEnumChildObjects<SkillPanelEnum, GameObject>(gameObject);
        for (int i = 1; i <= 5; i++) //SO ���� ��ų ������ ����
        {

            // SkillPanelEnum �̸� �����
            var enumName = $"Skill{i}IconImg";
            var panelEnum = (SkillPanelEnum)System.Enum.Parse(typeof(SkillPanelEnum), enumName);

            // Skills enum�� i�� �°� ����
            var skillEnum = System.Enum.Parse<Skill>($"Skill{i}");

            // ������ ����
            map[panelEnum].GetComponent<Image>().sprite = ManagerObject.instance.resourceManager.SkillDatas.Result.GetSkillDataById(skillEnum).skillIcon;
        }
    }

    private void Start()
    {
        ManagerObject.instance.eventManager.CooldownUIEvent -= StartCooldown;
        ManagerObject.instance.eventManager.CooldownUIEvent += StartCooldown;

        for (int i = 1; i <= 5; i++)
        {
            StartCooldown(i, 0);
        }
    }

    private void OnDestroy()
    {
        ManagerObject.instance.eventManager.CooldownUIEvent -= StartCooldown;

    }

    public void StartCooldown(int skillNumber, float durationSec)
    {
        if (skillNumber < 1 || skillNumber > 5) return;

        // ���� �ڷ�ƾ ����
        if (imgCo.TryGetValue(skillNumber, out var ic) && ic != null) StopCoroutine(ic);
        if (txtCo.TryGetValue(skillNumber, out var tc) && tc != null) StopCoroutine(tc);

        var rt = GetFadeRect(skillNumber);
        if (rt == null) return;

        EnsureBottomAnchored(rt);

        // ���� ���� ĳ��(������ ���� ���� ���)
        if (!baseHeight.ContainsKey(skillNumber))
            baseHeight[skillNumber] = rt.rect.height > 0 ? rt.rect.height : rt.sizeDelta.y;

        // �ʱ� ����: 100%
        SetHeight(rt, baseHeight[skillNumber]);

        // �ؽ�Ʈ �ʱ�ȭ
        SetText(skillNumber, Mathf.CeilToInt(durationSec).ToString());

        remain[skillNumber] = durationSec;

        // �ڷ�ƾ ����
        imgCo[skillNumber] = StartCoroutine(CoShrinkFade(skillNumber, durationSec));
        txtCo[skillNumber] = StartCoroutine(CoTextTick(skillNumber, Mathf.CeilToInt(durationSec)));
    }

    public void CancelCooldown(int skillNumber)
    {
        if (skillNumber < 1 || skillNumber > 5) return;

        if (imgCo.TryGetValue(skillNumber, out var ic) && ic != null) StopCoroutine(ic);
        if (txtCo.TryGetValue(skillNumber, out var tc) && tc != null) StopCoroutine(tc);
        imgCo[skillNumber] = txtCo[skillNumber] = null;

        var rt = GetFadeRect(skillNumber);
        if (rt != null) SetHeight(rt, 0f);

        SetText(skillNumber, "");
        remain[skillNumber] = 0f;
    }

    private IEnumerator CoShrinkFade(int idx, float duration)
    {
        var rt = GetFadeRect(idx);
        if (rt == null) yield break;

        float H = baseHeight[idx];
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(1f - t / duration); // 1��0
            SetHeight(rt, H * ratio);
            remain[idx] = Mathf.Max(0f, duration - t);
            yield return null;
        }

        SetHeight(rt, 0f);
        remain[idx] = 0f;
        imgCo[idx] = null;
    }

    private IEnumerator CoTextTick(int idx, int secs)
    {
        int t = secs;
        while (t > 0)
        {
            SetText(idx, t.ToString());
            yield return new WaitForSeconds(1f);
            t--;
        }
        SetText(idx, "");
        txtCo[idx] = null;
    }


    private RectTransform GetFadeRect(int idx)
    {
        if (!map.TryGetValue(ToEnum($"Skill{idx}CoolFadeImg"), out var go) || go == null) return null;
        return go.transform as RectTransform;
    }

    private void SetHeight(RectTransform rt, float h)
    {
        var sd = rt.sizeDelta;
        sd.y = h;
        rt.sizeDelta = sd;
        // anchoredPosition.y�� �����Ǹ�, �� ����(pivot=1, anchor=1) ���¿��� �Ʒ��θ� �پ��ϴ�.
    }

    private void EnsureBottomAnchored(RectTransform rt)
    {
        // �Ʒ�(Bottom)�� ���� �� ���̸� ���̸� ������ �������� ���Ʒ��� ���̴¡� ������ ����
        rt.pivot = new Vector2(rt.pivot.x, 0f);
        rt.anchorMin = new Vector2(rt.anchorMin.x, 0f);
        rt.anchorMax = new Vector2(rt.anchorMax.x, 0f);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f);
    }
    private void SetText(int idx, string s)
    {
        if (!map.TryGetValue(ToEnum($"Skill{idx}CoolTxt"), out var go) || go == null) return;

        if (go.TryGetComponent<TextMeshProUGUI>(out var tmp)) tmp.text = s;
        else if (go.TryGetComponent<Text>(out var uiText)) uiText.text = s;
    }

    private static SkillPanelEnum ToEnum(string name)
        => (SkillPanelEnum)System.Enum.Parse(typeof(SkillPanelEnum), name);
}
