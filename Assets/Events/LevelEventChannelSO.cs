using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LevelEventChannel", menuName = "Events/Level Event Channel")]
public class LevelEventChannelSO : ScriptableObject
{
    public UnityAction OnFlipUsed;

    public void UseFlip()
    {
        OnFlipUsed?.Invoke();
    }
}