using UnityEngine;

public class HealthSystem : MonoBehaviour, IStateListener
{
    [SerializeField]
    private AGameApplicationState targetState;

    private void OnEnable()
    {
        GameStateManager.Instance.Bind(targetState, new ServiceSubscription(this, PlayStateSetup, PlayStateTearDown));       
    }

    private void OnDisable()
    {
        GameStateManager.Instance.UnBind(targetState, this);
    }

    private void PlayStateSetup()
    {
        print($"Health System Setup");
    }

    private void PlayStateTearDown()
    {
        print($"Health System Teardown");
    }
}