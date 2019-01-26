using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Todo", menuName = "HomeWork/TodoCard")]
public class TodoCardData : ScriptableObject {
    public string Name;
    public LocationType Location;
    public StatusChangeData Cost;
    
    public bool IsExpirable;
    public GameTime ExpiryTime;

    public EventData[] FulFillEvent;
    public EventData[] FailedEvent;
}
