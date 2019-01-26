using System;

public class Todo : BaseEvent {
	public bool IsInternal;
	public bool IsExpirable;
	public GameTime ExpiryTimestamp;

	public bool IsExpired => IsExpirable && CurrentTime >= ExpiryTimestamp;
	public GameTime RemainingTime => ExpiryTimestamp - CurrentTime;
	
	public Todo(bool isExpirable, bool isInternal, GameTime expiryTimestamp) {
		IsExpirable = isExpirable;
		IsInternal = isInternal;
		ExpiryTimestamp = expiryTimestamp;

		if (ExpiryTimestamp <= CreatedAt) {
			OnExpire?.Invoke();
		}
	}

	public event Action OnExpire;

	public override void ProgressInTime(GameTime hour) {
		base.ProgressInTime(hour);

		if (IsExpired) {
			OnExpire?.Invoke();
		}
	}
}