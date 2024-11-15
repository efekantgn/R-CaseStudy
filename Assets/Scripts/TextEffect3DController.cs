using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextEffect3DController : MonoBehaviour
{
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
    public void PlayEffect()
    {
        transform.SetParent(null);

        Vector3 startPosition = transform.position;

        Vector3 targetPosition = startPosition + new Vector3(0, 2, 0);

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * 1.5f;

        Color startColor = textMesh.color;
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad)); // Yukarı doğru hareket
        sequence.Join(transform.DOScale(targetScale, 1f).SetEase(Ease.OutQuad)); // Boyut büyütme
        sequence.Append(DOTween.ToAlpha(() => textMesh.color, x => textMesh.color = x, 0f, 0.5f)); // Fade-out

        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
