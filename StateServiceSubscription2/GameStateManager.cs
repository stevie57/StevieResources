using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private Dictionary<Type, AGameState> _statesInstanceDictionary = new Dictionary<Type, AGameState>(); 
    private Dictionary<Type, List<object>> _gameStatesServicesDictionary = new Dictionary<Type, List<object>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        CreateStateDictionary();
    }

    private void CreateStateDictionary()
    {
        var gameStates = Assembly.GetAssembly(typeof(AGameState)).GetTypes();

        foreach(Type state in gameStates)
        {
            if (state.IsClass && !state.IsAbstract && state.IsSubclassOf(typeof(AGameState)))
            {
                List<object> servicesList = new List<object>();
                _gameStatesServicesDictionary[state] = servicesList;

                AGameState stateInstance = Activator.CreateInstance(state) as AGameState;
                _statesInstanceDictionary[state] = stateInstance;
            }
        }
    }

    public void Bind(object service)
    {
        AddService(service);
    }

    private void AddService(object service)
    {
        Type serviceType = service.GetType();
        foreach (Type interfaceType in serviceType.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStateListener<>))
            {
                var argumentsArray = interfaceType.GenericTypeArguments;
                Type targetGameState = argumentsArray[0];

                if (!_gameStatesServicesDictionary.ContainsKey(targetGameState))
                {
                    print($"gameState {targetGameState} not found. ");
                    return;
                }
                else
                    print($"gamestate {targetGameState} is available in the gameStatesDictionary");

                _gameStatesServicesDictionary[targetGameState].Add(service);


                foreach (MethodInfo method in serviceType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (method.Name == "Setup")
                    {
                        print($"Target game state is {targetGameState}");

                        object[] bindingParameters = { _statesInstanceDictionary[targetGameState] };
                        method.Invoke(service, bindingParameters);
                    }
                }
            }
        }
    }
}
