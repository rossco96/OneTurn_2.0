using UnityEngine;

public class ExternalLinksManager : MonoBehaviour
{
	private const string k_surveyURL = "https://docs.google.com/forms/d/e/1FAIpQLSdfdOzKYjbNNJ7Dq9t95HHtY4oDtm24GZyPgwDhJEysx6YXcQ/viewform?usp=sf_link";
	private const string k_websiteURL = "https://rosscoles.wixsite.com/auburnzone";
	//private const string k_linkTreeURL = "";
	private const string k_itchURL = "";
	private const string k_redditURL = "";
	private const string k_xURL = "";
	private const string k_patreonURL = "";
	//private const string k_tiktokURL = "";
	//private const string k_facebookURL = "";
	//private const string k_youtubeURL = "";

	public void OpenSurvey()
	{
		Application.OpenURL(k_surveyURL);
	}

	public void OpenWebsite()
	{
		Application.OpenURL(k_websiteURL);
	}

	//public void OpenLinkTree()
	//{
	//	Application.OpenURL(k_linkTreeURL);
	//}

	public void OpenItch()
	{
		Application.OpenURL(k_itchURL);
	}

	public void OpenReddit()
	{
		Application.OpenURL(k_redditURL);
	}

	public void OpenX()
	{
		Application.OpenURL(k_xURL);
	}

	public void OpenPatreon()
	{
		Application.OpenURL(k_patreonURL);
	}
}
