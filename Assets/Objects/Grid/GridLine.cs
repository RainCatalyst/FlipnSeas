using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridLine : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private float yOffset;
    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetPath(List<GridCell> path) {
        var points = new Vector3[path.Count];
        _lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++) {
            points[i] = path[i].transform.position + Vector3.up * yOffset;
        }
        _lineRenderer.SetPositions(points);
    }

    public void RevealPath() {
        _revealTween?.Complete();

        // Color transparent = new Color(0, 0, 0, 0);
        // Color opaque = new Color(0, 0, 0, 1);
        
        _revealTween = DOTween.Sequence()
            .Append(DOTween.To((x) => _lineRenderer.startColor = gradient.Evaluate(x), 0, 1, 0.1f))
            .Append(DOTween.To((x) => _lineRenderer.endColor = gradient.Evaluate(x), 0, 1, 0.3f)); //_lineRenderer.DOColor(new Color2(transparent, transparent), new Color2(opaque, opaque), 0.25f);
    }

    private void OnDestroy() {
        _revealTween?.Kill();
    }

    private Sequence _revealTween;
    private LineRenderer _lineRenderer;
}
