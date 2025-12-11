using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.WebRTC;

public class HttpWebRTCViewer : MonoBehaviour
{
    [Header("UI")]
    public RawImage videoImage;

    [Header("Server")]
    public string serverBaseUrl = "http://192.168.0.199:9000";

    private RTCPeerConnection pc;

    IEnumerator Start()
    {
        // ----------------------------
        // 1. PeerConnection 생성
        // ----------------------------
        var config = new RTCConfiguration
        {
            iceServers = new[] {
                new RTCIceServer {
                    urls = new[] { "stun:stun.l.google.com:19302" }
                }
            }
        };

        pc = new RTCPeerConnection(ref config);

        pc.OnIceConnectionChange = state =>
        {
            Debug.Log($"[ICE] {state}");
        };

        // ----------------------------
        // 2. RecvOnly 비디오 트랜시버 생성
        // ----------------------------
        var transceiver = pc.AddTransceiver(
            TrackKind.Video,
            new RTCRtpTransceiverInit
            {
                direction = RTCRtpTransceiverDirection.RecvOnly
            });

        // ----------------------------
        // 3. 코덱 강제 (H.264)
        // ----------------------------
        var caps = RTCRtpSender.GetCapabilities(TrackKind.Video);
        var h264 = caps.codecs.Where(c => c.mimeType.Contains("H264")).ToArray();

        if (h264.Length > 0)
        {
            transceiver.SetCodecPreferences(h264);
            Debug.Log("[Codec] Forced H264 codec.");
        }
        else
        {
            Debug.LogWarning("[Codec] H264 codec NOT found in capabilities.");
        }

        // ----------------------------
        // 4. onTrack 이벤트 처리
        // ----------------------------
        pc.OnTrack = e =>
        {
            Debug.Log("===== OnTrack EVENT =====");
            Debug.Log($"Track Kind: {e.Track.Kind}");
            Debug.Log($"Track ID: {e.Track.Id}");
            Debug.Log($"Track Enabled: {e.Track.Enabled}");

            if (e.Streams != null)
            {
                foreach (var s in e.Streams)
                    Debug.Log($"Stream ID: {s.Id}");
            }

            if (e.Track is VideoStreamTrack videoTrack)
            {
                Debug.Log("[Track] VideoStreamTrack received.");

                videoTrack.OnVideoReceived += tex =>
                {
                    Debug.Log($"[VIDEO FRAME] {tex.width}x{tex.height}");
                    videoImage.texture = tex;
                };
            }
        };

        // ----------------------------
        // 5. Offer 생성
        // ----------------------------
        var offerOp = pc.CreateOffer();
        yield return offerOp;

        if (offerOp.IsError)
        {
            Debug.LogError($"CreateOffer Error: {offerOp.Error.message}");
            yield break;
        }

        var offer = offerOp.Desc;

        // ----------------------------
        // 6. LocalDescription 설정
        // ----------------------------
        var localOp = pc.SetLocalDescription(ref offer);
        yield return localOp;

        if (localOp.IsError)
        {
            Debug.LogError($"SetLocalDescription Error: {localOp.Error.message}");
            yield break;
        }

        // ----------------------------
        // 7. HTTP로 Offer 전송 → Answer 받기
        // ----------------------------
        string url = serverBaseUrl.TrimEnd('/') + "/webrtc/offer";
        yield return StartCoroutine(SendOffer(url, offer));

        // ----------------------------
        // 8. WebRTC Update 루프 시작 (매우 중요!!)
        // ----------------------------
        StartCoroutine(WebRTC.Update());
    }


    // =====================================================================
    // OFFER 전송 → ANSWER 수신
    // =====================================================================
    IEnumerator SendOffer(string url, RTCSessionDescription offer)
    {
        string escapedSdp = EscapeJson(offer.sdp);
        string json = $"{{\"sdp\":\"{escapedSdp}\",\"type\":\"offer\"}}";

        using (var client = new HttpClient())
        {
            Debug.Log("[HTTP] Sending Offer JSON...");
            var respTask = client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            while (!respTask.IsCompleted)
                yield return null;

            if (respTask.IsFaulted)
            {
                Debug.LogError("[HTTP] Offer POST failed: " + respTask.Exception?.Message);
                yield break;
            }

            var response = respTask.Result;
            var readTask = response.Content.ReadAsStringAsync();
            while (!readTask.IsCompleted)
                yield return null;

            Debug.Log("[HTTP] Answer JSON: " + readTask.Result);

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError("[HTTP] Server Error: " + readTask.Result);
                yield break;
            }

            RTCSessionDescription answer =
                JsonUtility.FromJson<RTCSessionDescription>(readTask.Result);

            // RemoteDescription 설정
            var remoteOp = pc.SetRemoteDescription(ref answer);
            yield return remoteOp;

            if (remoteOp.IsError)
                Debug.LogError($"SetRemoteDescription Error: {remoteOp.Error.message}");
        }
    }


    // =====================================================================
    // JSON Escape
    // =====================================================================
    private string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
    }

    private void OnDestroy()
    {
        pc?.Close();
    }
}
