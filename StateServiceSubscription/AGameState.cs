using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGameApplicationState : ScriptableObject
{
    public GameStateType GameState;
}

public enum GameStateType
{
    Start,
    Play
}