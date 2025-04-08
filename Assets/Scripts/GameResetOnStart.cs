using UnityEngine;

using UnityEngine;

public class GameStateResetOnStart : MonoBehaviour
{
    void Awake()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SampleScene")
        {
            GameState.returnWithHorse = false;
            GameState.followFemaleOnReturn = false;
            GameState.hasStartedRideNarration = false;
            GameState.hasPlayedReturnRideNarration = false;
            GameState.hasStartedInteriorNarration = false;
            Debug.Log("🧼 GameState reset in IntroScene.");
        }
        else
        {
            Debug.Log("↩️ Scene is not IntroScene — skipping GameState reset.");
        }
    }


}
