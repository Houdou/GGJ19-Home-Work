using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {
	public event Action OnClick;

	public Transform CenterPos;
	public Transform RefPos;

	private void Start() {
		
	}

	private void Update() {
	}
	
	public void HandleClick() {
		OnClick?.Invoke();
	}
	
}