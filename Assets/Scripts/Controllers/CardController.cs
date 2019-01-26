using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : SpriteController {
    public event Action OnClick;
    public Vector3 PinPos;
    public Vector3 DetailOffset;

    public GameObject DetailsPrefab;
    private GameObject _detail;

    protected override void Awake() {
        base.Awake();
        CreateDetails();
    }

    public void HandleClick() {
        OnClick?.Invoke();
    }

    private void CreateDetails() {
        _detail = Instantiate(DetailsPrefab, transform.parent, false);
        _detail.transform.localPosition = DetailOffset;
    }

    public void MouseOver() {
        if (_detail == null) {
            CreateDetails();
        }

        _detail.transform.localPosition = DetailOffset;
        _detail.GetComponent<SpriteController>().FadeIn();
    }

    private void MouseExit() {
        _detail.GetComponent<SpriteController>().FadeOut();
    }
}