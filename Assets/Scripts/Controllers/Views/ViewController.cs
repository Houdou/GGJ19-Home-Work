using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour {
	public CrossFadeSpriteController HomeBackground;
	public CrossFadeSpriteController HomeForeground;
	public CrossFadeSpriteController HomeMan;
	public CrossFadeSpriteController HomeWoman;
	public CrossFadeSpriteController OfficeSky;
	public CrossFadeSpriteController OfficeBackground;
	public CrossFadeSpriteController OfficeForeground;
	public CrossFadeSpriteController OfficeMan;

	public void Init() {
		HomeBackground.Init("Home-1-Background-Day");
		HomeForeground.Init("PlaceHolder");
		HomeMan.Init("Home-Man-Day");
		HomeWoman.Init("Home-Woman-Day");
		
		OfficeBackground.Init("Office-1-Background-Day");
		OfficeForeground.Init("Office-1-Foreground-Fade");
		OfficeMan.Init("PlaceHolder");
	}

	void Update() {
		
	}

	public void UpdateUI() {
		HomeBackground.CheckStatus();
		HomeForeground.CheckStatus();
		HomeMan.CheckStatus();
		HomeWoman.CheckStatus();
		OfficeBackground.CheckStatus();
		OfficeForeground.CheckStatus();
		OfficeMan.CheckStatus();
	}
	
}
