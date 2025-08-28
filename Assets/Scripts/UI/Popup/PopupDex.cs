
using System.Collections.Generic;
using System.Linq;
using DATA;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDex : PopupBase, IPopupDex
{
    [SerializeField] public Button resetButton;
    [SerializeField] public TextMeshProUGUI uiOption1_a;
    [SerializeField] public TextMeshProUGUI uiOption1_b;
    [SerializeField] public TextMeshProUGUI uiOption2;
    [SerializeField] public TextMeshProUGUI uiOption3;
    private HeroData heroData = null;
    public Toggle selectPos1;
    public Toggle selectPos2;
    public Toggle selectPos3;

    bool _isReverting = false; // 이벤트 되돌릴 때 재진입 방지
    int _prevToggleIndex = 0;
    void Awake()
    {
    }

    public override void Start()
    {
        base.Start();

        selectPos1.onValueChanged.AddListener((isOn) => { if (isOn) OnToggleChanged(1); });
        selectPos2.onValueChanged.AddListener((isOn) => { if (isOn) OnToggleChanged(2); });
        selectPos3.onValueChanged.AddListener((isOn) => { if (isOn) OnToggleChanged(3); });
        
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetOptions);
    }

    public override void init()
    {
        base.init();
    }

    public override void Refresh()
    {
        base.Refresh();
        ShowPopup();
    }

    public override PopupBase ShowPopup()
    {
        gameObject.SetActive(true);
        uiTitle.text = heroData.Name;
        uiDescription.text = heroData.Description;

        var options = GameMyData.Instance.UserData.heroOptions.FirstOrDefault(h => h.Id == int.Parse(heroData.Id));
        if (options != null)
        {
            foreach (var outer in options.options) // 바깥 Dictionary<int, Dictionary<int, List<string>>>
            {
                int type = outer.Key;
                string strOptions = "";
                foreach (var inner in outer.Value) // 안쪽 Dictionary<int, List<string>>
                {
                    //var option = DataManager.Instance.heroOptionData[type].FirstOrDefault(opt => opt.Id == inner.Key.ToString());
                    var option = DataManager.Instance.heroOptionData
                        .SelectMany(kv => kv.Value)               // 모든 List<HeroOptionData> 펼치기
                        .FirstOrDefault(o => o.Id == inner.Key.ToString());

                    if (option != null)
                    {
                        float value1 = float.Parse(inner.Value[0]);
                        float value2 = float.Parse(inner.Value[1]);
                        float value3 = float.Parse(inner.Value[2]);
                        strOptions = $"{option.Description}({value1}~{value2}){value3}";
                    }
                    else
                    {
                        strOptions = "";
                    }
                }

                if (type == (int)GAME.HeroSkilSlot.Slot1_a)
                {
                    uiOption1_a.text = strOptions;
                }
                else if (type == (int)GAME.HeroSkilSlot.Slot1_b)
                {

                    uiOption1_b.text = strOptions;
                }
                else if (type == (int)GAME.HeroSkilSlot.Slot2)
                {
                    uiOption2.text = strOptions;
                }
                else if (type == (int)GAME.HeroSkilSlot.Slot31 || type == (int)GAME.HeroSkilSlot.Slot32)
                {
                    uiOption3.text = strOptions;
                }
            }
        }

        return this;
    }

    void OnToggleChanged(int index)
    {
        Debug.Log($"선택된 포지션: {index}");

        var equippedPos = GameMyData.Instance.getEquippedDexIndex(int.Parse(heroData.Id)); // "-1" or "1"/"2"/"3"


        // 같은 위치를 다시 누른 경우: 그대로 두면 됨
        if (equippedPos == index.ToString())
        {
            Debug.Log("이미 장착되어 있는 위치입니다.");

            _isReverting = true;               // 재진입 방지
            SetToggleTo(_prevToggleIndex); // 이벤트 없이 토글 상태만 변경
            _isReverting = false;
            return;
        }
    }
    void SetToggleTo(int pos)
    {
        if (pos == -1)
            return;
        // 이벤트 없이 상태만 바꾸기
        selectPos1.SetIsOnWithoutNotify(pos == 1);
        selectPos2.SetIsOnWithoutNotify(pos == 2);
        selectPos3.SetIsOnWithoutNotify(pos == 3);
    }

    public int GetSelectedIndex()
    {
        if (selectPos1.isOn) return 1;
        if (selectPos2.isOn) return 2;
        if (selectPos3.isOn) return 3;
        return 0; // 아무것도 선택되지 않음 (AllowSwitchOff=true인 경우 가능)
    }

    public override void Equip()
    {
        if (heroData == null)
            return;

        Debug.Log("Equip Clicked");
        var prevPos = GameMyData.Instance.getEquippedDexIndex(int.Parse(heroData.Id));
        var pos = GetSelectedIndex();
        if (pos == 0)
        {
            return;
        }

        var payloadObj = new
        {
            heroId = heroData.Id,
            prevposition = prevPos,
            position = pos
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);
        NetworkManager.SendRequest_Test("EquipHero", payloadJson);
    }
    public override void Unequip()
    {
        Debug.Log("Unequip Clicked");

        var position = GameMyData.Instance.getEquippedDexIndex(int.Parse(heroData.Id));
        var payloadObj = new
        {
            heroId = heroData.Id,
            position = position,
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);
        NetworkManager.SendRequest_Test("UnEquipHero", payloadJson);
    }

    public void SetItem(HeroData heroData)
    {
        this.heroData = heroData;
        var position = GameMyData.Instance.getEquippedDexIndex(int.Parse(heroData.Id));
        var equippedHeroIds = GameMyData.Instance.UserData.equippedHeroIds;
        string targetSlot = FindPreferredSlot(equippedHeroIds, position);

        if (targetSlot != null)
        {
            _prevToggleIndex = int.Parse(targetSlot);
            switch (targetSlot)
            {
                case "1": selectPos1.isOn = true; break;
                case "2": selectPos2.isOn = true; break;
                case "3": selectPos3.isOn = true; break;
                default: break;
            }
        }
        else
        {
            Debug.Log("PopupDex SetItem : 장착할 위치가 없음 오류");
        }
    }
    string FindPreferredSlot(Dictionary<string, int> equippedHeroIds, string position)
    {
        string[] order = equippedHeroIds.Keys
        .Select(k => int.Parse(k))
        .OrderBy(k => k)
        .Select(k => k.ToString())
        .ToArray();

        // 1) 빈 슬롯(-1) 우선 탐색 (현재 위치 제외)
        for (int i = 0; i < order.Length; i++)
        {
            string slot = order[i];

            if (position != "-1" && slot == position)
                continue;

            int val = equippedHeroIds.TryGetValue(slot, out var v) ? v : -1;
            if (val == -1)
                return slot;
        }

        // 2) 빈 슬롯이 없다면: 같은 순서로 (현재 위치 제외) 첫 슬롯 선택
        for (int i = 0; i < order.Length; i++)
        {
            string slot = order[i];

            if (position != "-1" && slot == position)
                continue;

            return slot; // 앞번호 반환
        }

        // 3) 모든 슬롯이 현재 위치뿐인 기묘한 경우(슬롯이 1개뿐이라면 등)
        return null;
    }

    public void ResetOptions()
    {        
        Debug.Log("Reset Clicked");

        var payloadObj = new
        {
            heroId = heroData.Id,
            diamond = GameMyData.Instance.UserData.diamond,
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);
        NetworkManager.SendRequest_Test("GetHeroOptions", payloadJson);
    }
}
