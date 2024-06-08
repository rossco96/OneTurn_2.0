using UnityEngine;

public class CameraManager : MonoBehaviour
{
	private const float k_cameraPosZ = -10.0f;

	private static Camera m_camera = null;

	private void Awake()
	{
		if (m_camera == null)
		{
			DontDestroyOnLoad(gameObject);
			m_camera = GetComponent<Camera>();
			SetupCameraPosition();
		}
		else if (m_camera != GetComponent<Camera>())
		{
			Destroy(gameObject);
		}
	}

	private void SetupCameraPosition()
	{
		float posX = m_camera.orthographicSize * m_camera.aspect;
		float posY = m_camera.orthographicSize * ((2.0f * m_camera.aspect) - 1.0f);
		transform.position = new Vector3(posX, posY, k_cameraPosZ);
	}
}
