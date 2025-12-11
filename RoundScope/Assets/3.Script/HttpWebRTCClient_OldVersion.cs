using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Unity.WebRTC;

public class HttpWebRTCClient_OldVersion : MonoBehaviour
{
    public RawImage videoImage;

    public string serverBaseUrl = "http://192.168.0.199:9000";

    private RTCPeerConnection pc;

    IEnumerator Start()
    {
        // ------------------------
        // ① 구버전 PeerConnection 생성
        // ------------------------
        RTCConfiguration config = default;
        config.iceServers = new[]
        {
            new RTCIceServer()
            {
                urls = new[] { "stun:stun.l.google.com:19302" }
            }
        };

        pc = new RTCPeerConnection(ref config);

        // ------------------------
        // ② 구버전 트랜시버 추가 (recvonly 불가, 옵션 없음)
        // ------------------------
        pc.AddTransceiver(TrackKind.Video); // ← 너 버전에서 되는 방식

        // ------------------------
        // ③ OnTrack 등록
        // ------------------------
        pc.OnIceConnectionChange = state =>
        {
            Debug.Log("[ICE] State = " + state);
        };

        pc.OnIceGatheringStateChange = state =>
        {
            Debug.Log("[ICE] Gathering = " + state);
        };

        pc.OnIceConnectionChange = s => Debug.Log("ICE: " + s);
        pc.OnTrack = e =>
        {
            Debug.Log("[OnTrack] Track Kind = " + e.Track.Kind);

            if (e.Track is VideoStreamTrack videoTrack)
            {
                Debug.Log("[OnTrack] VideoStreamTrack detected!");
                

                videoTrack.OnVideoReceived += tex =>
                {
                    Debug.Log($"[Video] Frame Received: {tex.width}x{tex.height}");

                    // RawImage에 텍스처 적용
                    videoImage.texture = tex;

                    // AspectRatio나 CanvasScaler 문제 방지
                    videoImage.SetNativeSize();
                };
            }
        };

        // ------------------------
        // ④ Offer 생성
        // ------------------------
        var offerOp = pc.CreateOffer();
        yield return offerOp;

        var offerDesc = offerOp.Desc;

        var localOp = pc.SetLocalDescription(ref offerDesc);
        yield return localOp;

        // ------------------------
        // ⑤ Offer → 서버 POST
        // ------------------------
        string url = serverBaseUrl.TrimEnd('/') + "/webrtc/offer";

        yield return StartCoroutine(SendOffer(url, offerDesc));

        // ------------------------
        // ⑥ 구버전에서는 WebRTC.Update() 불필요
        //    (패키지에서 자동 처리됨)
        // ------------------------
    }

    IEnumerator SendOffer(string url, RTCSessionDescription offer)
    {
        string escaped = EscapeJson(offer.sdp);
        string json = $"{{\"sdp\":\"{escaped}\",\"type\":\"offer\"}}";

        using var client = new HttpClient();
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Debug.Log("[WebRTC] POST Offer → " + url);

        var postTask = client.PostAsync(url, content);
        while (!postTask.IsCompleted)
            yield return null;

        var res = postTask.Result;
        var readTask = res.Content.ReadAsStringAsync();

        while (!readTask.IsCompleted)
            yield return null;

        Debug.Log("[WebRTC] Answer: " + readTask.Result);

        RTCSessionDescription answer =
            JsonUtility.FromJson<RTCSessionDescription>(readTask.Result);

        var op = pc.SetRemoteDescription(ref answer);
        yield return op;

        if (op.IsError)
            Debug.LogError("SetRemoteDescription ERROR: " + op.Error.message);
    }

    string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
    }

    private void OnDestroy()
    {
        pc?.Close(); // 구버전은 이것만 필요함
    }
}
