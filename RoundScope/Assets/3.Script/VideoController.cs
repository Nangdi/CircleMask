using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    private string videoName = "show.mp4";
    public GameObject videoCanvas;
    // Start is called before the first frame update
    void Start()
    {
        GetVideoFile();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
        }
    }
    private void GetVideoFile()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.source = VideoSource.Url;  // 스트리밍 경로 사용
        videoPlayer.url = path;
        //videoPlayer.Prepare();

        // 준비 완료 후 자동 재생
        videoPlayer.prepareCompleted += (VideoPlayer vp) =>
        {
            Debug.Log("✅ 비디오 준비 완료: " + vp.url);
            vp.Play();
        };
        Debug.Log("비디오 재생 시작: " + videoPlayer.url); 
        OnSketchEnd();
    }
    public void OnSketchEnd()
    {
        videoCanvas.SetActive(true);
        Debug.Log("🎞 영상 끝 → 처음부터 다시 재생");
        //videoPlayer.Stop();    // 재생을 완전히 멈추고
        videoPlayer.Play();    // 처음부터 다시 재생
    }
    public void OnSketchStart()
    {
        videoCanvas.SetActive(false);
        videoPlayer.Stop();
    }

}
