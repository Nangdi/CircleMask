using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.WebRTC;
using WebSocketSharp;

[System.Serializable]
public class WebRTCSignalMessage
{
    public string type;              // "offer" | "answer" | "ice"
    public string sdp;               // SDP 문자열
    public string candidate;         // ICE
    public string sdpMid;
    public int sdpMLineIndex;
}

public class WebRTCReceiver : MonoBehaviour
{
    [Header("UI")]
    public RawImage receiveImage;

    [Header("Signaling Server")]
    public string signalingUrl = "ws://192.168.0.10:8080"; // LAN 시그널링 서버 주소

    private WebSocket ws;
    private RTCPeerConnection pc;
    private Coroutine webrtcLoop;

    void Start()
    {
        // WebRTC 엔진 업데이트 시작
        webrtcLoop = StartCoroutine(WebRTC.Update());

        // PeerConnection 생성
        RTCConfiguration config = default;
        config.iceServers = new[] {
            new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } }
        };
        pc = new RTCPeerConnection(ref config);

        Debug.Log("[WebRTC] PeerConnection Created");

        // ============================== VIDEO TRACK 수신 ==============================
        pc.OnTrack = e =>
        {
            Debug.Log("[WebRTC] OnTrack Received: " + e.Track.Kind);

            if (e.Track is VideoStreamTrack videoTrack)
            {
                Debug.Log("[WebRTC] Video Track Connected!");

                videoTrack.OnVideoReceived += tex =>
                {
                    receiveImage.texture = tex;
                };
            }
        };

        // ============================== ICE 생성 시 서버로 전송 ==============================
        pc.OnIceCandidate = candidate =>
        {
            if (candidate == null) return;

            WebRTCSignalMessage msg = new WebRTCSignalMessage
            {
                type = "ice",
                candidate = candidate.Candidate,
                sdpMid = candidate.SdpMid,
                sdpMLineIndex = candidate.SdpMLineIndex ?? 0
            };

            ws?.Send(JsonUtility.ToJson(msg));
        };

        ConnectWebSocket();
    }

    // ============================== WebSocket 연결 ==============================
    void ConnectWebSocket()
    {
        ws = new WebSocket(signalingUrl);

        ws.OnOpen += (s, e) =>
        {
            Debug.Log("[WS] Connected to signaling server: " + signalingUrl);
        };

        ws.OnMessage += (s, e) =>
        {
            Debug.Log("[WS] Message Received: " + e.Data);
            HandleSignal(e.Data);
        };

        ws.OnError += (s, e) =>
        {
            Debug.LogError("[WS] Error: " + e.Message);
        };

        ws.OnClose += (s, e) =>
        {
            Debug.Log("[WS] Closed");
        };

        ws.ConnectAsync();
    }

    // ============================== 메시지 처리 ==============================
    void HandleSignal(string json)
    {
        WebRTCSignalMessage msg = JsonUtility.FromJson<WebRTCSignalMessage>(json);

        // Offer 수신
        if (msg.type == "offer")
        {
            RTCSessionDescription offer = new RTCSessionDescription
            {
                type = RTCSdpType.Offer,
                sdp = msg.sdp
            };
            StartCoroutine(HandleOffer(offer));
        }

        // ICE 수신
        if (msg.type == "ice")
        {
            var candInit = new RTCIceCandidateInit
            {
                candidate = msg.candidate,
                sdpMid = msg.sdpMid,
                sdpMLineIndex = msg.sdpMLineIndex
            };

            RTCIceCandidate cand = new RTCIceCandidate(candInit);
            pc.AddIceCandidate(cand);
        }
    }

    // ============================== Offer 처리 & Answer 생성 ==============================
    IEnumerator HandleOffer(RTCSessionDescription offer)
    {
        Debug.Log("[WebRTC] SetRemoteDescription(Offer)");

        var op1 = pc.SetRemoteDescription(ref offer);
        yield return op1;
        if (op1.IsError)
        {
            Debug.LogError("[WebRTC] Offer Error: " + op1.Error.message);
            yield break;
        }

        // Answer 생성
        var answerOp = pc.CreateAnswer();
        yield return answerOp;

        if (answerOp.IsError)
        {
            Debug.LogError("[WebRTC] CreateAnswer Error: " + answerOp.Error.message);
            yield break;
        }

        RTCSessionDescription answer = answerOp.Desc;

        var op2 = pc.SetLocalDescription(ref answer);
        yield return op2;

        // Answer 전송
        WebRTCSignalMessage msg = new WebRTCSignalMessage
        {
            type = "answer",
            sdp = answer.sdp
        };
        ws.Send(JsonUtility.ToJson(msg));

        Debug.Log("[WebRTC] Answer Sent");
    }

    void OnDestroy()
    {
        ws?.Close();
        pc?.Close();
        pc?.Dispose();
        if (webrtcLoop != null)
            StopCoroutine(webrtcLoop);
    }
}
