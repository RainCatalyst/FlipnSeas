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

    private void Start() {
        LoadNextLevel();
    }

    private void OnEnable() {
        gameEventChannel.OnLevelWon += OnLevelWon;
        gameEventChannel.OnLevelLost += OnLevelLost;
    }

    private void OnDisable() {
        gameEventChannel.OnLevelWon -= OnLevelWon;
        gameEventChannel.OnLevelLost -= OnLevelLost;
    }

    public void LoadNextLevel() {
        gameEventChannel.CloseLevel();
        levelIdx = Mathf.Clamp(levelIdx + 1, 0, levels.Length - 1);
        
        var loadSequence = DOTween.Sequence();
        loadSequence.AppendCallback(() => gameEventChannel.LoadLevel(levels[levelIdx]));
        loadSequence.InsertCallback(0.5f, () => 
            gameEventChannel.FocusCamera(levelManager.transform.position, levelManager.LevelSize.y));
        loadSequence.PrependInterval(0.5f);
    }

    public void RestartLevel(float delay = 0f) {
        var restartSequnce = DOTween.Sequence();
        restartSequnce.AppendCallback(() => gameEventChannel.RestartLevel());
        restartSequnce.PrependInterval(delay);
    }

    private void OnClick() {
        if (_waitForClick) {
            _waitForClick = false;
            if (_levelWon)
                LoadNextLevel();
            else
                RestartLevel(0.45f);
        }
    }

    private void OnInteract()
    {
        LoadNextLevel();
    }

    public void OnRestart()
    {
        RestartLevel();
    }

    private void OnLevelWon()
    {
        _waitForClick = true;
        _levelWon = true;
    }

    private void OnLevelLost()
    {
        RestartLevel(0.45f);
    }

    private bool _waitForClick = false;
    private bool _levelWon = false;
}
