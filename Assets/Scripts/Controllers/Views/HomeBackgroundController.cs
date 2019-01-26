public class HomeBackgroundController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var homeLevel = statusManager.HomeLevel;
		var time = statusManager.CurrentTime;

		return
			$"Home-{homeLevel}-Background-{(time.IsNight ? "Night" : "Day")}";
	}
}