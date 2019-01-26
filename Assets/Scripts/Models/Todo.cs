using System;

public class Todo : BaseEvent {
	public bool IsExpirable;
	public GameTime ExpireTime;

	public bool IsExpired => IsExpirable && CurrentTime >= ExpireTime;
	
	public Todo(bool isExpirable) {
		IsExpirable = isExpirable;
	}

	public event Action OnExpire;

	public override void ProgressInTime(GameTime hour) {
		base.ProgressInTime(hour);

		if (IsExpired) {
			OnExpire?.Invoke();
		}
	}
}