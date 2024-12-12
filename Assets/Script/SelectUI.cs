using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    public Button speedButton;      // 이동 속도 버튼
    public Button attackButton;     // 공격력 버튼
    public Button healthButton;     // 최대 체력 버튼
    public Button randomButton;     // 무작위 버튼

    public HeroKnightUsing heroKnight;  // HeroKnightUsing 스크립트 참조
    public Boss boss;                   // Boss 스크립트 참조
    private List<Button> buttons;       // 모든 버튼 리스트

    void Start()
    {
        // 버튼 리스트 초기화
        buttons = new List<Button> { speedButton, attackButton, healthButton, randomButton };

        // 버튼 클릭 이벤트에 메소드 연결
        speedButton.onClick.AddListener(() => OnAnyButtonClicked("speed"));
        attackButton.onClick.AddListener(() => OnAnyButtonClicked("attack"));
        healthButton.onClick.AddListener(() => OnAnyButtonClicked("health"));
        randomButton.onClick.AddListener(() => OnAnyButtonClicked("random"));

        // 보스의 죽음을 감지하는 이벤트 연결
        if (boss != null) boss.OnBossDeath += HandleBossDeath;
        

    }

    private void OnAnyButtonClicked(string attribute)
    {
        // 캐릭터 속성 설정
        heroKnight.SetCharacterAttribute(attribute);

        // 모든 버튼 숨기기
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        // 로그 출력 (디버깅용)
        Debug.Log($"{attribute} 를 선택하였습니다.");
    }

    private void HandleBossDeath()
    {
        // 보스를 죽였을 때 버튼을 다시 활성화
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }

        // 로그 출력 (디버깅용)
        Debug.Log("축하합니다! 보스를 처치하였습니다.");
    }
}
