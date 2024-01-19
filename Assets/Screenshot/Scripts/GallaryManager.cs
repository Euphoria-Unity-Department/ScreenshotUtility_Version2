using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GallaryManager : MonoBehaviour
{
    private static GallaryManager instance;
    public static GallaryManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance ==null)
        {
            instance = this;
        }
        else if (instance!=this)
        {
            Destroy(this.gameObject);
        }
    }
    private CanvasGroup canvasGroup, imageViewerCanvesGroup;
    public float fadeDuration = 1.0f;
    public GameObject gallery, galleryParentCanvesGroup,appGallaryButton, galleryCloseButton,imageViewerObject;
    public Image imageViewer;
    public Button deleteImageButton, shareImageButton;
    private void Start()
    {
        ScriptIntiliazer();
    }
    public void ButtonClick()
    {
       
    }
    public void ScriptIntiliazer()
    {
        canvasGroup = galleryParentCanvesGroup.GetComponent<CanvasGroup>();
        imageViewerCanvesGroup = imageViewerObject.GetComponent<CanvasGroup>();
        SetAlpha(canvasGroup, 0f);
        SetAlpha(imageViewerCanvesGroup, 0f);
        canvasGroup.blocksRaycasts = false;
        galleryCloseButton.SetActive(false);
    }
    public void OpenGallery()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, fadeDuration));
        canvasGroup.blocksRaycasts = true;
        galleryCloseButton.SetActive(true);
    }
    public void CloseGallery()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, fadeDuration));
        galleryCloseButton.SetActive(false);
        canvasGroup.blocksRaycasts = false;
    }
    public void ShowImage()
    {
        StartCoroutine(FadeCanvasGroup(imageViewerCanvesGroup, imageViewerCanvesGroup.alpha, 1f, fadeDuration));
        imageViewerCanvesGroup.blocksRaycasts = true;
    }
    public void CloseImage()
    {
        StartCoroutine(FadeCanvasGroup(imageViewerCanvesGroup, imageViewerCanvesGroup.alpha, 0f, fadeDuration));
        imageViewerCanvesGroup.blocksRaycasts = false;
    }
    IEnumerator FadeCanvasGroup(CanvasGroup canvesGroup,float start, float end, float duration)
    {
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(start, end, currentTime / duration);
            SetAlpha(canvesGroup,alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        // Ensure the CanvasGroup reaches the final alpha value precisely
        SetAlpha(canvesGroup,end);
    }
    // Set the alpha value of the CanvasGroup
    private void SetAlpha(CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
