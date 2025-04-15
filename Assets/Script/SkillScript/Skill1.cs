using UnityEngine;

public class Skill1 : MonoBehaviour
{
    public void Activate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        PlayerManager pm = player.GetComponent<PlayerManager>();
        Animator anim = player.GetComponent<Animator>();
        if (pm == null) return;
        anim.SetTrigger("Skill1");
        //pm.animator.SetTrigger("Skill1");
        pm.playerMove.DoJump(); // <- 통일된 점프 방식 호출

        Debug.Log("Skill1 점프 실행됨");
    }
}
