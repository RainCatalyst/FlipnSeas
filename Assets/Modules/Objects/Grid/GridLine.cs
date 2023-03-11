using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridLine : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] [ColorUsage(true, true)] private Color winColor;
    [SerializeField] [ColorUsage(true, true)] private Color loseColor;
    [SerializeField] private float yOffset;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.material.SetColor("_EmissionColor", gradient.Evaluate(1));
    }

    public void SetPath(List<GridCell> path)
    {
        var points = new Vector3[path.Count];
        _lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            points[i] = path[i].transform.position + Vector3.up * yOffset;
        }

        _lineRenderer.SetPositions(points);
    }

    public void RevealPath()
    {
        _revealTween?.Complete();

        _revealTween = DOTween.Sequence()
            .Append(DOTween.To((x) => _lineRenderer.startColor = gradient.Evaluate(x), 0, 1, 0.1f))
            .Append(DOTween.To((x) => _lineRenderer.endColor = gradient.Evaluate(x), 0, 1, 0.3f))
            .PrependInterval(0.15f);
    }

    public void Pulse(bool win)
    {
        var finalColor = gradient.Evaluate(1);
        if (_revealTween == null)
            _revealTween = DOTween.Sequence();

        if (win)
        {
            _revealTween.Append(DOTween.To((x) =>
            {
                var emissionColor = Color.Lerp(finalColor, winColor, x);
                _lineRenderer.material.SetColor("_EmissionColor", emissionColor);
            }, 0, 1, .25f));
        }
        else
        {
            _revealTween.Append(DOTween.To((x) =>
            {
                var emissionColor = Color.Lerp(finalColor, loseColor, x);
                _lineRenderer.material.SetColor("_EmissionColor", emissionColor);
            }, 0, 1, .25f).SetLoops(2, LoopType.Yoyo));
        }
    }

    private void OnDestroy()
    {
        _revealTween?.Kill();
    }

    private Sequence _revealTween;
    private LineRenderer _lineRenderer;
}