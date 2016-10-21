using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class NetworkTest : MonoBehaviour {

    public int myReliableChannelId;
    public int socketId;
    public int socketPort = 8888;

    int connectionId;

    public bool connect = false;
    public bool send = false;

    // Use this for initialization
    void Awake() {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        myReliableChannelId = config.AddChannel(QosType.Reliable);
        int maxConnections = 10;
        HostTopology topology = new HostTopology(config, maxConnections);

        socketId = NetworkTransport.AddHost(topology, socketPort);
        Debug.Log("Socket Open. SocketId is: " + socketId);
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveSocketMessage();
    }

    public void ReceiveSocketMessage()
    {
        int recHostId;
        int recConnectedId;
        int recChannelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectedId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("incoming connection event received");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                Debug.Log("incoming message event received: " + message);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("remote client event disconnected");
                break;
        }
    }

    public void SendSocketMessage()
    {
        byte error;
        byte[] buffer = new byte[1024];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, "Hello World!");

        int bufferSize = 1024;

        NetworkTransport.Send(socketId, connectionId, myReliableChannelId, buffer, bufferSize, out error);
    }
}
