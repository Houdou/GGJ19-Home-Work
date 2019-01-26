using UnityEngine;

[CreateAssetMenu(fileName = "New GameTime Status Trigger", menuName = "HomeWork/StatusTrigger/GameTime")]
public class GameTimeStatusTriggerData : StatusTriggerData {
	public GameTime TargetGameTime;

	public bool Test(GameTime value) {
		switch (ConditionOperator) {
			case ConditionOperator.Equal:
				return value == TargetGameTime;
			case ConditionOperator.NotEqual:
				return value != TargetGameTime;
			case ConditionOperator.Greater:
				return value > TargetGameTime;
			case ConditionOperator.GreaterOrEqual:
				return value >= TargetGameTime;
			case ConditionOperator.Less:
				return value < TargetGameTime;
			case ConditionOperator.LessOrEqual:
				return value <= TargetGameTime;
		}

		return false;
	}
}