using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LevelEventChannel", menuName = "Events/Level Event Channel")]
public class LevelEventChannelSO : ScriptableObject
{
    public UnityAction<int> OnFlipsUpdated;

    public void UpdateFlips(int flips) {
        OnFlipsUpdated?.Invoke(flips);
    }
}