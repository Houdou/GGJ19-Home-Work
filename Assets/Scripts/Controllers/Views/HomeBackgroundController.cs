public class HomeBackgroundController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var homeLevel = statusManager.HomeLevel;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		var attr = location == LocationType.Home ? time.IsNight ? "Night" : "Day" : "Fade";

		return
			$"Home-{homeLevel}-Background-{attr}";
	}
}