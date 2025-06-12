using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuPanelDexTabController : MonoBehaviour
{
    [SerializeField] public Toggle[] toggles;
    [SerializeField] public GameObject[] dexTab;
    public GAME.HeroType currentTabIndex { get; private set; } = GAME.HeroType.None;
    private bool isFirstTabLoaded = false;


    void Start()
    {
        // 각 Toggle에 onValueChanged 이벤트 연결
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;  // 클로저 문제 방지
            toggles[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    OnTabSelected(index);
            });
        }

        // 시작 시 첫 탭 활성화
        OnTabSelected(0);
    }
    void OnTabSelected(int index)
    {
        if ((int)currentTabIndex == index)
        {
            // 같은 탭 눌렀으면 아무 동작 안 함
            return;
        }

        if (Enum.IsDefined(typeof(GAME.HeroType), index))
        {
            currentTabIndex = (GAME.HeroType)index;
        }
        else
        {
            Debug.LogWarning("해당 값은 GameState에 없습니다.");
            return;
        }


        // 선택된 탭에 맞게 내용 활성화/비활성화 처리
        for (int i = 0; i < dexTab.Length; i++)
        {
            dexTab[i].SetActive(i == index);
        }


        if (index == 0 && !isFirstTabLoaded)
        {
            isFirstTabLoaded = true;
        }
    }
}
