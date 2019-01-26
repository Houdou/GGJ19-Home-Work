using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {
	public event Action OnClick;
	
	void Start() { }

	void Update() { }

	public void HandleClick() {
		OnClick?.Invoke();
	}
}