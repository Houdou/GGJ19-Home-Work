using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "HomeWork/Event")]
public class EventData : ScriptableObject {
    public string Name = "Event";

    public GenerateCardsData[] CardsGenerations;
    public GenerateDelayedCardsData[] DelayedCardsGenerations;
    
    
    public StatusChangeData[] StatusChanges;
    public GameEnding Ending;
}
