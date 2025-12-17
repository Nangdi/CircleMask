using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Vuplex.Demos;
public enum State
{
    Play,
    Stop
}
public class CustomSPManager : SerialPortManager
{
    [SerializeField] private VideoController videoController;
    [SerializeField] private SimpleWebViewDemo webView;
    public State state = State.Stop;
    bool webstart;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
       
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ReceivedData("S1");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ReceivedData("E1");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ReceivedData("R1");
        }
    }
    protected override void ReceivedData(string data)
    {
        switch (data)
        {
            case "S1":
                if (!webstart)
                {
                    webstart = true;
                    StartCoroutine(delay(8));


                }
                else
                {
                state = State.Play;

                }
                break;
            case "E1":
                state = State.Stop;
                break;
            case "R1":
               
                break;
        }


        switch (state)
        {
            case State.Play:
                //홍보영상끄고 카메라보여주기
                videoController.OnSketchStart();
                break;
            case State.Stop:
                //홍보영상키기  : 루프
                videoController.OnSketchEnd();
                break;
        }
    }
    private IEnumerator delay(float time)
    {
        webView.StartConnectWeb();
        Debug.Log("딜레이전");
        yield return new WaitForSeconds(time);
        Debug.Log("딜레이후");
        state = State.Play;
        videoController.OnSketchStart();
    }
}
