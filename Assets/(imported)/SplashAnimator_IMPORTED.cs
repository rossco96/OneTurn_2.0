using UnityEngine;
using UnityEngine.UI;

public class SplashAnimator_IMPORTED : MonoBehaviour {

	[SerializeField] private Image fader;

	private float totalAlphaReduction;
	private int timesAlphaReachedTarget;


	void Start ()
	{
		timesAlphaReachedTarget = 0;
		totalAlphaReduction = 0;
		fader.color = new Vector4(1, 1, 0, 1);
	}

	void Update ()
	{
		if (timesAlphaReachedTarget == 0 && fader.color.a > 0.5)
		{
			totalAlphaReduction += Time.deltaTime*0.3f;
			fader.color = new Vector4(1, (1-2*totalAlphaReduction), 0, (1-totalAlphaReduction));
		}

		if (timesAlphaReachedTarget == 0 && fader.color.a <= 0.5)
		{
			fader.color = new Vector4(1, 0, 0, 0.5f);
			totalAlphaReduction = 0;
			timesAlphaReachedTarget = 1;
		}

		if (timesAlphaReachedTarget == 1 && fader.color.a < 0.9)
		{
			totalAlphaReduction += Time.deltaTime*0.4f;
			fader.color = new Vector4(1, 0, 0, (0.5f+totalAlphaReduction));
		}

		if (timesAlphaReachedTarget == 1 && fader.color.a >= 0.9)
		{
			fader.color = new Vector4(1, 0, 0, 0.9f);
			totalAlphaReduction = 0;
			timesAlphaReachedTarget = 2;
		}

		if (timesAlphaReachedTarget == 2 && fader.color.a > 0)
		{
			totalAlphaReduction += Time.deltaTime*0.5f;
			fader.color = new Vector4(1, 0, 0, (0.9f-totalAlphaReduction));
		}
	}
}
