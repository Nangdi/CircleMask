using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public RectTransform cam1Rect;
    public RectTransform cam2Rect;
    public RectTransform currentRect;
    public Camera cam;
    private bool isLeftCamSetting = true;
    private bool camDistanceSetting = false;

    void Start()
    {
        cam1Rect.anchoredPosition = new Vector2(JsonManager.instance.gameSettingData.cam1horizontlal, cam1Rect.anchoredPosition.y);
        cam2Rect.anchoredPosition = new Vector2(JsonManager.instance.gameSettingData.cam2horizontlal, JsonManager.instance.gameSettingData.cam2vertical);
        cam.fieldOfView = JsonManager.instance.gameSettingData.camDistance;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isLeftCamSetting = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isLeftCamSetting = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            camDistanceSetting = !camDistanceSetting;
        }
        SetHorizontal();
    }
    private void SetHorizontal()
    {
        float tempX = Input.GetAxis("Horizontal");
        float tempY = Input.GetAxis("Vertical");
        if (!camDistanceSetting)
        {
            if (isLeftCamSetting)
            {
                currentRect = cam1Rect;
            }
            else
            {
                currentRect = cam2Rect;
            }
        }
        
        float scaleDelta = tempY * 0.02f;   // 조절 속도 (원하면 변경 가능)
        if (camDistanceSetting)
        {
            cam.fieldOfView += scaleDelta;

        }
        Vector3 newScale = currentRect.localScale + new Vector3(scaleDelta, scaleDelta, scaleDelta);
        currentRect.anchoredPosition = new Vector2(currentRect.anchoredPosition.x - tempX, currentRect.anchoredPosition.y);
        currentRect.anchoredPosition = new Vector2(currentRect.anchoredPosition.x, currentRect.anchoredPosition.y - tempY);
        //currentRect.localScale = newScale;
    }

    private void OnApplicationQuit()
    {
        JsonManager.instance.gameSettingData.cam1horizontlal = cam1Rect.anchoredPosition.x;
        JsonManager.instance.gameSettingData.cam2horizontlal = cam2Rect.anchoredPosition.x;
        JsonManager.instance.gameSettingData.cam2vertical = cam2Rect.anchoredPosition.y;
        JsonManager.instance.gameSettingData.camDistance = cam.fieldOfView;
        JsonManager.instance.SaveGameSetting();
    }
}
