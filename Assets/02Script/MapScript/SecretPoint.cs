using UnityEngine;

public class SecretPoint : MonoBehaviour
{
    public int secretMapIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MapManager.Instance.GoToSecretMap(secretMapIndex);
        }
    }
}
