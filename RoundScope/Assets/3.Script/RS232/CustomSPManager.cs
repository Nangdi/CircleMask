using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public enum State
{
    Play,
    Stop
}
public class CustomSPManager : SerialPortManager
{
    [SerializeField] private VideoController videoController;
    public State state = State.Stop;
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
    }
    protected override void ReceivedData(string data)
    {
        switch (data)
        {
            case "S1":
                state = State.Play;
                break;
            case "E1":
                state = State.Stop;
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
  
}
