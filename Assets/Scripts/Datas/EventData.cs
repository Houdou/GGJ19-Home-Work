using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "HomeWork/Event")]
public class EventData : ScriptableObject {
    public string Name;

    public StatusChangeData[] StatusChanges;
    public GameEnding Ending;
}
