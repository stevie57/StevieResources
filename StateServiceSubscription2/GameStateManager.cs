using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private List<object> _servicesList = new List<object>();
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
                List<object> servicesList = new List<object>();
                _gameStatesDictionary[state] = servicesList;
            }
        }
    }

    public void Add(object service)
    {
        Type serviceType = service.GetType();
        foreach(Type interfaceType in serviceType.GetInterfaces())
        {
            if(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStateListener<>))
            {
                var argumentsArray =  interfaceType.GenericTypeArguments;
                Type targetGameState = argumentsArray[0];

                foreach (MethodInfo method in serviceType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (method.Name == "Bind")
                    {
                        Type[] genericTypes = method.GetGenericArguments();


                        if (_gameStatesDictionary.ContainsKey(targetGameState))
                        {


                            object[] bindingParameters = { genericTypes };
                            method.Invoke(service, bindingParameters);
                        }                            
                    }
                }
            }
        }
            
        
    } 

    private void Bind(object toBind)
    {
        Type toBindType = toBind.GetType();

        foreach(Type interfaceType in toBindType.GetInterfaces())
        {
            //if(interfaceType.)
        }
    }



}
