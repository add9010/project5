using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void Awake()
    {
        // Inspector에 할당 안 돼 있으면 자동으로 태그 “Player” 찾아 할당
        if (target == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogError("CameraFollow: Player 태그를 가진 오브젝트가 없습니다.");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );
        }
    }
}
