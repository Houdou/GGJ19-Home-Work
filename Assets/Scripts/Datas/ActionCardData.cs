using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Card", menuName = "HomeWork/ActionCard")]
public class ActionCardData : ScriptableObject {
	public string Name;
	public LocationType Location;

    public bool isEmergency;
	
	public GameTime Time;
	public int Energy;
	public int Money;

	public StatusChangeData Cost;

	public EventData[] TriggerEvents;
}
