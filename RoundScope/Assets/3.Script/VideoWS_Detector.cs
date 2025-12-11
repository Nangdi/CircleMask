using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using System;
using System.Collections.Generic;

public class VideoWS_Detector : MonoBehaviour
{
    public RawImage previewImage;

    private WebSocket ws;
    private Texture2D tex;

    // 메인 스레드에서 처리할 작업 큐
    private Queue<Action> mainThreadQueue = new Queue<Action>();

    void Start()
    {
        tex = new Texture2D(2, 2);

        ws = new WebSocket("ws://192.168.0.199:9000");

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("=== WebSocket Message Received ===");
            Debug.Log($"IsBinary: {e.IsBinary}");
            Debug.Log($"Length: {e.RawData.Length}");
            Debug.Log($"First bytes: {BitConverter.ToString(e.RawData, 0, Mathf.Min(10, e.RawData.Length))}");

            if (!e.IsBinary)
            {
                Debug.Log("Message is TEXT");

                if (IsBase64(e.Data))
                {
                    Debug.Log("→ Looks like BASE64 STRING. Attempting decode...");
                    byte[] imgBytes = Convert.FromBase64String(e.Data);
                    EnqueueMainThread(() => TryDecodeImage(imgBytes));
                }
                else
                {
                    Debug.Log("→ Not Base64");
                }
            }
            else
            {
                Debug.Log("Message is BINARY");
                EnqueueMainThread(() => TryDecodeImage(e.RawData));
            }
        };

        ws.Connect();
    }

    void Update()
    {
        // 메인 스레드 큐 처리
        while (mainThreadQueue.Count > 0)
        {
            var action = mainThreadQueue.Dequeue();
            action?.Invoke();
        }
    }

    // 메인 스레드 큐 등록
    void EnqueueMainThread(Action action)
    {
        lock (mainThreadQueue)
        {
            mainThreadQueue.Enqueue(action);
        }
    }

    bool IsBase64(string text)
    {
        try
        {
            Convert.FromBase64String(text);
            return true;
        }
        catch
        {
            return false;
        }
    }

    void TryDecodeImage(byte[] data)
    {
        bool result = tex.LoadImage(data);

        if (result)
        {
            Debug.Log("✔ SUCCESS: This data is a JPEG/PNG IMAGE!");
            previewImage.texture = tex;
        }
        else
        {
            Debug.Log("✘ FAILED: Not an image (maybe H.264 / raw video stream)");
        }
    }

    void OnDestroy()
    {
        ws?.Close();
    }
}
