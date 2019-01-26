using UnityEngine;

[CreateAssetMenu(fileName = "New Generate Action Card Event", menuName = "HomeWork/GenerateActionCard")]
public class GenerateActionCardsData : ScriptableObject {
	public string Name;
	
	public string[] ActionCardsToGenerate;
}