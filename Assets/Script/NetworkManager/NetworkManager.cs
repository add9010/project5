using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient Instance { get; private set; }

    private RemotePlayerUpdater remoteUpdater; //추가

    public string playerName; // 인스펙터에서 설정 가능
    private Socket socket;
    private Player localPlayer;
    private GameWorld gameWorld;

    private Thread recvThread;
    private bool isConnected = false;

    private float x = 0, y = 0;
    private float sendInterval = 0.03f; // 초
    private float timer = 0f;

    void Start()
    {
        socket = ConnectToServer("127.0.0.1", 5000);
        if (socket == null) return;

        localPlayer = new Player(socket, playerName);
        gameWorld = new GameWorld();
        gameWorld.SetLocalPlayer(localPlayer);


        localPlayer.Init();

        remoteUpdater = FindFirstObjectByType<RemotePlayerUpdater>(); //추가

        recvThread = new Thread(() => ReceiveData(gameWorld));
        recvThread.IsBackground = true;
        recvThread.Start();

        isConnected = true;
    }

    void Update()
    {
        if (!isConnected) return;

        timer += Time.deltaTime;
        if (timer >= sendInterval)
        {
            Vector2 playerPos = PlayerManager.Instance.rb.position;
            x = playerPos.x;
            y = playerPos.y;

            localPlayer.UpdatePosition(x, y);
            AnimType currentAnim = PlayerManager.Instance.GetCurrentAnimState();

            localPlayer.SetAnimType(currentAnim);

            localPlayer.SendPlayerData();

            timer = 0f;
        }
    }

    void OnApplicationQuit()
    {
        if (socket != null)
        {
            socket.Close();
        }
        if (recvThread != null && recvThread.IsAlive)
        {
            recvThread.Abort(); // Unity에서 권장되진 않지만 예제용으로 사용
        }
    }

    Socket ConnectToServer(string ip, int port)
    {
        try
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            Debug.Log($"Connected to server: {ip}:{port}");
            return sock;
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Connection failed: {ex.Message}");
            return null;
        }
    }

    void ReceiveData(GameWorld gameWorld)
    {
        Player localPlayer = gameWorld.GetLocalPlayer();
        Packet recvPacket = new Packet();

        while (true)
        {
            if (!localPlayer.ReceivePlayerData(out recvPacket))
            {
                Debug.LogError("Error receiving data from server");
                break;
            }
            else
            {
                var recvHeader = recvPacket.Header;
                var dataType = recvHeader.Type;

                if (dataType == PacketType.WorldUpdate)
                {
                    gameWorld.SyncWorldData(recvPacket);

                    var snapshots = gameWorld.GetRemoteSnapshots();
                    MainThreadDispatcher.RunOnMainThread(() =>
                    {
                        remoteUpdater.Apply(snapshots);
                    });
                }
                else
                {
                    Debug.LogWarning("Invalid packet type received");
                }
            }
        }
    }


}
