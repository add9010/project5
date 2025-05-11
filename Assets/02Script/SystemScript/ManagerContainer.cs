// ManagerContainer.cs
using UnityEngine;

public class ManagerContainer : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
