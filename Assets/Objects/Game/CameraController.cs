using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder;
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

    public void FocusOnPosition(Vector3 target, float distance) {
        distance = (distance - 1.8f) * 1.05f;
        
        _focusTween?.Kill();
        _focusTween = DOTween.Sequence();
        _focusTween.Append(transform.DOMove(target, focusDuration).SetEase(Ease.InOutQuad));
        print("Initial " + cameraHolder.localPosition);
        print("Final " + -cameraHolder.forward * distance);
        _focusTween.Join(cameraHolder.DOLocalMove(-cameraHolder.forward * distance, focusDuration).SetEase(Ease.InOutQuad));
    }

    private Sequence _focusTween;
}
