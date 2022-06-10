using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour, IStateListener<GameState_Play>
{

    void Start()
    {
        GameStateManager.Instance.Bind(this);
    }

    private void SetUp()
    {

    }

    private void TearDown()
    {

    }

    public void Bind(GameState_Play State)
    {
        print($"this has been invoked");
        print($"State is {State.GetType()}");

    }

}
