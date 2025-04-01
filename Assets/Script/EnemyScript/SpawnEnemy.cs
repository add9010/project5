using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] float spawnLoopTime;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnOffset = 2f;  // 스폰 위치의 오프셋 (양옆으로 얼마나 떨어질지)
    private bool isSpawning = false;  // 스폰 여부를 체크하는 변수

    // 스폰을 시작하는 메서드
    public void StartSpawning()
    {
        if (!isSpawning)  // 이미 스폰이 시작된 경우 다시 시작하지 않도록
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

  
    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            // 왼쪽과 오른쪽으로 적 스폰
            Vector3 leftSpawnPosition = transform.position + Vector3.left * spawnOffset;
            Vector3 rightSpawnPosition = transform.position + Vector3.right * spawnOffset;

            // 적을 왼쪽과 오른쪽에 각각 스폰
            Instantiate(enemyPrefab, leftSpawnPosition, Quaternion.identity);
            Instantiate(enemyPrefab, rightSpawnPosition, Quaternion.identity);

            // 다음 스폰까지 대기
            yield return new WaitForSeconds(spawnLoopTime);
        }
    }
}
