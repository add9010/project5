using UnityEngine;

public class Trap
{
    public string Id { get; private set; }
    public Vector2 Position { get; private set; }
    public GameObject Visual { get; private set; }

    public Trap(string id, float x, float y)
    {
        Id = id;
        Position = new Vector2(x, y);
    }

    public void UpdatePosition(float x, float y)
    {
        if (Visual == null) return;

        Vector2 newPos = new Vector2(x, y);
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            Visual.transform.position = newPos;
        });
    }


    // Visual을 생성하는 메서드
    public void SpawnVisual(Vector2 position)
    {
        var prefab = Resources.Load<GameObject>("TrapPrefab"); // "TrapPrefab"은 Resources 폴더 안에 있어야 합니다.
        if (prefab != null)
        {
            // Instantiate를 메인 스레드에서 실행하도록 MainThreadDispatcher를 사용
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                Visual = Object.Instantiate(prefab, position, Quaternion.identity); // Visual 생성
            });
        }
        else
        {
            Debug.LogWarning("Trap prefab not found!");
        }
    }

    public void DestroyVisual()
    {
        if (Visual != null)
        {
            Object.Destroy(Visual); // 트랩을 제거할 때 시각화 객체도 제거
            Visual = null;
        }
    }
}
