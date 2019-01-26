using UnityEngine;

[CreateAssetMenu(fileName = "New Status Change", menuName = "HomeWork/StatusChanges")]
public class StatusChangeData : ScriptableObject {
	public string Name;
	public GameTime Time;

	public int Energy;
	public int Money;
	public int Happiness;
	
	// TODO: Other fields 
}