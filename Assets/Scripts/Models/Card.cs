using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : BaseEvent {
	public override void ProgressInTime(GameTime hour) {
		base.ProgressInTime(hour);
	}

	public event Action<GameTime> OnFinished;

	public void Finish() {
		OnFinished?.Invoke(CurrentTime);
	}
}