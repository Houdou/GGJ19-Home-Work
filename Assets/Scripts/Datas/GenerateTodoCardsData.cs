using UnityEngine;

[CreateAssetMenu(fileName = "New Generate Todo Card Event", menuName = "HomeWork/GenerateTodoCard")]
public class GenerateTodoCardsData : ScriptableObject {
	public string Name;
	
	public string[] TodoCardsToGenerate;
}