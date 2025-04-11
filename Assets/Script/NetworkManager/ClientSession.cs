using System.Net.Sockets;
using System;
using UnityEngine;

public class ClientSession
{
    private Socket clientSocket;
    private byte[] receiveBuffer = new byte[1024];

    public ClientSession(Socket socket)
    {
        clientSocket = socket;
    }
    public void Close()
    {
        clientSocket.Close();
    }

    public bool ReceiveData(out Packet packet)
    {
        packet = null;
        int headerSize = Packet.HEADER_SIZE;
        int totalBytesReceived = 0;
        
        int bytesRead = clientSocket.Receive(receiveBuffer, 0, headerSize, SocketFlags.None);
        if (bytesRead <= 0) { return false; }

        if (bytesRead == headerSize)
        {
            PacketHeader header = Packet.FromBytes(receiveBuffer);
            int totalSize = (int)header.Length;
            totalBytesReceived += headerSize;
           // Debug.Log($"[ClientSession] 헤더 수신 완료, 총 크기: {totalSize} 바이트");
            if (totalSize > headerSize) // 패킷 오류 확인
            {
                int dataSize = totalSize - headerSize;

                while (totalBytesReceived < totalSize)
                {
                    // 추가 데이터를 받음 (남은 데이터 크기만큼 받기)
                    bytesRead = clientSocket.Receive(
                        receiveBuffer, totalBytesReceived, dataSize, SocketFlags.None);

                    if (bytesRead <= 0) { return false; } // 연결이 끊어졌거나 에러 발생

                    totalBytesReceived += bytesRead; // 받은 데이터 크기 갱신
                    dataSize -= bytesRead;           // 남은 데이터 크기 갱신
                }

                // 수신된 전체 데이터 (헤더 + 데이터) 배열 생성
                byte[] fullPacket = new byte[totalSize];
                Array.Copy(receiveBuffer, 0, fullPacket, 0, totalBytesReceived);

                packet = Packet.Deserialize(fullPacket);
                return true;
            }
        }

        return false;
    }


    public bool SendData(Packet packet)
    {
        byte[] buffer = packet.Serialize();
        int bytesSent = clientSocket.Send(buffer);
        return bytesSent == buffer.Length;
    }

    public Socket GetSocket()
    {
        return clientSocket;
    }
}
