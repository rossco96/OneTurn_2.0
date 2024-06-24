using UnityEngine;

public class TEST_DEBUG : MonoBehaviour
{
	public static TEST_DEBUG Instance;

	[SerializeField] private TMPro.TextMeshProUGUI m_debugText;

	private void Awake()
	{
		Instance = this;
		/*
		m_debugText.text = $"" +
			$"{Screen.width} / {Screen.height}\n" +
			$"{Screen.currentResolution.width} / {Screen.currentResolution.height}\n" +
			$"{Screen.cutouts.Length} / {Screen.dpi}\n" +
			$"{Screen.safeArea.min} / {Screen.safeArea.max}\n" +
			$"{Camera.main.pixelWidth} / {Camera.main.pixelHeight}\n" +
			$"{Camera.main.scaledPixelWidth} / {Camera.main.scaledPixelHeight}\n" +
			$"{Camera.main.aspect} / {Camera.main.orthographicSize}\n" +
			$"{Camera.main.pixelRect.min} / {Camera.main.pixelRect.max}\n" +
			$"{Camera.main.rect.min} / {Camera.main.rect.max}" +
			$"";
		//*/

		m_debugText.text = $"";
	}

	public void SetText(string text)
	{
		m_debugText.text = text;
	}
}
