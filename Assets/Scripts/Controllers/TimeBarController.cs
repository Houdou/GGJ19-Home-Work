using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBarController : MonoBehaviour {
    private Image _fill;
    
    public float TargetAmount;

    private void Awake() {
        _fill = GetComponent<Image>();
    }

    void Update() {
        if (Math.Abs(_fill.fillAmount - TargetAmount) > 0.001f) {
            _fill.fillAmount = Mathf.Lerp(_fill.fillAmount, TargetAmount, 5f * Time.deltaTime);
        }
    }

    public void SetAmount(float amount) {
        _fill.fillAmount = amount;
    }
}