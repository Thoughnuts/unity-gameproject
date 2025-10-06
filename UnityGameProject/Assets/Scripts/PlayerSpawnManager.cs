using UnityEngine;

public static class PlayerSpawnManager
{
    private static Vector2 spawnPosition = Vector2.zero;

    public static void SetSpawnPosition(Vector2 position)
    {
        spawnPosition = position;
    }

    public static Vector2 GetSpawnPosition()
    {
        return spawnPosition;
    }
}