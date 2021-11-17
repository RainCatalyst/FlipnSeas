using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelSO[] levels;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private int levelIdx = -1;

    [Header("Events")]
    [SerializeField] private GameEventChannelSO gameEventChannel;

    private void OnEnable() {
        gameEventChannel.OnLevelWon += OnLevelWon;
        gameEventChannel.OnLevelLost += OnLevelLost;
    }

    private void OnDisable() {
        gameEventChannel.OnLevelWon -= OnLevelWon;
        gameEventChannel.OnLevelLost -= OnLevelLost;
    }

    public void LoadNextLevel() {
        levelIdx = Mathf.Clamp(levelIdx + 1, 0, levels.Length - 1);
        
        var loadSequence = DOTween.Sequence();
        loadSequence.AppendCallback(() => gameEventChannel.LoadLevel(levels[levelIdx]));
        loadSequence.InsertCallback(0.5f, () => gameEventChannel.FocusCamera(levelManager.transform.position));
        loadSequence.PrependInterval(0.5f);
    }

    public void RestartLevel() {
        var restartSequnce = DOTween.Sequence();
        restartSequnce.AppendCallback(() => gameEventChannel.RestartLevel());
        restartSequnce.PrependInterval(0.45f);
    }

    private void OnInteract()
    {
        LoadNextLevel();
    }

    private void OnLevelWon()
    {
        print("won");
        LoadNextLevel();
    }

    private void OnLevelLost()
    {
        print("lost");
        RestartLevel();
    }
}
