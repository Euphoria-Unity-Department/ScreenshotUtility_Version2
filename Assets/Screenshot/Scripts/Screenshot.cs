using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public enum filePath
{
	Downloads,
	UnityEditor,
	Desktop,
	Documents
}
public class Screenshot : MonoBehaviour
{
    public filePath filepath;
    public string screenshotName = "Screenshot_";
    [Header("* Album name to create in device")]
    public string folderName;
    [Space(5)]
    [Header("* SAVE screenshots to Gallery")]
    public bool saveToGallery;
    [Space(5)]
    [Header("* SHARE screenshots")]
    public bool share;
	[Space(5)]
	[Header("* SAVE screenshots to App's Gallery")]
	public bool appGallery;
	[Space(5)]
    [Header("* WATERMARK on screenshots")]
    public bool watermarkText;
    [Space(5)]
    public string waterMarkText;
	public bool watermarkImage;
	[Space(5)]
	public Image waterMarkImage;
	[Space(-5)]
	[Header("(Leave it empty if you don't want image logo)")]
	[Space(5)]
	[Header("* Pass All UI objects")]
    [Space(-15)]
    [Header("    you want to make 'invisible' in screenshot")]
    public GameObject[] UIObjects;
    [Space(5)]
    [Header("* Pass All GameObjects")]
    [Space(-15)]
    [Header("  you want to make 'Inactive' in screenshot")]
    public GameObject[] GameObjects;
	public Button capture;
	private Texture2D screenshotTexture;
	private bool takingScreenshot = false;
	private int numberOfCharsAllowed = 32;
	private Gallary _gallery;
	[HideInInspector]
	public static int index=0;
	void Start()
    {
		Debug.Log("persitance data path" + Application.persistentDataPath);
		capture.onClick.AddListener(_CaptureScreenshot);
		GalleryButton();
		_gallery = GallaryManager.Instance.gallery.GetComponent<Gallary>();

	}
	// Update is called once per frame
	void Update()
	{
		//to auto-rename screenshot
		AutoPlaceholder();
#if UNITY_EDITOR
		//ClickEditorScreen();
		
#endif
	}
	public void GalleryButton()
    {
        if (appGallery)
        {
			GallaryManager.Instance.appGallaryButton.SetActive(true);
        }
        else
        {
			GallaryManager.Instance.appGallaryButton.SetActive(false);
		}
    }
	public void ClickEditorScreen()
    {
		if (Input.GetMouseButtonDown(0))
		{
			CaptureScreenshot();
		}
	}
	void DeactivateObjects(GameObject[] _Objects)
	{
        if (watermarkImage)
        {
			waterMarkImage.gameObject.SetActive(true);
        }
		GallaryManager.Instance.appGallaryButton.SetActive(false);
		capture.gameObject.SetActive(false);
		foreach (GameObject obj in _Objects)
		{
			if (obj != null)
				obj.SetActive(false);
		}
	}

