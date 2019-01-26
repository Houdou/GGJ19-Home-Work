public class HomeForegroundController : CrossFadeSpriteController {
	protected override string GetResourceId() {
		var statusManager = StatusManager.Instance;
		var homeLevel = statusManager.HomeLevel;
		var location = statusManager.Location;
		
		return location == LocationType.Home ? "PlaceHolder" : $"Home-{homeLevel}-Foreground-Fade";
	}
}