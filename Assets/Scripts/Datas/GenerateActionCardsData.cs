using UnityEngine;

[CreateAssetMenu(fileName = "New Generate Action Card Event", menuName = "HomeWork/GenerateActionCard")]
public class GenerateActionCardsData : ScriptableObject {
	public string Name;
	public GameTime Time;
	public string[] ActionCardsToGenerate;
	public bool IsInnate;
	public bool Repeat;
	public GameTime DeltaTime;
}