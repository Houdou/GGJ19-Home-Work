public class OfficeBackgroundController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var officeLevel = statusManager.OfficeLevel;
		var time = statusManager.CurrentTime;

		return $"Office-{officeLevel}-Background-{(time.IsNight ? "Night" : "Day")}";
	}
}