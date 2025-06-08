using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.transform.position = transform.position;
        }
    }
}
