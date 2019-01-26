using TMPro;
using UnityEngine;

public class ConsequenceController : SpriteController {
	protected TextMeshProUGUI Text;
	protected Color OriginalTextColor;
	protected Color TargetTextColor;
	protected Color FadeOutTextColor;

	protected override void Awake() {
		base.Awake();
		Text = GetComponentInChildren<TextMeshProUGUI>();
		
		TargetTextColor = OriginalTextColor = Text.color;
		FadeOutTextColor = new Color(OriginalTextColor.r, OriginalTextColor.g, OriginalTextColor.b, 0f);

		SetFadeOut();
	}

	protected override void HandleFading() {
		base.HandleFading();
		if (Text.color != TargetTextColor) {
			Text.Fade(TargetTextColor);
		}
	}

	public override void FadeIn() {
		base.FadeIn();
		TargetTextColor = OriginalTextColor;
	}

	public override void FadeOut() {
		base.FadeOut();
		TargetTextColor = FadeOutTextColor;
	}

	public void SetFadeOut() {
		Image.color = FadeOutTextColor;
		Text.color = FadeOutImageColor;
		FadeOut();
	}
}
