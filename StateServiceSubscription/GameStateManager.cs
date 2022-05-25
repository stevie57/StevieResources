using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [SerializeField]
    private List<AGameApplicationState> _statesList = new List<AGameApplicationState>();

    [SerializeReference]
    [SerializeField]
    private AGameApplicationState _currentGameState;

    private Dictionary<AGameApplicationState, List<ServiceSubscription>> _stateDictionary = new Dictionary<AGameApplicationState, List<ServiceSubscription>>();

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
        NewState(_statesList[0]);
    }

    private void CreateStateDictionary()
    {
        foreach (AGameApplicationState state in _statesList)
        {
            List<ServiceSubscription> stateListeners = new List<ServiceSubscription>();
            _stateDictionary[state] = stateListeners; 
        }
    }

    public void Bind(AGameApplicationState targetState, ServiceSubscription stateAction)
    {
        if (CheckStateAvailable(targetState))
        {
            _stateDictionary[targetState].Add(stateAction);
        }
    }

    public void UnBind(AGameApplicationState targetState, IStateListener listener)
    {
        if (CheckStateAvailable(targetState))
        {
            var listenersList = GetServices(targetState);
            for (int i = 0; i < listenersList.Count; i++)
            {
                if (listenersList[i].Service == listener)
                    listenersList.RemoveAt(i);
            }
        }
    }

        private bool CheckStateAvailable(AGameApplicationState newState)
    {
        return _statesList.Contains(newState);
    }

    private List<ServiceSubscription> GetServices(AGameApplicationState State)
    {
        return _stateDictionary[State];
    } 

    public void NewState(AGameApplicationState newState)
    {
        if (CheckStateAvailable(newState))
        {
            if(_currentGameState != null)
            {
                var StateServiceSubscribers = GetServices(_currentGameState);
                foreach(ServiceSubscription Service in StateServiceSubscribers)
                {
                    Service.TearDownCallback();
                }
            }               

            _currentGameState = newState;
            List<ServiceSubscription> listeners = GetServices(newState);
            foreach (ServiceSubscription Service in listeners)
            {
                Service.SetupCallback();                
            }
        }
    }

    [ContextMenu("Play Game State")]
    public void PlayGameState()
    {
        NewState(_statesList[1]);
    }

    [ContextMenu("Start Game State")]
    public void StartGameState()
    {
        NewState(_statesList[0]);
    }
}
