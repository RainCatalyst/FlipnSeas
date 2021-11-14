using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelSO testLevel;

    [Header("Events")]
    [SerializeField] private GameEventChannelSO gameEventChannel;

    private void OnInteract() {
        gameEventChannel.LoadLevel(testLevel);
    }
}
