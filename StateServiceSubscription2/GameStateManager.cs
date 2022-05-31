using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private static Dictionary<Type, object> _mSerivces = new Dictionary<Type, object>();
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

    public void AddService(object service)
    {
        _mSerivces.Add(service.GetType(), service);
        print($"added {service.GetType()}");

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

                foreach (MethodInfo method in serviceType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (method.Name == "Bind")
                    {
                        Type[] genericTypes = method.GetGenericArguments();
                        var typeObject = genericTypes[0];
                        print($"Type Object is {typeObject.Name}");

                        if (_gameStatesDictionary.ContainsKey(targetGameState))
                        {
                            print($"searching for {typeObject.Name}");
                            print($"mServices has {_mSerivces.Count}");
                            print($"Comparing {typeObject} with {service.GetType()}");
                            print($"mServices[{typeObject.Name}] = {_mSerivces[service.GetType()]}  ");
                            //var test = _mSerivces[typeObject];
                            //print($"test is {test.Name}");

                            object[] bindingParameters = { _mSerivces[service.GetType()]};
                            method.Invoke(service, bindingParameters);
                        }                            
                    }
                }
            }
        }                    
    } 
}
