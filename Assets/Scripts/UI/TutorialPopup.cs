using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
	[SerializeField] private TutorialData m_tutorialData;
	
	[Space]
	[SerializeField] private SettingsDataBool m_masterTutorialsSettingsData;
	[SerializeField] private SettingsDataBool m_thisTutorialSettingsData;

	[Space]
	//[SerializeField] private GameObject m_popupParent;
	[SerializeField] private TextMeshProUGUI m_title;
	[SerializeField] private TextMeshProUGUI m_text;
	[SerializeField] private Image m_image;
	[SerializeField] private GameObject m_nextButton;
	[SerializeField] private GameObject m_closeButton;

	// [TODO] Implement these? If so, consider either index or have bool in TutorialData m_enableHighlight and m_enableArrow?
	//[Space]
	//[SerializeField] private GameObject m_highlight;
	//[SerializeField] private GameObject m_arrow;

	private int m_index = 0;


	public bool TryShow()
	{
		if (SettingsSystem.GetValue(m_masterTutorialsSettingsData.Key) == $"{false}" || SettingsSystem.GetValue(m_thisTutorialSettingsData.Key) == $"{false}")
			return false;
		Init();
		SetImageText();
		if (m_tutorialData.Content.Length == 1)
			ShowCloseButton();
		m_title.text = m_tutorialData.Title;
		gameObject.SetActive(true);
		return true;
	}


	public void OnNext()
	{
		// [Q] Need minimum time before continuing?
		m_index++;
		SetImageText();
		if (m_index == m_tutorialData.Content.Length - 1)
			ShowCloseButton();
	}

	// [TODO] DELETE and just use the inspector for this
	//public void OnClose()
	//{
	//	m_popupParent.SetActive(false);
	//}

	public void OnDontShow()
	{
		SettingsSystem.UpdateSettings(m_thisTutorialSettingsData.Key, $"{false}");
	}

	public void OnDisableTutorials()
	{
		SettingsSystem.UpdateSettings(m_masterTutorialsSettingsData.Key, $"{false}");
	}


	private void Init()
	{
		m_index = 0;
		m_nextButton.SetActive(true);
		m_closeButton.SetActive(false);
	}

	// [TODO] Assign in the inspector!																<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
	private void SetImageText()
	{
		m_image.sprite = m_tutorialData.Content[m_index].Image;
		m_text.text = m_tutorialData.Content[m_index].Text;
	}

	private void ShowCloseButton()
	{
		m_nextButton.SetActive(false);
		m_closeButton.SetActive(true);
	}
}
