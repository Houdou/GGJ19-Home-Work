using UnityEngine;

public interface IStatusTrigger<T> {
	bool Compare(T obj);
}

public abstract class StatusTriggerData : ScriptableObject {
	public string Name = "StatusTrigger";
	public StatusFields Field;
	public ConditionOperator ConditionOperator;
	public bool IsInnate;
	public bool Repeat;
	public GameTime DeltaTime;
	public EventData[] TriggerEvents;
}