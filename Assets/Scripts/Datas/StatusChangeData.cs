using UnityEngine;

[CreateAssetMenu(fileName = "New Status Change", menuName = "HomeWork/StatusChanges")]
public class StatusChangeData : ScriptableObject {
	public string Name = "StatusChange";
	
	public GameTime Time;
	public bool OverrideTime;

	public LocationType Location;
	
	public int Money;
	public bool OverrideMoney;
	
	public int Energy;
	public bool OverrideEnergy;
	
	public int PersonalHappiness;
	public bool OverridePersonalHappiness;
	
	public int FamilyHappiness;
	public bool OverrideFamilyHappiness;
	
	public int Career;
	public bool OverrideCareer;
	
	public int ProjectProgress;
	public bool OverrideProjectProgress;
}