using UnityEngine;

public class MapEnemyController : MonoBehaviour
{
    public GameObject endPoint;  // 이 맵에서 열릴 EndPoint
    private int enemyCount = 0;

    private void Start()
    {
        Enemy[] enemies = GetComponentsInChildren<Enemy>();
        enemyCount = enemies.Length;

        if (endPoint != null)
            endPoint.SetActive(false);
    }

    public void OnEnemyDied()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            Debug.Log($"{gameObject.name}: 모든 적 처치 완료, 출구 열림!");
            if (endPoint != null)
                endPoint.SetActive(true);
        }
    }
}
