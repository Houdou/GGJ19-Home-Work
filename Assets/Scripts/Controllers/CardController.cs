using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {
	public event Action OnClick;

	public Transform CenterPos;
	public Transform RefPos;
	public Vector3 PinPos;
	
	
	public void HandleClick() {
		OnClick?.Invoke();
	}
	
}