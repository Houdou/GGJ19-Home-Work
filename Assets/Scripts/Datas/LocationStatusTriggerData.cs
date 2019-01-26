public class LocationStatusTriggerData : StatusTriggerData {
	public LocationType TargetIntValue;

	public bool Test(LocationType value) {
		switch (ConditionOperator) {
			case ConditionOperator.Equal:
				return value == TargetIntValue;
			case ConditionOperator.NotEqual:
				return value != TargetIntValue;
		}

		return false;
	}
}