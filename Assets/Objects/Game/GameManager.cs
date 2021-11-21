using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelSO[] levels;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private VisualEffect completeEffect;
    [SerializeField] private AudioSource completeSound;
    [SerializeField] private AudioSource failSound;
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
        loadSequence.PrependInterval(0.95f);
    }

    public void RestartLevel(float delay = 0f) {
        var restartSequnce = DOTween.Sequence();
        restartSequnce.AppendCallback(() => gameEventChannel.RestartLevel());
        restartSequnce.PrependInterval(delay);
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
        LoadNextLevel();
        completeEffect.Play();
        completeSound.PlayDelayed(0.2f);
    }

    private void OnLevelLost()
    {
        RestartLevel(0.45f);
        failSound.PlayDelayed(0.2f);
    }
}
