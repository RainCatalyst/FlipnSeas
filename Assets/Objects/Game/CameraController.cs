using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float focusDuration;

    [Header("Events")]
    [SerializeField] private GameEventChannelSO gameEventChannel;
    
    private void OnEnable() {
        gameEventChannel.OnCameraFocused += FocusOnPosition;
    }

    private void OnDisable() {
        gameEventChannel.OnCameraFocused -= FocusOnPosition;
    }

    public void FocusOnPosition(Vector3 target) {
        _focusTween?.Kill();
        _focusTween = transform.DOMove(target, focusDuration);
    }

    private Tween _focusTween;
}
