public class OfficeBackgroundController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var officeLevel = statusManager.OfficeLevel;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		var attr = location == LocationType.Home ? time.IsNight ? "Night" : "Day" : "Fade";

		return $"Office-{officeLevel}-Background-{attr}";
	}
}