	void ReactivateObjects(GameObject[] _Objects)
	{
		if (watermarkImage)
		{
			waterMarkImage.gameObject.SetActive(false);
		}
		GallaryManager.Instance.appGallaryButton.SetActive(true);
		capture.gameObject.SetActive(true);
		foreach (GameObject obj in _Objects)
		{
			if (obj != null)
				obj.SetActive(true);
		}
	}
	// this method is to rename screenshot automatically(only in editor mode)
	public void AutoPlaceholder()
    {
		if (string.IsNullOrEmpty(screenshotName))
		{
			screenshotName = "Screenshot_";
        }
        else
        {
			return;
        }
	}
	//filepath for screenshot only in editor mode
	public string FilePath()
    {
        if (filepath ==filePath.Downloads)
        {
			return "Downloads";
        }
		else if (filepath == filePath.UnityEditor)
        {
			//return "UnityEditor";
			return "";
		}
		else if(filepath == filePath.Desktop)
        {
			return "Desktop";
        }
		else if (filepath == filePath.Documents)
        {
			return "Documents";
        }
		return "Downloads";

	}
	//capture screen in editor mode
	public void CaptureScreenshot()
	{
		takingScreenshot = true;
		string screenshotName = this.screenshotName +DateTime.Now.ToString("yyyy-MM-dd-HHmmss-fff") + ".png";
		string downloadsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), FilePath());
		string filePath = Path.Combine(downloadsFolderPath, screenshotName);
		DeactivateObjects(GameObjects);
		DeactivateObjects(UIObjects);
		//StartCoroutine(CaptureAndMoveScreenshot(screenshotName, filePath));
		StartCoroutine(TakeScreenshotAndSaveEditor(filePath));
	}
	public void AppGalleryScreenshot(Texture2D screenshot)
    {
		byte[] pngData = screenshot.EncodeToPNG();
		string name = "Screenshot"+ DateTime.Now.ToString("yyyy - MM - dd - HHmmss - fff") +".png";
		//string name = index+".png";

		// Specify the file path within the persistent data path
		string filePath = Path.Combine(Application.persistentDataPath, name);

		// Write the PNG data to the file
		File.WriteAllBytes(filePath, pngData);
		_gallery.LoadPNGsAndCreateSpritesFromFolder();
		_gallery.ScreenshotName(name);
	}
	//previous method to take screenshot in editor mode
	//private IEnumerator CaptureAndMoveScreenshot(string screenshotName, string filePath)
	//{
		
	//	ScreenCapture.CaptureScreenshot(screenshotName);
 //       if (appGallery)
 //       {
	//		//these lines of code is used to convert screenshot into texture 
	//		byte[] screenshotData = File.ReadAllBytes(screenshotName);
	//		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA64, false);
	//		texture.LoadImage(screenshotData);
	//		texture.Apply();
	//		//call the method to save screenshot into persistance data path
	//		AppGalleryScreenshot(texture);
	//	}
	//	// Wait for a short period to ensure the screenshot is saved
	//	yield return new WaitForSeconds(0.5f);
	//	takingScreenshot = false;
	//	File.Move(screenshotName, filePath);
 //       ReactivateObjects(GameObjects);
	//	ReactivateObjects(UIObjects);
	//}
	
	//for mobile devices
	public void _CaptureScreenshot()
	{
#if UNITY_EDITOR
		CaptureScreenshot();
#else
		if (saveToGallery)
        {
			takingScreenshot = true;
			StartCoroutine(TakeScreenshotAndSave());
		}
		if (share)
        {
			takingScreenshot = true;
			StartCoroutine(TakeScreenshotAndShare());
		}
#endif
	}

	private IEnumerator TakeScreenshotAndSave()
	{
		DeactivateObjects(GameObjects);
		DeactivateObjects(UIObjects);
		yield return new WaitForEndOfFrame();
		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA64, false);
		takingScreenshot = false;
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();
        if (appGallery)
        {
			AppGalleryScreenshot(ss);
		}
		string name = string.Format(DateTime.Now.ToString("yyyy-MM-dd-HHmmss-fff"));
		Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss,folderName, screenshotName+name));
		ReactivateObjects(GameObjects);
		ReactivateObjects(UIObjects);
	}
	//for editor mode method alteration to save data in persistance data path
	private IEnumerator TakeScreenshotAndSaveEditor(string filePath)
	{
		DeactivateObjects(GameObjects);
		DeactivateObjects(UIObjects);
		yield return new WaitForEndOfFrame();
		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA64, false);
		takingScreenshot = false;
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();
		if (appGallery)
		{
			AppGalleryScreenshot(ss);
		}
		File.WriteAllBytes(filePath, ss.EncodeToPNG());
		ReactivateObjects(GameObjects);
		ReactivateObjects(UIObjects);
	}
	public string TruncateString(string input, int maxLength)
	{
		if (input.Length <= maxLength)
		{
			return input;
		}
		else
		{
			return input.Substring(0, maxLength);
		}
	}
	private void OnGUI()
	{
		if (takingScreenshot && watermarkText)
		{
			// Show the screenshot texture
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), screenshotTexture);

			GUI.color = Color.white;

			float diagonalAngle = Mathf.Atan2(Screen.height, Screen.width) * Mathf.Rad2Deg;

			// Rotate the GUI matrix around the center of the screen
			GUIUtility.RotateAroundPivot(-diagonalAngle, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

			// Show the watermark text
			GUIStyle style = new GUIStyle();
			style.normal.textColor = new Color(0f, 0f, 0f, 0.25f); // White with 50% transparency
			style.fontSize = 150; // Adjust the font size as needed
			style.alignment = TextAnchor.MiddleCenter;
			Debug.Log("screenshot GUI method");
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), TruncateString(waterMarkText, numberOfCharsAllowed), style);

			// Reset the GUI matrix rotation
			GUIUtility.RotateAroundPivot(diagonalAngle, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
			//takingScreenshot = false;
		}
	}
	private IEnumerator TakeScreenshotAndShare()
	{
		DeactivateObjects(GameObjects);
		DeactivateObjects(UIObjects);
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();

		string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// To avoid memory leaks
		Destroy(ss);

		new NativeShare().AddFile(filePath)
			//.SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
			.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
			.Share();
		ReactivateObjects(GameObjects);
		ReactivateObjects(UIObjects);
	}
	//seprate method to share file just pass and filepath to the method
	private IEnumerator Share(string filePath)
    {
		yield return new WaitForEndOfFrame();
		new NativeShare().AddFile(filePath)
			//.SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
			.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
			.Share();

	}
}
