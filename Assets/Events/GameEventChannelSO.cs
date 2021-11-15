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

    public UnityAction<Vector3> OnCameraFocused;

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

    public void FocusCamera(Vector3 position) {
        OnCameraFocused?.Invoke(position);
    }
}