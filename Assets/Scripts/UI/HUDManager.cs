using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	[SerializeField] private GameObject m_statsParent;
	[SerializeField] private GameObject m_statsParentP2;					// [TODO][Q] Is this the best way to do this?
	[SerializeField] private TextMeshProUGUI m_timerText;
	[SerializeField] private Slider m_timerSlider;
	[SerializeField] private TextMeshProUGUI m_livesCount;
	[SerializeField] private TextMeshProUGUI m_itemsCount;
	
	[Space]
	[SerializeField] private GameObject m_pauseMenu;
	[SerializeField] private Button m_pauseButton;
	[SerializeField] private Button m_resumeButton;							// [TODO][Q] Do we want this in its own PauseManager?



	private void Awake()
	{
		// [TODO][IMPORTANT] Must calculate what the y-position is! Using aspectRatio, etc.
		m_statsParent.transform.localPosition = new Vector3(0, -140, 0);
	}



	public void SetTimerSliderActive(bool active)
	{
		m_timerSlider.gameObject.SetActive(active);
	}

	public void SetItemsCountActive(bool active)
	{
		m_itemsCount.gameObject.SetActive(active);
	}



	public void UpdateLivesCount(int lives)
	{
		m_livesCount.text = $"Lives: {lives}";
	}

	public void UpdateItemsCount(int items, int totalItems)
	{
		m_itemsCount.text = $"Items: {items}/{totalItems}";
	}

	public void UpdateTimerTextExit(int timeTaken)
	{
		m_timerText.text = $"Time Taken: {timeTaken}s";
	}

	public void UpdateTimerTextItems(float timeLeft)
	{
		m_timerText.text = $"Time Left: {timeLeft}s";
	}

	public void UpdateTimerSlider(float timeLeft, int timeLimit)
	{
		m_timerSlider.value = 1 - (timeLeft / timeLimit);
	}



	public void AssignPauseButton(UnityAction onPause)
	{
		m_pauseButton.onClick.AddListener(onPause);
	}

	// [TODO][Q] Do we want this in its own PauseManager?
	public void AssignResumeButton(UnityAction onResume)
	{
		m_resumeButton.onClick.AddListener(onResume);
	}



	public void SetPauseMenuActive(bool active)
	{
		m_pauseMenu.SetActive(active);
	}
}
