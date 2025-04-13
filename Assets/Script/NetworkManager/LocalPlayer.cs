using System;
using System.Net.Sockets;

public class Player
{
    private ClientSession session;
    private string name;
    private float posX, posY;
    private AnimType animType;

    public Player(Socket socket, string playerName)
    {
        session = new ClientSession(socket);
        name = playerName;
        posX = 0f;
        posY = 0f;
        animType = AnimType.Idle;
    }
    public ClientSession GetSession() => session;
    public string GetName() => name;
    public float GetPosX() => posX;
    public float GetPosY() => posY;
    public AnimType GetAnimType() => animType;


    public void SetName(string playerName) => name = playerName;
    public void SetAnimType(AnimType type) => animType = type;
    public void UpdatePosition(float x, float y)
    {
        posX = x;
        posY = y;
    }
    public void Init()
    {
        Packet packet = new Packet
        {
            Header = new PacketHeader
            {
                Type = PacketType.PlayerInit,
            }
        };
        packet.WriteString(name);
        packet.Write(posX);
        packet.Write(posY);
        session.SendData(packet);
    }
    public void SendPlayerData()
    {
        Packet packet = new Packet
        {
            Header = new PacketHeader
            {
                Type = PacketType.PlayerUpdate,
            }
        };
        packet.Write(posX);
        packet.Write(posY);
        packet.Write((byte)animType);
        session.SendData(packet);
    }

    public void SendMonsterDamage(int damage)
    {
        Packet packet = new Packet
        {
            Header = new PacketHeader
            {
                Type = PacketType.MonsterUpdate,
            }
        };
   
        packet.Write(damage);
        session.SendData(packet);
    }

    // 플레이어 데이터 수신
    public bool ReceivePlayerData(out Packet packet)
    {
        return session.ReceiveData(out packet);
    }


}
