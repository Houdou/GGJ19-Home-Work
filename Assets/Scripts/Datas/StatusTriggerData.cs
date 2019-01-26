using UnityEngine;

public interface IStatusTrigger<T> {
	bool Compare(T obj);
}

public abstract class StatusTriggerData : ScriptableObject {
	public string Name;
	public StatusFields Field;
	public ConditionOperator ConditionOperator;
	public EventData[] TriggerEvents;
}