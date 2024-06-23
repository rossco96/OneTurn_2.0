using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_DEBUG : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI m_debugText;

	private void Awake()
	{
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
	}
}
