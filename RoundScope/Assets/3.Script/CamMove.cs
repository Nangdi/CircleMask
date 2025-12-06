using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public RectTransform cam1Rect;
    public RectTransform cam2Rect;
    public RectTransform currentRect;
    private bool isLeftCamSetting = true;

    void Start()
    {
        cam1Rect.anchoredPosition = new Vector2(JsonManager.instance.gameSettingData.cam1horizontlal, 0);
        cam2Rect.anchoredPosition = new Vector2(JsonManager.instance.gameSettingData.cam2horizontlal, 0);
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
        SetHorizontal();
    }
    private void SetHorizontal()
    {
        float tempX = Input.GetAxis("Horizontal");
        if (isLeftCamSetting)
        {
            currentRect = cam1Rect;
        }
        else
        {
            currentRect = cam2Rect;
        }

        currentRect.anchoredPosition = new Vector2(currentRect.anchoredPosition.x + tempX, 0);
    }

    private void OnApplicationQuit()
    {
        JsonManager.instance.gameSettingData.cam1horizontlal = cam1Rect.anchoredPosition.x;
        JsonManager.instance.gameSettingData.cam2horizontlal = cam2Rect.anchoredPosition.x;
        JsonManager.instance.SaveGameSetting();
    }
}
