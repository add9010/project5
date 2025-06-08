using UnityEngine;

public class Trap : MonoBehaviour
{
    public string Id { get; private set; }
    public Vector2 Position { get; private set; }
    public float Hp { get; private set; }
    public GameObject Visual { get; private set; }

    public Trap(string id, float x, float y, float hp)
    {
        Id = id;
        Position = new Vector2(x, y);
        Hp = hp;
    }

    public void UpdatePosition(float x, float y)
    {
        Position = new Vector2(x, y);

        if (Visual == null) return;

        MainThreadDispatcher.RunOnMainThread(() =>
        {
            Visual.transform.position = Position;
        });
    }

    public void UpdateHp(float hp)
    {
        Hp = hp;
        // 여기서 필요하면 HP바 시각화 업데이트도 처리 가능
    }

    public void SpawnVisual(Vector2 position)
    {
        var prefab = Resources.Load<GameObject>("TrapPrefab");
        if (prefab != null)
        {
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                Visual = Object.Instantiate(prefab, position, Quaternion.identity);
                Position = position;

                TrapVisual visualScript = Visual.GetComponent<TrapVisual>();

                visualScript.Init(this.Id);  // 트랩 ID만 넘김

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
            Object.Destroy(Visual);
            Visual = null;
        }
    }
}
