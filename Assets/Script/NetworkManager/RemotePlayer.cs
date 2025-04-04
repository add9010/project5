using System;
using System.Collections.Generic;
using System.Threading;

public class RemotePlayer
{
    private string name;
    private float posX, posY;

    public RemotePlayer(string name)
    {
        this.name = name;
        posX = 0;
        posY = 0;
    }

    public void UpdatePosition(float x, float y) { posX = x; posY = y; }
    public void SetName(string name) => this.name = name;
    public float GetPosX() => posX;
    public float GetPosY() => posY;
    public string GetName() => name;
}
