using UnityEngine;

[CreateAssetMenu(fileName = "New Generate Cards Event", menuName = "HomeWork/GenerateCards")]
public class GenerateCardsData : ScriptableObject {
	public string Name = "GenerateCards";
	public ActionCardData[] ActionCardsToGenerate;
	public TodoCardData[] TodoCardsToGenerate;
	
	public bool IsEmergency;
}