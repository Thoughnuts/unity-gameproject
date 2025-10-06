using UnityEngine;

public class PlayerSceneLoader : MonoBehaviour
{
    private void Start()
    {
        // Position player at spawn point when scene loads
        Vector2 spawnPos = PlayerSpawnManager.GetSpawnPosition();
        if (spawnPos != Vector2.zero)
        {
            transform.position = spawnPos;
        }
    }
}