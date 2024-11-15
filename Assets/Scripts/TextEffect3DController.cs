using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextEffect3DController : MonoBehaviour
{
    [SerializeField] private float StartScale = .1f;
    [SerializeField] private float TargetScale = .1f;
    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        PlayEffect();
    }

    public void SetText(string value)
    {
        textMesh.text = value;
    }

    /// <summary>
    /// Plays a visual effect on the object by animating its position, scale, and text color using DOTween.
    /// The effect includes a movement upwards, scaling up, and fading out the color, followed by the destruction of the object.
    /// </summary>
    public void PlayEffect()
    {
        transform.SetParent(null);

        Vector3 startPosition = transform.position;

        Vector3 targetPosition = startPosition + new Vector3(0, 2, 0);

        Vector3 startScale = Vector3.one * StartScale;
        Vector3 targetScale = Vector3.one * TargetScale;

        Color startColor = textMesh.color;
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad)); //Move Up
        sequence.Join(transform.DOScale(targetScale, 1f).SetEase(Ease.OutQuad)); //Scale Up
        sequence.Append(DOTween.ToAlpha(() => textMesh.color, x => textMesh.color = x, 0f, 0.5f)); // Fade-out

        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
