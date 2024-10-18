using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraToDomColors : MonoBehaviour
{
    public Camera cam; 
    public RenderTexture renderTexture;
    
    private Queue<Texture2D> imageQueue = new Queue<Texture2D>();
    private int maxQueueSize = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        renderTexture = new RenderTexture(128, 128, 24);
        cam.targetTexture = renderTexture;
        
        StartCoroutine(CaptureAndProcessImages());

    }

    IEnumerator CaptureAndProcessImages()
    {
        FuzzyCMeans fcm = new FuzzyCMeans(3, 1.5f); // FCM initialization

        while (true)
        {
            // Capture the current frame
            Texture2D capturedImage = CaptureImage();
            
            // Add the new image to the queue
            if (imageQueue.Count >= maxQueueSize)
            {
                // Remove the oldest image if the queue is full
                imageQueue.Dequeue();
            }
            imageQueue.Enqueue(capturedImage);

            if (imageQueue.Count == maxQueueSize)
            {

                // Combine all images from the queue to perform FCM
                Color[] combinedColors = CombineColorsFromQueue();
                Color[] dominantColors = fcm.ClusterColors(combinedColors);

                // Process the dominant colors
                Debug.Log("Dominant Colors from the last 5 frames:");
                for (int i = 0; i < dominantColors.Length; i++)
                {
                    Debug.Log($"Dominant Color {i}: {dominantColors[i]}");
                }

                // Generate camouflage texture and save it
                Texture2D camouflagedTexture = GenerateCamouflageTexture(dominantColors, 128, 128, 4);
                //Texture2D smoothedTexture = ApplyErosionAndDilation(camouflagedTexture);
                //SaveTextureAsPNG(smoothedTexture, "SmoothedCamouflage.png");
                SaveTextureAsPNG(camouflagedTexture, "CamouflageTexture.png");
            }
            
            cam.transform.position += new Vector3(0, 0.25f, 0);

            // Wait for the next frame (or adjust the interval as needed)
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    Color[] CombineColorsFromQueue()
    {
        List<Color> combinedColors = new List<Color>();

        foreach (Texture2D image in imageQueue)
        {
            combinedColors.AddRange(image.GetPixels());
        }

        return combinedColors.ToArray();
    }
    
    public Texture2D ApplyErosionAndDilation(Texture2D texture)
    {
        ImageProcessing ip = new ImageProcessing();
        Texture2D erodedTexture = ip.ErodeTexture(texture);
        Texture2D dilatedTexture = ip.DilateTexture(erodedTexture);
        return dilatedTexture;
    }
    
    Texture2D CaptureImage()
    {
        // Set the camera to render to the RenderTexture
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Create Texture2D and read the RenderTexture into it
        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        // Restore the original RenderTexture
        RenderTexture.active = currentRT;

        return image;
    }
    
    Texture2D GenerateCamouflageTexture(Color[] dominantColors, int textureWidth, int textureHeight, int squareSize)
    {
        Texture2D camouflageTexture = new Texture2D(textureWidth, textureHeight);
    
        for (int y = 0; y < textureHeight; y += squareSize)
        {
            for (int x = 0; x < textureWidth; x += squareSize)
            {
                // Select a random dominant color for the current square
                Color randomColor = dominantColors[Random.Range(0, dominantColors.Length)];
            
                // Fill the square with the selected color
                for (int i = 0; i < squareSize; i++)
                {
                    for (int j = 0; j < squareSize; j++)
                    {
                        // Ensure we stay within the bounds of the texture
                        if (x + i < textureWidth && y + j < textureHeight)
                        {
                            camouflageTexture.SetPixel(x + i, y + j, randomColor);
                        }
                    }
                }
            }
        }
    
        camouflageTexture.Apply();
        return camouflageTexture;
    }
    
    void SaveTextureAsPNG(Texture2D texture, string baseFileName)
    {
// Get the current time and format it as a string

        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{baseFileName}_{timeStamp}.png";
        string outputFolder = Path.Combine(Application.dataPath, "output");

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        string filePath = Path.Combine(outputFolder, fileName);

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    
        Debug.Log($"Texture saved to: {filePath}");
    }
}
