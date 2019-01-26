public class OfficeForegroundController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var officeLevel = statusManager.OfficeLevel;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		if (location != LocationType.Office) {
			return "PlaceHolder";
		}

		return $"Office-{officeLevel}-Foreground-{(time.IsNight ? "Night" : "Day")}";
	}
}