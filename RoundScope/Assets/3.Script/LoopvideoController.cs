using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LoopvideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas;

    private int currentIndex = 1;
    private const int maxIndex = 5;
    private const string baseName = "show";
    private const string extension = ".mp4";

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;    // 영상 정상 종료
        videoPlayer.errorReceived += OnVideoError;          // 영상 로딩 실패
        LoadAndPlayVideo(currentIndex);
    }

    private string GetVideoPath(int index)
    {
        string fileName = baseName + index + extension;
        return System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
    }

    private void LoadAndPlayVideo(int index)
    {
        string path = GetVideoPath(index);

        Debug.Log($"📂 [{index}] 로드 시도: {path}");

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= OnVideoPrepared;

        Debug.Log("▶ 재생 시작: " + vp.url);
        videoCanvas.SetActive(true);
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("🔁 영상 끝 → 다음 영상 재생");
        LoadNextVideo();
    }

    private void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogWarning($"⚠ 영상 로드 실패({message}) → 다음 파일로 넘어갑니다.");
        LoadNextVideo();
    }

    private void LoadNextVideo()
    {
        currentIndex++;
        if (currentIndex > maxIndex)
            currentIndex = 1;

        LoadAndPlayVideo(currentIndex);
    }

    // 필요 시 외부에서 호출
    public void OnSketchStart()
    {
        videoCanvas.SetActive(false);
        videoPlayer.Stop();
    }
}
