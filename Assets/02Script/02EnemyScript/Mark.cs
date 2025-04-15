using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Mark : MonoBehaviour
{
    public Transform enemy; // 에너미를 추적하기 위한 변수
    public float followSpeed = 3f; // 마커가 에너미를 추적하는 속도
    private float followTime = 1f; // 마커가 존재할 시간 (1초)

    private void Start()
    {


        // 에너미를 추적하기 시작하고, 1초 후에 마커 삭제
        StartCoroutine(DestroyMarkAfterDelay());
    }

    private void Update()
    {
        if (enemy != null)
        {
            // 에너미를 추적하는 로직 (Y축은 고정)
            Vector3 targetPosition = new Vector3(enemy.position.x, transform.position.y, enemy.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DestroyMarkAfterDelay()
    {
        yield return new WaitForSeconds(followTime); // 1초 대기 후
        Destroy(gameObject); // 마커 삭제
    }
}