using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using System;



public class PlayerState : MonoBehaviour, EventListener<GameEvent>
{
    IPlayerStates currentState;

    public void OnEvent(GameEvent eventType)
    {
        if(eventType.EventName == "StateChange")
        {
            UpdateCurrentState();
        }
    }

    private void UpdateCurrentState()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
