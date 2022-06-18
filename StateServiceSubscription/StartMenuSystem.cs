using UnityEngine;

public class StartMenuSystem : MonoBehaviour, IStateListener<GameState_Setup>
{
    [SerializeField] private GameObject _setupCube;

    private void Start()
    {
        GameStateManager.Instance.Bind(this);       
    }

    public void Setup(GameState_Setup gameState)
    {
        print($"Start menu is setting up {gameState}");
        _setupCube.SetActive(true);
    }

    public void TearDown(GameState_Setup gameState)
    {
        print($"Start menu is tearing down {gameState}");
        _setupCube.SetActive(false);
    }
}