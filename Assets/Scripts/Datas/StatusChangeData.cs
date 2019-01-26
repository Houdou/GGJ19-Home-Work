using UnityEngine;

[CreateAssetMenu(fileName = "New Status Change", menuName = "HomeWork/StatusChanges")]
public class StatusChangeData : ScriptableObject {
	public string Name;
	public GameTime Time;

	public LocationType Location;
	public int Money;
	public int Energy;
	public int PersonalHappiness;
	public int FamilyHappiness;
	public int Career;
	public int ProjectProgress;
}