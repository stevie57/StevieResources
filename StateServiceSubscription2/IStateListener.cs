using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateListener<T> where T : AGameState
{
    public void Bind( T State);
}
