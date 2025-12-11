using UnityEngine;
using WebSocketSharp;

public class WebSocket_ConnectionTest : MonoBehaviour
{
    WebSocket ws;

    void Start()
    {
        Debug.Log("Connecting to WS...");

        ws = new WebSocket("ws://192.168.0.199:9000");

        ws.OnOpen += (s, e) =>
        {
            Debug.Log("🔥 WS Connected!");
        };

        ws.OnError += (s, e) =>
        {
            Debug.LogError("❌ WS Error: " + e.Message);
        };

        ws.OnClose += (s, e) =>
        {
            Debug.Log("⚠ WS Closed");
        };

        ws.OnMessage += (s, e) =>
        {
            Debug.Log("📩 Message: " + e.Data);
        };

        ws.ConnectAsync();
    }
}
