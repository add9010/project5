using UnityEngine;

public class Skill1 : MonoBehaviour
{
    // 임시로 Animator를 직접 찾음 (나중에 외부에서 주입 예정)
    public void Activate()
    {
        Animator anim = GameObject.FindWithTag("Player")?.GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogWarning("플레이어 애니메이터를 찾을 수 없습니다.");
            return;
        }

        anim.SetTrigger("Skill1");
        Debug.Log("Skill1 실행됨!");

        // TODO: 나중에 PlayerManager에서 직접 애니메이터 주입받도록 수정
        // ex) SkillManager.Instance.RegisterPlayerAnimator(anim);
        //      logic.SetPlayerAnimator(SkillManager.Instance.GetPlayerAnimator());
    }
}
