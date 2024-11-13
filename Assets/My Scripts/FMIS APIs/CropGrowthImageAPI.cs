using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class CropGrowthImageAPI : FMIS_API
{

    private string sensingDate = "2024-08-01"; // Replace with the sensing date (YYYY-MM-DD)
    private int width = 256; // Replace with desired image width
    private int height = 256; // Replace with desired image height
    private string bbox = "your-bbox-here"; // Replace with the bounding box coordinates

    public SpriteRenderer spriteRenderer; // Renderer to display the texture

    public void GetCropGrowthImage(string parcelID, Action<Texture2D> onCropGrowthImageDataReceived)
    {
        // Start the coroutine to fetch the crop growth image
        StartCoroutine(GetCropGrowthImageEnumerator(parcelID, onCropGrowthImageDataReceived));
    }

    // Coroutine to fetch the crop growth image from the API
    IEnumerator GetCropGrowthImageEnumerator(string parcelID, Action<Texture2D> onCropGrowthImageDataReceived)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID) || string.IsNullOrEmpty(bbox))
        {
            Debug.LogError("API key, Parcel ID, or BBOX is missing.");
            yield break;
        }

        string url = $"https://api.cropapp.gr/external/crop-growth-image?parcel_id={parcelID}&sensing_date={sensingDate}&WIDTH={width}&HEIGHT={height}&BBOX={bbox}";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onCropGrowthImageDataReceived?.Invoke(null);
        }
        else if (request.responseCode == 412)  // Handle HTTP 412 error
        {
            Debug.LogError("Error 412: Precondition Failed. Check if the parcel_id, apiKey, sensing_date, or BBOX is correct.");
            onCropGrowthImageDataReceived?.Invoke(null);
        }
        else
        {
            // Get the texture from the API response
            Texture2D cropGrowthImage = DownloadHandlerTexture.GetContent(request);
            Debug.Log("Crop Growth Image fetched successfully.");

            onCropGrowthImageDataReceived?.Invoke(cropGrowthImage);

            // Assign the texture to a material's main texture to display the image
            if (spriteRenderer != null)
            {
                spriteRenderer.material.mainTexture = cropGrowthImage;
            }
        }
    }
}