using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class Gallary : MonoBehaviour
{

    RectTransform myTransform;
    private HashSet<string> loadedFiles = new HashSet<string>(); // Collection to store names of loaded files
    private List<string> loadedFileNames = new List<string>(); // List to store names of loaded files
    private string screenshotName;
    void Start()
    {
        // Load PNGs from the specified folder path
        myTransform = GetComponent<RectTransform>();
        LoadPNGsAndCreateSpritesFromFolder();
    }
    public void LoadPNGsAndCreateSpritesFromFolder()
    {
        // Get the full path to the folder within the persistent data path
        string fullFolderPath = Application.persistentDataPath;

        // Check if the folder exists
        if (Directory.Exists(fullFolderPath))
        {
            // Get all PNG files in the folder
            string[] pngFiles = Directory.GetFiles(fullFolderPath, "*.png");

            // Loop through each PNG file
            foreach (string pngFilePath in pngFiles)
            {
                // Check if the file has already been loaded
                if (!loadedFiles.Contains(pngFilePath))
                {
                    // Load the PNG file into a Texture2D
                    Texture2D texture = LoadPNGToTexture2D(pngFilePath);

                    if (texture != null)
                    {
                        // Convert the texture to a sprite
                        Sprite sprite = SpriteFromTexture(texture);

                        // Create a UI Image for the sprite and make it a child of the canvas
                        CreateUIImageWithSprite(sprite, Path.GetFileNameWithoutExtension(pngFilePath));

                        // Add the file name to the loadedFiles collection
                        loadedFiles.Add(pngFilePath);
                        //loadedFileNames.Add();
                        Debug.Log("Value added method get called");
                        myTransform.sizeDelta = new Vector2(myTransform.sizeDelta.x, myTransform.sizeDelta.y+73f);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Folder not found: " + fullFolderPath);
        }
    }

    Texture2D LoadPNGToTexture2D(string filePath)
    {
        if (File.Exists(filePath))
        {
            // Read the PNG file data
            byte[] fileData = File.ReadAllBytes(filePath);
            // Create a new Texture2D
            Texture2D texture = new Texture2D(2, 2);
            // Load the PNG data into the Texture2D
            texture.LoadImage(fileData);
            return texture;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }
    Sprite SpriteFromTexture(Texture2D texture)
    {
        // Create a new sprite using the loaded texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        return sprite;
    }
    public void ScreenshotName(string screenshotName)
    {
        this.screenshotName = screenshotName;
    }
    void CreateUIImageWithSprite(Sprite sprite, string name)
    {
        // Create a new UI Image
        GameObject imageObject = new GameObject("GameObject", typeof(Image));
        Image imageComponent = imageObject.GetComponent<Image>();
        imageObject.AddComponent<Button>().onClick.AddListener(() => ExpandImage(sprite, imageObject));
        imageObject.AddComponent<GallaryImage>().nameOfImage = name;
        // Set the sprite for the Image component
        imageComponent.sprite = sprite;

        // Make the new Image a child of the canvas
        imageObject.transform.SetParent(this.transform, false);
        Debug.Log("new gameobject has been created");
    }
    void ExpandImage(Sprite sprite, GameObject _gameobject)
    {
        GallaryManager.Instance.deleteImageButton.onClick.AddListener(() => DeleteImage(_gameobject));
        GallaryManager.Instance.shareImageButton.onClick.AddListener(() =>ShareImage(_gameobject));
        Debug.Log("Calling expland method and the object name is" + _gameobject.GetComponent<GallaryImage>().nameOfImage);
        GallaryManager.Instance.ShowImage();
        GallaryManager.Instance.imageViewer.sprite = sprite;
    }
    void DeleteImage(GameObject gameobject_)
    {
        string directoryPath = Application.persistentDataPath;
        string fileNameToDelete = gameobject_.GetComponent<GallaryImage>().nameOfImage;
       
        // Combine directory path and file name to get the full file path
        string filePathToDelete = Path.Combine(directoryPath, fileNameToDelete);
        Debug.Log("The name of file i want to delete is" + filePathToDelete);
        //Destroy(gameobject_);
        gameobject_.SetActive(false);
        GallaryManager.Instance.CloseGallery();
        GallaryManager.Instance.CloseImage();
       
        if (File.Exists(filePathToDelete+".png"))
        {
            Debug.Log("if file folder exists or not");
            // Attempt to delete the file
            try
            {
                File.Delete(filePathToDelete + ".png");
                Debug.Log("File deleted successfully: " + filePathToDelete);
            }
            catch (IOException e)
            {
                Debug.LogError("Error deleting file: " + filePathToDelete + "\n" + e.Message);
            }
        }
        else
        {
            Debug.Log("No file found");
        }
    }
    public void ShareImage(GameObject game_object)
    {
        string directoryPath = Application.persistentDataPath;
        string fileNameToDelete = game_object.GetComponent<GallaryImage>().nameOfImage;

        // Combine directory path and file name to get the full file path
        string filePathToShare = Path.Combine(directoryPath, fileNameToDelete);

        StartCoroutine(Share(filePathToShare + ".png"));
    }
    private IEnumerator Share(string filePath)
    {
        yield return new WaitForEndOfFrame();
        new NativeShare().AddFile(filePath)
            //.SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
    }
}
