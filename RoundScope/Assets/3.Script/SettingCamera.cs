using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
[System.Serializable]
public class Option
{
    public string propertyName;
    public Slider slider;
    public TMP_InputField inputField;

}
public class SettingCamera : MonoBehaviour
{

    public List<Option> optionList = new List<Option>();
    public Material material;
    private string _Feather;

    public GameObject SettingPanel;
    // Start is called before the first frame update
    void Start()
    {
        InitSaveData();


        optionList[0].slider.value = material.GetFloat(optionList[0].propertyName);
        optionList[1].slider.value = material.GetFloat(optionList[1].propertyName)/2;
        foreach (var item in optionList)
        {
            UpdateText(item);

        }
        //optionList[1].inputField.text = optionList[1].slider.value.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingPanel.SetActive(!SettingPanel.activeSelf);
            Cursor.visible = SettingPanel.activeSelf;
        }
    }
    public void SetFeather(float value)
    {
        material.SetFloat(optionList[0].propertyName , value);
    }
    public void SetRatioX(float value)
    {
        float fix = 2 * value;
        material.SetFloat(optionList[1].propertyName, fix);
        UpdateText(optionList[1]);
    }
    public void SetRatioY(float value)
    {
        float fix = 2 * value;
        material.SetFloat(optionList[2].propertyName, fix);
        UpdateText(optionList[2]);
    }
    public void SetRatioX_Input(string value_string)
    {
        float temp = float.Parse(value_string);
        float value = Mathf.Clamp(temp, 0, 2);
        SetUIValue(optionList[1], value);
    }
    public void SetRatioY_Input(string value_string)
    {
        float temp = float.Parse(value_string);
        float value = Mathf.Clamp(temp, 0, 2);
        SetUIValue(optionList[2], value);
    }

    private void UpdateText(Option option)
    {
        option.inputField.text = material.GetFloat(option.propertyName).ToString();
    }
    private void SetUIValue(Option option , float value)
    {
        material.SetFloat(option.propertyName, value);
        option.inputField.text = value.ToString();
        option.slider.value = value/2;
    }
    private void InitSaveData()
    {
        material.SetFloat(optionList[0].propertyName, JsonManager.instance.gameSettingData.feather);
        material.SetFloat(optionList[1].propertyName, JsonManager.instance.gameSettingData.aspectX);
        material.SetFloat(optionList[2].propertyName, JsonManager.instance.gameSettingData.aspectY);
    }
    public void SaveSetting()
    {
        JsonManager.instance.gameSettingData.feather = material.GetFloat(optionList[0].propertyName);
        JsonManager.instance.gameSettingData.aspectX = material.GetFloat(optionList[1].propertyName);
        JsonManager.instance.gameSettingData.aspectY = material.GetFloat(optionList[2].propertyName);

        JsonManager.SaveData(JsonManager.instance.gameSettingData, JsonManager.instance.gameDataPath);
    }
    private void OnApplicationQuit()
    {
        SaveSetting();
    }
}

