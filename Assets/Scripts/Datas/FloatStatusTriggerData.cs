using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Float Status Trigger", menuName = "HomeWork/StatusTrigger/Float")]
public class FloatStatusTriggerData : StatusTriggerData {
	public float TargetFloatValue;

	public bool Test(float value) {
		switch (ConditionOperator) {
			case ConditionOperator.Equal:
				return Math.Abs(value - TargetFloatValue) < 0.0001f;
			case ConditionOperator.NotEqual:
				return Math.Abs(value - TargetFloatValue) > 0.0001f;
			case ConditionOperator.Greater:
				return value > TargetFloatValue;
			case ConditionOperator.GreaterOrEqual:
				return value >= TargetFloatValue;
			case ConditionOperator.Less:
				return value < TargetFloatValue;
			case ConditionOperator.LessOrEqual:
				return value <= TargetFloatValue;
		}

		return false;
	}
}