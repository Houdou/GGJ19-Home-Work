using UnityEngine;

[CreateAssetMenu(fileName = "New Delayed Generate Cards Event", menuName = "HomeWork/GenerateDelayedCards")]
public class GenerateDelayedCardsData : ScriptableObject {
	public string Name = "DelayedGenerateCards";
	public GameTime Delay;
	public GenerateCardsData GenerateData;
	public bool IsInnate;
	public bool Repeat;
	public GameTime DeltaTime;
}
