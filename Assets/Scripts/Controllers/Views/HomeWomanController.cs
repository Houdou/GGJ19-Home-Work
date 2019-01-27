public class HomeWomanController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		if (location != LocationType.Home) {
			return "PlaceHolder";
		}
		var attr = statusManager.Normal ? "" : statusManager.LieDown && location == LocationType.Home ? "-Lie" : "-Sick";

		return $"Home-Woman-{(time.IsNight ? "Night" : "Day")}{attr}";
	}
}