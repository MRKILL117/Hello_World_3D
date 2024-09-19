using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Player")]
    [Tooltip("The player prefab to spawn")]
    public NetworkObject playerPrefab;
    [Tooltip("The spawn radius for the player based on the game manager position")]
    public float spawnRadius = 5f;

    void Start()
    {
        Debug.Log("GameManager started");
    }

    public override void Spawned()
    {
        Debug.Log("GameManager spawned");
        var randomPosOffset = Random.insideUnitCircle * this.spawnRadius;
        var spawnPos = this.transform.position + new Vector3(randomPosOffset.x, 1, randomPosOffset.y);

        this.Runner.Spawn(this.playerPrefab, spawnPos, Quaternion.identity);
    }
}
