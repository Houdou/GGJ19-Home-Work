public class HomeManController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		if (location != LocationType.Home) {
			return "PlaceHolder";
		}
		
		return $"Home-Man-{(time.IsNight ? "Night" : "Day")}{(statusManager.Sick ? "-Sick" : "")}";
	}
}