using UnityEngine;

public class TrapVisual : MonoBehaviour
{
    public string trapId;

    public void Init(string id)
    {
        trapId = id;
        Debug.Log($"TrapVisual Init called with ID: {trapId}");
    }
}
