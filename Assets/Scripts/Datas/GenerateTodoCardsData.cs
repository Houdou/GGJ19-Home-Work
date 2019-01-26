using UnityEngine;

[CreateAssetMenu(fileName = "New Generate Todo Card Event", menuName = "HomeWork/GenerateTodoCard")]
public class GenerateTodoCardsData : ScriptableObject {
	public string Name;
	public GameTime Time;
	public string[] TodoCardsToGenerate;
	public bool IsInnate;
	public bool Repeat;
	public GameTime DeltaTime;
}
