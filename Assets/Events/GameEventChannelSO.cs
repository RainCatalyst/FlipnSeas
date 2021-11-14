using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameEventChannel", menuName = "Events/Game Event Channel")]
public class GameEventChannelSO : ScriptableObject
{
    public UnityAction<LevelSO> OnLevelLoad;
    public UnityAction OnLevelStarted;
    public UnityAction OnLevelClosed;
    public UnityAction OnLevelWon;
    public UnityAction OnLevelLost;

    public void LoadLevel(LevelSO level)
    {
        OnLevelLoad?.Invoke(level);
    }

    public void StartLevel()
    {
        OnLevelStarted?.Invoke();
    }

    public void CloseLevel()
    {
        OnLevelClosed?.Invoke();
    }

    public void WinLevel()
    {
        OnLevelWon?.Invoke();
    }

    public void LoseLevel()
    {
        OnLevelLost?.Invoke();
    }
}