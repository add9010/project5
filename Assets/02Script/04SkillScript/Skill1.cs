using UnityEngine;

public class Skill1 : MonoBehaviour
{
    private PlayerManager pm;

    public float jumpForce = 7.5f;

    public void Initialize(PlayerManager playerManager)
    {
        this.pm = playerManager;
    }

    public void Activate()
    {
        if (pm == null)
        {
            Debug.LogWarning("PlayerManager가 연결되지 않았습니다.");
            return;
        }
        pm.playerStateController.ForceSetSkill();
        pm.rb.linearVelocity = new Vector2(pm.rb.linearVelocity.x, jumpForce);

        Debug.Log("Skill1 스킬 점프 실행됨!");
    }
}
