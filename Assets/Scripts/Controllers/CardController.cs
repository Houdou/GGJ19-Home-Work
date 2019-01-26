using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardController : MonoBehaviour {
    public event Action OnClick;

    public Vector3 PinPos;
    public Vector3 Offset;

    public GameObject ConsequencePrefab;
    private GameObject _consequence;

    private Image _image;

    private Color _fadeOutColor;
    public Color OriginalColor;

    private Color _targetColor;

    public bool IsEmergency;

    void Awake() {
        _image = GetComponent<Image>();

        _targetColor = OriginalColor = _image.color;
        _fadeOutColor = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0f);

        _consequence = Instantiate(ConsequencePrefab, PinPos + Offset, Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update() {
        HandleFading();
    }
    
    private void HandleFading() {
        if (_image.color == _targetColor) return;
        _image.Fade(_targetColor);
    }

    public void HandleClick() {
        OnClick?.Invoke();
    }

    public void MouseOver() {
        if (_consequence == null) {
            _consequence = Instantiate(ConsequencePrefab, PinPos + Offset, Quaternion.identity, transform);
        }

        _consequence.transform.localPosition = PinPos + Offset;
        FadeIn();
    }

    public void MouseExit() {
        FadeOut();
    }

    public void FadeIn() {
        _targetColor = OriginalColor;
    }

    public void FadeOut() {
        _targetColor = _fadeOutColor;
    }
}