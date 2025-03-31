using UnityEngine;

[System.Serializable]
public class PlayerSnapshot
{
    public float x;
    public float y;
    public float z;
    public string animationState;

    public Vector3 GetPosition()
    {
        return new Vector3(x, y, z);
    }
}