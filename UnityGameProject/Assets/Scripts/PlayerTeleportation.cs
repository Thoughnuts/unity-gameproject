using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private Vector2 spawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Store spawn position for the next scene
            PlayerSpawnManager.SetSpawnPosition(spawnPosition);

            // Start transition
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName);
        }
    }
}