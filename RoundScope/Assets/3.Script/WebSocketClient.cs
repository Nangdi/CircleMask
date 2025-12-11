//using System;
//using System.Collections;
//using System.Text;
//using UnityEngine;
//using UnityEngine.UI;
//using Unity.WebRTC;
//using UnityEngine.Networking;

//public class WebRTC_HTTP_Receiver : MonoBehaviour
//{
//    public RawImage videoImage;

//    private RTCPeerConnection pc;
//    private Texture2D receiveTex;

//    private string serverUrl = "http://192.168.0.199:9000/webrtc/offer";

//    IEnumerator Start()
//    {
//        receiveTex = new Texture2D(2, 2);

//        var config = new RTCConfiguration
//        {
//            iceServers = new RTCIceServer[]
//            {
//                new RTCIceServer{ urls = new string[] { "stun:stun.l.google.com:19302" } }
//            }
//        };

//        pc = new RTCPeerConnection(ref config);

//        pc.OnTrack = OnTrackReceived;

//        // 서버는 recvonly 스트림 요구 → Unity도 Transceiver 추가해야 함
//        pc.AddTransceiver(TrackKind.Video, new RTCRtpTransceiverInit
//        {
//            direction = RTCRtpTransceiverDirection.RecvOnly
//        });

//        // Offer 생성
//        var offerOp = pc.CreateOffer();
//        yield return offerOp;
//        var offer = offerOp.Desc;
//        yield return pc.SetLocalDescription(ref offer);

//        // Offer를 HTTP로 서버에 전송
//        yield return StartCoroutine(SendOfferToServer(offer));

//        // WebRTC Update 루프
//        StartCoroutine(WebRTC.Update());
//    }

//    IEnumerator SendOfferToServer(RTCSessionDescription offer)
//    {
//        string json = JsonUtility.ToJson(offer);
//        byte[] body = Encoding.UTF8.GetBytes(json);

//        UnityWebRequest req = new UnityWebRequest(serverUrl, "POST");
//        req.uploadHandler = new UploadHandlerRaw(body);
//        req.downloadHandler = new DownloadHandlerBuffer();
//        req.SetRequestHeader("Content-Type", "application/json");

//        yield return req.SendWebRequest();

//        if (req.result != UnityWebRequest.Result.Success)
//        {
//            Debug.LogError("Offer send FAILED: " + req.error);
//            yield break;
//        }

//        // 서버로부터 answer SDP 받기
//        string answerJson = req.downloadHandler.text;

//        RTCSessionDescription answer =
//            JsonUtility.FromJson<RTCSessionDescription>(answerJson);

//        var op = pc.SetRemoteDescription(ref answer);
//        yield return op;

//        Debug.Log("Connected to RTC server!");
//    }

//    private void OnTrackReceived(RTCTrackEvent e)
//    {
//        Debug.Log("Track received: " + e.Track.Kind);

//        if (e.Track is VideoStreamTrack video)
//        {
//            video.OnVideoReceived += tex =>
//            {
//                videoImage.texture = tex;
//            };
//        }
//    }
//}
