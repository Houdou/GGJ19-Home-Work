using UnityEngine;

[CreateAssetMenu(fileName = "New Int Status Trigger", menuName = "HomeWork/StatusTrigger/Int")]
public class IntStatusTriggerData : StatusTriggerData {
	public int TargetIntValue;

	public bool Test(int value) {
		switch (ConditionOperator) {
			case ConditionOperator.Equal:
				return value == TargetIntValue;
			case ConditionOperator.NotEqual:
				return value != TargetIntValue;
			case ConditionOperator.Greater:
				return value > TargetIntValue;
			case ConditionOperator.GreaterOrEqual:
				return value >= TargetIntValue;
			case ConditionOperator.Less:
				return value < TargetIntValue;
			case ConditionOperator.LessOrEqual:
				return value <= TargetIntValue;
		}

		return false;
	}
}