using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteController : MonoBehaviour {
	public bool CanFade = true;
	protected Image Image;
	protected Color OriginalImageColor;
	protected Color TargetImageColor;
	protected Color FadeOutImageColor;

	protected virtual void Awake() {
		Image = GetComponent<Image>();

		TargetImageColor = OriginalImageColor = Image.color;
		FadeOutImageColor = new Color(OriginalImageColor.r, OriginalImageColor.g, OriginalImageColor.b, 0f);
	}

	protected virtual void Update() {
		if (CanFade) {
			HandleFading();
		}
	}

	protected virtual void HandleFading() {
		if (Image.color != TargetImageColor) {
			Image.Fade(TargetImageColor);
		}
	}

	public virtual void FadeIn() {
		TargetImageColor = OriginalImageColor;
	}

	public virtual void FadeOut() {
		TargetImageColor = FadeOutImageColor;
	}
}
