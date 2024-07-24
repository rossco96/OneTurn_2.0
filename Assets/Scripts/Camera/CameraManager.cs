using UnityEngine;

public class CameraManager : MonoBehaviour
{
	private const float k_cameraPosZ = -10.0f;

	private static Camera m_camera = null;

	private void Awake()
	{
		// [TODO][Q][IMPORTANT]
		// MUST BE A BETTER WAY TO DO THIS? WANT TO DO THIS HERE?
		//SetAllCanvasScalers();

		if (m_camera == null)
		{
			DontDestroyOnLoad(gameObject);
			m_camera = GetComponent<Camera>();
			SetupCameraPosition();
		}
		else if (m_camera != GetComponent<Camera>())
		{
			Destroy(gameObject);
			return;
		}

		// [TODO][Q][IMPORTANT]
		// MUST BE A BETTER WAY TO DO THIS? WANT TO DO THIS HERE?
		// ... Or rename script to something like 'UISetup'
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += SetAllCanvasScalers;
	}



	private void SetAllCanvasScalers(UnityEngine.SceneManagement.Scene s, UnityEngine.SceneManagement.LoadSceneMode lsm)
	{
		// DELETE! TESTING ON LAPTOP ONLY!
#if UNITY_EDITOR
		UnityEngine.UI.CanvasScaler[] canvasScalersTEST = FindObjectsOfType<UnityEngine.UI.CanvasScaler>(false);                // [TODO] Set to true! Only false for testing!
		for (int i = 0; i < canvasScalersTEST.Length; ++i)
		{
			canvasScalersTEST[i].referenceResolution = new Vector2(1080, 2160);
			for (int j = 0; j < canvasScalersTEST[i].transform.childCount; ++j)
			{
				SetCanvasToSafeArea(canvasScalersTEST[i].transform.GetChild(j).GetComponent<RectTransform>());
			}
		}
		return;
#endif
		// ^^^ DELETE ^^^

		UnityEngine.UI.CanvasScaler[] canvasScalers = FindObjectsOfType<UnityEngine.UI.CanvasScaler>(false);				// [TODO] Set to true! Only false for testing!
		for (int i = 0; i < canvasScalers.Length; ++i)
		{
			canvasScalers[i].referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
			for (int j = 0; j < canvasScalers[i].transform.childCount; ++j)
			{
				SetCanvasToSafeArea(canvasScalers[i].transform.GetChild(j).GetComponent<RectTransform>());
			}
		}
	}

	private void SetCanvasToSafeArea(RectTransform rectTransform)
	{
		Rect safeArea = Screen.safeArea;
		Vector2 minAnchor = safeArea.position;
		Vector2 maxAnchor = minAnchor + safeArea.size;

		minAnchor.x /= Screen.width;
		minAnchor.y /= Screen.height;
		maxAnchor.x /= Screen.width;
		maxAnchor.y /= Screen.height;

		rectTransform.anchorMin = minAnchor;
		rectTransform.anchorMax = maxAnchor;
	}



	private void SetupCameraPosition()
	{
		float posX = m_camera.orthographicSize * m_camera.aspect;
		float posY = m_camera.orthographicSize * ((2.0f * m_camera.aspect) - 1.0f);
		transform.position = new Vector3(posX, posY, k_cameraPosZ);
	}
}
