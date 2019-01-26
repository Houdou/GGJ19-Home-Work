using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour {
	public CrossFadeSpriteController HomeBackground;
	public CrossFadeSpriteController HomeMan;
	public CrossFadeSpriteController HomeWoman;
	public CrossFadeSpriteController OfficeSky;
	public CrossFadeSpriteController OfficeBackground;
	public CrossFadeSpriteController OfficeForeground;
	public CrossFadeSpriteController OfficeMan;

	public void Init() {
		HomeBackground.Init("Home-1-Background-Day");
		HomeMan.Init("Home-Man-Day");
		HomeWoman.Init("Home-Woman-Day");
		
		OfficeBackground.Init("Office-1-Background-Fade");
		OfficeForeground.Init("PlaceHolder");
		OfficeMan.Init("PlaceHolder");
	}

	void Update() {
		
	}

	public void UpdateUI() {
		HomeBackground.CheckStatus();
		HomeMan.CheckStatus();
		HomeWoman.CheckStatus();
	}
	
}
