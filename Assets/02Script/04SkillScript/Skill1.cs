using UnityEngine;

public class Skill1 : MonoBehaviour
{
    private PlayerManager pm;

    [Header("점프 힘")]
    public float jumpForce = 7.5f;

    [Header("소환할 오브젝트 프리팹")]
    public GameObject summonPrefab;

    [Header("플레이어 위치 기준 소환 오프셋")]
    public Vector2 spawnOffset = Vector2.zero;

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

        // 상태 및 점프
        pm.playerStateController.ForceSetSkill();
        pm.rb.linearVelocity = new Vector2(pm.rb.linearVelocity.x, jumpForce);
        Debug.Log("Skill1 스킬 점프 실행됨!");

        // 오브젝트 소환
        if (summonPrefab != null)
        {
            Vector2 spawnPos = (Vector2)pm.transform.position + spawnOffset;
            GameObject clone = Instantiate(summonPrefab, spawnPos, Quaternion.identity);
            Debug.Log("오브젝트 소환 완료: " + clone.name);
            
            Destroy(clone, 1f);
        }
    }
}
