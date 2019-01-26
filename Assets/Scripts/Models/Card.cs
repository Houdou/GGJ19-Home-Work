using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Card : BaseEvent {
	public override void ProgressInTime(GameTime hour) {
		base.ProgressInTime(hour);
	}

	public event Action<GameTime> OnFinished;

	public void Finish() {
		OnFinished?.Invoke(CurrentTime);
	}
}

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