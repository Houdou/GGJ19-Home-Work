public class BaseEvent {
	
	/// <summary>
	/// TotalHour in game
	/// </summary>
	public GameTime CreatedAt;

	/// <summary>
	/// TotalHour in game
	/// </summary>
	public GameTime CurrentTime;
	
	public virtual void ProgressInTime(GameTime hour) {
		CurrentTime += hour;
	}
}