using UnityEngine;

public class CameraManager : MonoBehaviour
{
	private const float k_cameraPosZ = -10.0f;

	private static Camera m_camera = null;

	private void Awake()
	{
		// [TODO][Q][IMPORTANT]
		// IS THERE A BETTER WAY TO DO THIS? WANT TO DO THIS HERE?
		SetAllCanvasScalers();

		if (m_camera == null)
		{
			DontDestroyOnLoad(gameObject);
			m_camera = GetComponent<Camera>();
			SetupCameraPosition();
		}
		else if (m_camera != GetComponent<Camera>())
		{
			Destroy(gameObject);
			return;												// RETURN TEMP HERE BECAUSE TEST CODE BELOW
		}

		// [TODO] TEMP HERE! PUT INTO GameStartup or SplashScreen
		SaveSystem.Init();
	}

	private void SetAllCanvasScalers()
	{
		UnityEngine.UI.CanvasScaler[] canvasScalers = FindObjectsOfType<UnityEngine.UI.CanvasScaler>(true);
		for (int i = 0; i < canvasScalers.Length; ++i)
		{
			canvasScalers[i].referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		}
	}

	private void SetupCameraPosition()
	{
		float posX = m_camera.orthographicSize * m_camera.aspect;
		float posY = m_camera.orthographicSize * ((2.0f * m_camera.aspect) - 1.0f);
		transform.position = new Vector3(posX, posY, k_cameraPosZ);
	}
}
