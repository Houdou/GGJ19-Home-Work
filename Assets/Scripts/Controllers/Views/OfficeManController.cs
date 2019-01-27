public class OfficeManController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		if (location != LocationType.Office) {
			return "PlaceHolder";
		}
		var attr = statusManager.Normal ? "" : statusManager.LieDown && location == LocationType.Home ? "-Lie" : "-Sick";

		return $"Office-Man-{(time.IsNight ? "Night" : "Day")}{attr}";
	}
}