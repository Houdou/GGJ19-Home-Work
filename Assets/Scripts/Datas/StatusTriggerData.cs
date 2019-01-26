using UnityEngine;

[CreateAssetMenu(fileName = "New Status Trigger", menuName = "HomeWork/StatusTrigger")]

public class StatusTriggerData : ScriptableObject {
	public StatusFields Field;
	public ConditionOperator ConditionOperator;
	public int TargetIntValue;
	public float TargetFloatValue;
	public string TargetStringValue;
}