using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class FadeExtenstion {
	public static void Fade(this Image img, Color targetColor, float smoothingFactor = 7f) {
		img.color = Color.Lerp(img.color, targetColor, Time.deltaTime * smoothingFactor);
	}
	
	public static void Fade(this TextMeshProUGUI text, Color targetColor, float smoothingFactor = 7f) {
		text.color = Color.Lerp(text.color, targetColor, Time.deltaTime * smoothingFactor);
	}
}