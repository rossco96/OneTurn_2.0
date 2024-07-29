using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData_", menuName = "Data/Tutorial")]
public class TutorialData : ScriptableObject
{
	public string Title;
	public ImageTextPair[] Content;
}

[System.Serializable]
public struct ImageTextPair
{
	public Sprite Image;
	public string Text;
}
