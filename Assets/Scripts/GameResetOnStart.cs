using UnityEngine;

using UnityEngine;

public class GameStateResetOnStart : MonoBehaviour
{
    void Awake()
    {
        // Only reset if we're not returning with the horse
        if (!GameState.returnWithHorse && !GameState.followFemaleOnReturn)
        {
            GameState.returnWithHorse = false;
            GameState.followFemaleOnReturn = false;
            Debug.Log("🧼 GameState reset on start (normal)");
        }
        else
        {
            Debug.Log("↩️ Returning from cutscene — skipping GameState reset");
        }
    }

}
