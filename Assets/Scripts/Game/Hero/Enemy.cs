using System;
using GAME;
using TMPro;
using UnityEngine;

public class Enemy : HeroBase
{
    private TextMeshProUGUI textLevel = null;
    private TextMeshProUGUI textHP = null;
    public override void init(DATA.HeroData data, int pos)
    {
        base.init(data, pos);
    }
    public override void attack()
    {
    }
    public override void hit(float beforeHP, float damage, bool isCrit)
    {
        Debug.Log("[Simulation] enemy hit!");

        float afterHP = Math.Max(beforeHP - damage, 0);
        string current = afterHP.ToString("F2"); // 반올림
        string hp = GameMyData.Instance.UserData.enemy.GetHP().ToString("F2"); // 반올림

        textHP.text = $"{current}/{hp}";
    }

    public void setLevel(int level)
    {
        if (textLevel == null)
        {
            GameObject textObj = new GameObject("textLevel");
            textObj.transform.SetParent(transform);
            textLevel = textObj.AddComponent<TextMeshProUGUI>();
            textLevel.fontSize = 24;
            textLevel.color = Color.black;
            textLevel.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Center;

            RectTransform rect = textObj.GetComponent<RectTransform>();

            // 앵커 Y를 1.1로 (X는 중앙 유지)
            rect.anchorMin = new Vector2(0.5f, 1.22f);
            rect.anchorMax = new Vector2(0.5f, 1.22f);

            // 피벗은 중앙 (0.5)
            rect.pivot = new Vector2(0.5f, 0.5f);

            // 위치는 앵커 기준으로 (0, 0)
            rect.anchoredPosition = Vector2.zero;

            // 회전, 스케일 초기화도 필요할 수 있음
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

        }

        textLevel.text = $"Lv {level}";
        setHP();
    }

    public void setHP()
    {
        if (textHP == null)
        {
            GameObject textObj = new GameObject("textHP");
            textObj.transform.SetParent(transform);
            textHP = textObj.AddComponent<TextMeshProUGUI>();

            textHP.fontSize = 16;
            textHP.color = Color.black;
            textHP.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Center;

            RectTransform rect = textObj.GetComponent<RectTransform>();

            // 앵커 Y를 1.1로 (X는 중앙 유지)
            rect.anchorMin = new Vector2(0.5f, 1.1f);
            rect.anchorMax = new Vector2(0.5f, 1.1f);

            // 피벗은 중앙 (0.5)
            rect.pivot = new Vector2(0.5f, 0.5f);

            // 위치는 앵커 기준으로 (0, 0)
            rect.anchoredPosition = Vector2.zero;

            // 회전, 스케일 초기화도 필요할 수 있음
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }
        
        string current = GameMyData.Instance.UserData.enemy.GetHP().ToString("F2"); // 반올림
        string hp = GameMyData.Instance.UserData.enemy.GetHP().ToString("F2"); // 반올림
        textHP.text = $"{current}/{hp}";
    }
}
