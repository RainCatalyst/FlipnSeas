using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelSO[] levels;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private int levelIdx = -1;

    [Header("Events")]
    [SerializeField] private GameEventChannelSO gameEventChannel;

    public void LoadNextLevel() {
        levelIdx = Mathf.Clamp(levelIdx + 1, 0, levels.Length - 1);
        //levelManager.MoveToNextLevel();
        gameEventChannel.LoadLevel(levels[levelIdx]);
        gameEventChannel.FocusCamera(levelManager.transform.position);
    }

    private void OnInteract()
    {
        LoadNextLevel();
    }
}
