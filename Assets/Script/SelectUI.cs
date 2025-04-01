using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SelectUI : MonoBehaviour
{
    public Button speedButton;      // 이동 속도 버튼
    public Button attackButton;     // 공격력 버튼
    public Button healthButton;     // 최대 체력 버튼
    public Button randomButton;     // 무작위 버튼

    public HeroKnightUsing heroKnight;  // HeroKnightUsing 스크립트 참조

    public RedSlimeKing F3Boss;       // F3 보스 스크립트 참조
    public BFGolem F2Boss;            // F2 보스 스크립트 참조

    private List<Button> buttons;  // 모든 버튼 리스트

    void Start()
    {
        // 버튼 리스트 초기화
        buttons = new List<Button> { speedButton, attackButton, healthButton, randomButton };

        // 버튼 클릭 이벤트에 메소드 연결
        speedButton.onClick.AddListener(() => OnAnyButtonClicked("speed"));
        attackButton.onClick.AddListener(() => OnAnyButtonClicked("attack"));
        healthButton.onClick.AddListener(() => OnAnyButtonClicked("health"));
        randomButton.onClick.AddListener(() => OnAnyButtonClicked("random"));

        // 두 보스의 죽음을 감지하는 이벤트 연결
        if (F3Boss != null)
        {
            F3Boss.OnBossDeath += HandleBossDeath;
            Debug.Log("F3 보스 이벤트 연결됨");
        }

        if (F2Boss != null)
        {
            F2Boss.OnBossDeath += HandleBossDeath;
            Debug.Log("F2 보스 이벤트 연결됨");
        }
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
        Debug.Log($"{attribute} 버튼이 클릭되어 모든 버튼이 비활성화되었습니다.");
    }

    // 보스가 죽었을 때 호출되는 메소드
    private void HandleBossDeath()
    {
        // 보스를 죽였을 때 버튼을 다시 활성화
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }

        // 로그 출력 (디버깅용)
        Debug.Log("보스가 죽었습니다. 버튼들이 다시 활성화되었습니다.");
    }

    // OnDestroy 또는 다른 종료 로직에서 이벤트 해제 (선택 사항)
    void OnDestroy()
    {
        if (F3Boss != null) F3Boss.OnBossDeath -= HandleBossDeath;

        if (F2Boss != null) F2Boss.OnBossDeath -= HandleBossDeath;

    }
}
