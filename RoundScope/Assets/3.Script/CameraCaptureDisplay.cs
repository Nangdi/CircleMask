using UnityEngine;
using UnityEngine.UI;

public class CameraCaptureDisplay : MonoBehaviour
{
    public RawImage display;         // UI에 띄울 RawImage
    public AspectRatioFitter fitter; // 비율 유지용 (선택사항)
    private WebCamTexture webcamTexture;

    void Start()
    {
        // 연결된 모든 비디오 장치(웹캠/캡처보드) 리스트 출력
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("캡처보드 또는 웹캠이 감지되지 않았습니다.");
            return;
        }

        // 첫 번째 장치 사용 (필요 시 인덱스로 조정)
        string camName = devices[0].name;
        Debug.Log("사용 중인 장치: " + camName);

        // WebCamTexture 생성
        webcamTexture = new WebCamTexture(camName, 1600, 1200, 60);
        display.texture = webcamTexture;
        webcamTexture.Play();
    }

    void Update()
    {
        if (webcamTexture == null) return;

        // 카메라 비율 유지 (선택)
        if (fitter != null)
        {
            float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
            fitter.aspectRatio = ratio;
        }

        // 화면 회전 대응 (캡처보드 방향 맞추기)
        display.rectTransform.localEulerAngles = new Vector3(0, 0, -webcamTexture.videoRotationAngle);

        // 미러링 대응 (전면카메라인 경우 반전 필요)
        display.uvRect = new Rect(0, 0, 1, 1);
    }

    void OnDisable()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
            webcamTexture.Stop();
    }
}
