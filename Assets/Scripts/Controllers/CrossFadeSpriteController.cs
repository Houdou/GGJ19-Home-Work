using UnityEngine;
using UnityEngine.UI;

public class CrossFadeSpriteController : MonoBehaviour {
	public Image Current;
	public Image Next;

	private void Awake() {
		Current = transform.Find("Current").GetComponent<Image>();
		Next = transform.Find("Next").GetComponent<Image>();
	}

	public void Init(string resource) {
		Current.sprite = GameMaster.Instance.GetSpriteResources(resource);
		Next.color = Config.SpriteFadeOutColor;
		Next.sprite = GameMaster.Instance.GetSpriteResources(resource);
	}

	void Update() {
		if(Fading) {
			Next.FadeSlow(Config.SpriteFadeInColor);
			if (Next.color.ColorDistance(Config.SpriteFadeInColor) <= 0.01f) {
				Fading = false;
				Current.sprite = Next.sprite;
				Next.color = Config.SpriteFadeOutColor;
			}
		}
	}

	protected bool Fading;
	protected string PreviousId;

	public void CheckStatus() {
		var id = GetResourceId();
		if (id == PreviousId) return;

		if (Fading) {
			Current.sprite = Next.sprite;
			Next.color = Config.SpriteFadeOutColor;
		}

		// Fading
		Next.sprite = GameMaster.Instance.GetSpriteResources(id);
		PreviousId = id;
		Fading = true;
	}

	protected virtual string GetResourceId() {
		return "";
	}
}