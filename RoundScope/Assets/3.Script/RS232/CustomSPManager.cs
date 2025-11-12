using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum State
{
    Play,
    Stop
}
public class CustomSPManager : SerialPortManager
{
    public State state;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void ReceivedData(string data)
    {
        switch (state)
        {
            case State.Play:
                //홍보영상끄고 카메라보여주기
                break;
            case State.Stop:
                //홍보영상키기  : 루프
                break;
        }
    }
}
