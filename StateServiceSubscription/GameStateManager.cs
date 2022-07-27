using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance; // Make sure there is only one game state manager

    private Dictionary<Type, AGameState> _statesInstanceDictionary = new Dictionary<Type, AGameState>();            // Dict of all game states created at run time
    private Dictionary<Type, List<object>> _gameStatesServicesDictionary = new Dictionary<Type, List<object>>();    // Dict of all services listening to each game state

    AGameState _currentState;   // current game state

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        CreateStateDictionary();
        _currentState = _statesInstanceDictionary[typeof(GameState_Setup)];
    }

    // Create instances of each game state and add to the State Dictionary
    private void CreateStateDictionary()
    {
        // Get all classess that inherit from AGameState at run time 
        var gameStates = Assembly.GetAssembly(typeof(AGameState)).GetTypes();

        foreach(Type state in gameStates)
        {
            // Make sure it is a Class, that it is not Abstract class and that it's inherited from AGameState
            if (state.IsClass && !state.IsAbstract && state.IsSubclassOf(typeof(AGameState)))
            {
            
                //Create a empty List that can be populated later for services who are listening to this specific gameState 
                List<object> servicesList = new List<object>();
                _gameStatesServicesDictionary[state] = servicesList;
                
                // Create an instance of the gameState type. 
                AGameState stateInstance = Activator.CreateInstance(state) as AGameState;
                _statesInstanceDictionary[state] = stateInstance;
            }
        }
    }

    public void Bind(object service)
    {
        AddService(service);
        SetupService(service, _currentState); // SetupService is and checks if the current state is where its supposed to set up
    }

    private void AddService(object service)
    {
        Type serviceType = service.GetType();
        
        // All systems that want to subscribe as a service to a gamestate implement the IStateListener Interface. 
        foreach (Type interfaceType in serviceType.GetInterfaces())
        {
            //Using Interface to filter and the implementation to find out which specific gameState the service is interested in.
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStateListener<>))
            {
                // The implementation should only have one gameState generic
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
                        // Interface requires the implementation of Setu
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
