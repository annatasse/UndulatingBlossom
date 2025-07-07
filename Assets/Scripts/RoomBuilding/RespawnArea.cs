using System;
using UnityEngine;

public class RespawnArea : MonoBehaviour {

    // Assign in Inspector
    public Room room;
    public Transform felixSpawnPoint;
    public Transform annaSpawnPoint;
    //

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Felix")) {
            room.SetSpawnPoint(CharacterName.Felix, felixSpawnPoint.position);
        } else if (other.CompareTag("Anna")) {
            room.SetSpawnPoint(CharacterName.Anna, annaSpawnPoint.position);
        }
    }

    public Vector3 GetSpawnPoint(CharacterName character) {
        return character switch {
            CharacterName.Felix => felixSpawnPoint.position,
            CharacterName.Anna => annaSpawnPoint.position,
            _ => throw new ArgumentException($"{character} is not a known CharacterName")
        };
    }
}
