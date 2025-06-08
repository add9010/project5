using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Mark : MonoBehaviour
{
    public Transform enemy;
    private float duration = 1f;

    private void Update()
    {
        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = enemy.position + new Vector3(0, 1f, 0);
    }

    private void Start()
    {
        Destroy(gameObject, duration);
    }
}
