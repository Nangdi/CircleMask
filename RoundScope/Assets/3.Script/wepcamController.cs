using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class wepcamController : MonoBehaviour
{
    public RawImage display;
    private WebCamTexture webcamTexture;
    private string currentDeviceName;
    public int cameraIndex = 0;
    void Start()
    {
        // 연결된 모든 비디오 장치(웹캠, 캡처보드 등) 출력
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("캡처보드 또는 웹캠이 감지되지 않았습니다.");
            return;
        }

        // -----------------------
        // ✅ 사용할 장치 선택
        // -----------------------
        // 일반적으로 박스카메라가 연결된 캡처보드 이름이 "USB Video", "HDMI Capture" 등으로 뜹니다.
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"장치 이름: {devices[i].name}, 순서 : {i}");
        }
        //foreach (var dev in devices)
        //    Debug.Log("장치 이름: " + dev.name);

        // 첫 번째 장치 사용 (필요 시 인덱스 변경)
        currentDeviceName = devices[cameraIndex].name;
        Debug.Log("사용 중인 장치: " + currentDeviceName);

        // -----------------------
        // ✅ WebCamTexture 생성
        // -----------------------
        webcamTexture = new WebCamTexture(currentDeviceName, 1920, 1080, 30);
        display.texture = webcamTexture;
        webcamTexture.Play();
    }

}
