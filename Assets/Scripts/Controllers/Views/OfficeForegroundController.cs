using UnityEngine;

public class OfficeForegroundController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var officeLevel = statusManager.OfficeLevel;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		var attr = location == LocationType.Office ? time.IsNight ? "Night" : "Day" : "Fade";

		return $"Office-{officeLevel}-Foreground-{attr}";
	}
}