using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessing
{
    private static readonly Vector2Int[] ErosionKernel = 
    {
        new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2), new Vector2Int(1, -2), new Vector2Int(2, -2),
        new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(2, -1),
        new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
        new Vector2Int(-2, 1), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
        new Vector2Int(-2, 2), new Vector2Int(-1, 2), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2)
    };

    private static readonly Vector2Int[] DilationKernel = 
    {
        new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2), new Vector2Int(1, -2), new Vector2Int(2, -2),
        new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(2, -1),
        new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
        new Vector2Int(-2, 1), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
        new Vector2Int(-2, 2), new Vector2Int(-1, 2), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2)
    };
    
    public Texture2D ErodeTexture(Texture2D texture)
    {
        Texture2D erodedTexture = new Texture2D(texture.width, texture.height);
        Color[] pixels = texture.GetPixels();
        Color[] erodedPixels = new Color[pixels.Length];

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color minColor = Color.white;

                foreach (var offset in ErosionKernel)
                {
                    int nx = x + offset.x;
                    int ny = y + offset.y;

                    if (nx >= 0 && nx < texture.width && ny >= 0 && ny < texture.height)
                    {
                        Color neighborColor = pixels[ny * texture.width + nx];
                        if (neighborColor.grayscale < minColor.grayscale)
                        {
                            minColor = neighborColor;
                        }
                    }
                }

                erodedPixels[y * texture.width + x] = minColor;
            }
        }

        erodedTexture.SetPixels(erodedPixels);
        erodedTexture.Apply();
        return erodedTexture;
    }

    public Texture2D DilateTexture(Texture2D texture)
    {
        Texture2D dilatedTexture = new Texture2D(texture.width, texture.height);
        Color[] pixels = texture.GetPixels();
        Color[] dilatedPixels = new Color[pixels.Length];

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color maxColor = Color.black;

                foreach (var offset in DilationKernel)
                {
                    int nx = x + offset.x;
                    int ny = y + offset.y;

                    if (nx >= 0 && nx < texture.width && ny >= 0 && ny < texture.height)
                    {
                        Color neighborColor = pixels[ny * texture.width + nx];
                        if (neighborColor.grayscale > maxColor.grayscale)
                        {
                            maxColor = neighborColor;
                        }
                    }
                }

                dilatedPixels[y * texture.width + x] = maxColor;
            }
        }

        dilatedTexture.SetPixels(dilatedPixels);
        dilatedTexture.Apply();
        return dilatedTexture;
    }
}
