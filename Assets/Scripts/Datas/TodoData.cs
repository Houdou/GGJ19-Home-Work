using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Todo", menuName = "HomeWork/Todo")]
public class TodoData : ScriptableObject {
    public string Name;
    public GameTime Time;
    
    public bool IsExpirable;
    public GameTime ExpiryTime;

    public EventData FulFillEvent;
    public EventData FailedEvent;
}
