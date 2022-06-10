using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private Dictionary<string, Type> _states = new Dictionary<string, Type>();
    private Dictionary<Type, List<object>> _gameStatesDictionary = new Dictionary<Type, List<object>>();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        CreateStateDictionary();
    }

    private void Start()
    {

    }

    private void CreateStateDictionary()
    {
        var gameStates = Assembly.GetAssembly(typeof(AGameState)).GetTypes();

        foreach(Type state in gameStates)
        {
            if (state.IsClass && !state.IsAbstract && state.IsSubclassOf(typeof(AGameState)))
            {
                if (!_states.ContainsKey(state.GetType().ToString()))
                    _states[state.GetType().ToString()] = state; 

                List<object> servicesList = new List<object>();
                _gameStatesDictionary[state] = servicesList;
            }
        }
    }

    public void AddService(object service)
    {
        //print($"added {service.GetType()}");
    }

    public void Bind(object service)
    {
        Type serviceType = service.GetType();
        foreach(Type interfaceType in serviceType.GetInterfaces())
        {
            if(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStateListener<>))
            {
                var argumentsArray =  interfaceType.GenericTypeArguments;
                Type targetGameState = argumentsArray[0];

                if (!_gameStatesDictionary.ContainsKey(targetGameState))
                {
                    print($"gameState {targetGameState} not found. ");
                    return;
                }
                else
                    print($"gamestate {targetGameState} is available in the gameStatesDictionary");


                foreach (MethodInfo method in serviceType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (method.Name == "Bind")
                    {
                        print($"Target game state is {targetGameState}");
                        object[] bindingParameters = { targetGameState  }; 
                        method.Invoke(service, bindingParameters);                                                    
                    }
                }
            }
        }                    
    } 
}
