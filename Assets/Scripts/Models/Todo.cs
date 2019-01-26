using System;

public class Todo : BaseEvent {
	public bool IsInternal;
	public bool IsExpirable;
	public GameTime ExpireTime;

	public bool IsExpired => IsExpirable && CurrentTime >= ExpireTime;
	public GameTime RemainingTime => ExpireTime - CurrentTime;
	
	public Todo(bool isExpirable, bool isInternal) {
		IsExpirable = isExpirable;
		IsInternal = isInternal;
	}

	public event Action OnExpire;

	public override void ProgressInTime(GameTime hour) {
		base.ProgressInTime(hour);

		if (IsExpired) {
			OnExpire?.Invoke();
		}
	}
}