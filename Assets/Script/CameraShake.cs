using UnityEngine;
using System.Collections;  // 코루틴을 사용하기 위해 추가

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;  // 카메라 원래 위치
    public float shakeDuration = 0.1f; // 짧은 흔들림 지속 시간 (베기 느낌)
    public float shakeAmount = 0.2f;   // 강렬한 흔들림 강도
    public float shakeReturnSpeed = 5f; // 원래 위치로 돌아가는 속도

    public Transform player;           // 플레이어의 Transform (플레이어를 추적)

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    public void ShakeCamera()
    {
        // 흔들림을 시작하기 전에 현재 카메라 위치를 저장
        originalPosition = transform.position;

        // 공격할 때마다 카메라 흔들림 발생
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // 카메라의 원래 위치에서 랜덤한 위치로 이동시켜 강렬한 흔들림 효과 생성
            float shakeX = Random.Range(-shakeAmount, shakeAmount);
            // Y축은 원래 위치 유지
            float shakeY = Random.Range(-shakeAmount, shakeAmount);

            // 카메라는 플레이어를 따라가며 흔들림 적용
            transform.position = new Vector3(player.position.x + shakeX, originalPosition.y + shakeY, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 흔들림이 끝난 후 카메라는 바로 플레이어를 추적하도록 유지
        transform.position = new Vector3(player.position.x, originalPosition.y, transform.position.z);
    }
}
