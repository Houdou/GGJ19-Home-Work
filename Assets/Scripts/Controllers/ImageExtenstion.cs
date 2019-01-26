using UnityEngine;
using UnityEngine.UI;

public static class ImageExtenstion {
	public static void Fade(this Image img, Color targetColor, float smoothingFactor = 0.8f) {
		img.color = Color.Lerp(img.color, targetColor, Time.deltaTime * smoothingFactor);
	}
}