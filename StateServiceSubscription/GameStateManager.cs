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

    AGameState _currentState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        CreateStateDictionary();
        _currentState = _statesInstanceDictionary[typeof(GameState_Setup)];
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
        SetupService(service, _currentState);
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

                _gameStatesServicesDictionary[targetGameState].Add(service);
                print($"binding {service.ToString()} to {targetGameState.ToString()}");
            }
        }
    }

    private void TransitionToState(AGameState newState)
    {
        if(_currentState != newState)
        {
            List<object> ServicesToTearDown = _gameStatesServicesDictionary[_currentState.GetType()];
            if(ServicesToTearDown != null)
            {
                foreach(object service in ServicesToTearDown)
                {
                    TearDown(service);
                }
            }

            List<object> ServicesToSetUp = _gameStatesServicesDictionary[newState.GetType()];
            if(ServicesToSetUp != null)
            {
                foreach(object service in ServicesToSetUp)
                {
                    SetupService(service, newState);
                }
            }
        }
    }

    private void SetupService(object service, AGameState newState)
    {
        Type serviceType = service.GetType();
        foreach (Type interfaceType in serviceType.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStateListener<>))
            {
                var argumentsArray = interfaceType.GenericTypeArguments;
                Type interfaceGameState = argumentsArray[0];  // interface target game state 

                if (newState.GetType() == interfaceGameState)
                {
                    foreach (MethodInfo method in serviceType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                    {
                        if (method.Name == "Setup")
                        {
                            var methodParamaters = method.GetParameters();
                            string stateName = interfaceGameState.ToString();

                            foreach (ParameterInfo param in methodParamaters)
                            {
                                if (param.ToString().Contains(stateName))
                                {
                                    object[] bindingParameters = { _statesInstanceDictionary[interfaceGameState] };
                                    method.Invoke(service, bindingParameters);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        _currentState = newState;
    }

    [ContextMenu("Transition To Play")]
    public void TransitionToPlayState()
    {
        print($"Game state manager transition to Play State");
        AGameState playState = _statesInstanceDictionary[typeof(GameState_Play)];
        TransitionToState(playState);
    }


    [ContextMenu("Transition To Setup")]
    public void TransitionToSetUpState()
    {
        print($"Game state manager transition to Setup State");
        AGameState playState = _statesInstanceDictionary[typeof(GameState_Setup)];
        TransitionToState(playState);
    }

    private void TearDown(object service)
    {
        Type serviceType = service.GetType();
        foreach (Type interfaceType in serviceType.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStateListener<>))
            {
                var argumentsArray = interfaceType.GenericTypeArguments;
                Type interfaceGameState = argumentsArray[0];  // interface target game state 

                if (_currentState.GetType() == interfaceGameState)
                {
                    foreach (MethodInfo method in serviceType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                    {
                        if (method.Name == "TearDown")
                        {
                            var methodParamaters = method.GetParameters();
                            string stateName = interfaceGameState.ToString();

                            foreach (ParameterInfo param in methodParamaters)
                            {
                                if (param.ToString().Contains(stateName))
                                {
                                    object[] bindingParameters = { _statesInstanceDictionary[interfaceGameState] };
                                    method.Invoke(service, bindingParameters);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
