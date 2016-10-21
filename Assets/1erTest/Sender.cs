using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Sender : MonoBehaviour {

    int connectionId;
    public NetworkTest nt;

	// Use this for initialization
	void Start ()
    {
        NetworkTransport.Init();

        Connect();
	}
	
	// Update is called once per frame
	void Update () {
        SendSocketTransform();
	}

    public void Connect()
    {
        byte error;
        connectionId = NetworkTransport.Connect(nt.socketId, "127.0.0.1", nt.socketPort, 0, out error);
        Debug.Log("SENDER : Connected to server. ConnectionId: " + connectionId);
    }

    public void SendSocketTransform()
    {
        byte error;
        byte[] buffer = new byte[1024];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, this.transform.position.ToString() + this.transform.rotation.ToString());

        int bufferSize = 1024;

        NetworkTransport.Send(nt.socketId, connectionId, nt.myReliableChannelId, buffer, bufferSize, out error);
    }
}
