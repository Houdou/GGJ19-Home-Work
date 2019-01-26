public class OfficeManController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var time = statusManager.CurrentTime;
		var location = statusManager.Location;
		if (location != LocationType.Office) {
			return "PlaceHolder";
		}
		
		return $"Office-Man-{(time.IsNight ? "Night" : "Day")}{(statusManager.Sick ? "-Sick" : "")}";
	}
}