using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLImageProvider : MonoBehaviour
{

    public Action<Texture2D> Callback;


    public void ShowUploadDialog(Action<Texture2D> callback ) {
        Callback += callback;
        GetImage.GetImageFromUserAsync(gameObject.name, "ReceiveImage");
    }

    static string s_dataUrlPrefix = "data:image/png;base64,";
    public void ReceiveImage(string dataUrl) {
        if (dataUrl.StartsWith(s_dataUrlPrefix)) {
            byte[] pngData = System.Convert.FromBase64String(dataUrl.Substring(s_dataUrlPrefix.Length));

            // Create a new Texture (or use some old one?)
            Texture2D tex = new Texture2D(1, 1); // does the size matter?
            if (tex.LoadImage(pngData)) {
                Callback(tex);

            } else {
                Debug.LogError("could not decode image");
            }
        } else {
            Debug.LogError("Error getting image:" + dataUrl);
        }
    }
}
