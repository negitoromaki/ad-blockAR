using System.Collections.Generic;
using System;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;
using UnityEngine;
using System.Net.Security;
using System.IO;


#if UNITY_EDITOR
using System.Security.Cryptography.X509Certificates;
#endif

#if !UNITY_EDITOR
using System.Net.Http;
#endif

public class AnimatedGifDrawer : MonoBehaviour {
	public string loadingGifPath;
	public float speed = 1;
	public Vector2 drawPosition;
    public string filePath = "random.gif";
    public string apiUrl = "https://api.giphy.com/v1/gifs/random?api_key=8x8lSV24JClazrD4mCSy8qJ2dkDLZPiM&tag=adb&rating=G";
    List<Texture2D> gifFrames = new List<Texture2D>();

#if !UNITY_EDITOR
    void Awake() {

        HttpClient client = new HttpClient();
        String html = client.GetStringAsync("https://api.giphy.com/v1/gifs/random?api_key=8x8lSV24JClazrD4mCSy8qJ2dkDLZPiM&tag=adb&rating=PG").Result;
        string id = "";
        //https://api.giphy.com/v1/gifs/random?api_key=8x8lSV24JClazrD4mCSy8qJ2dkDLZPiM&tag=adb&rating=PG
        for (int i = 28; i < (html.IndexOf('"', i)); i++)
            id += html[i];
        string url = "https://media3.giphy.com/media/" + id + "/giphy.gif";
        //Debug.Log(url);

        var content = client.GetAsync(url).Result;
        FileStream fs = new FileStream(filePath, FileMode.Create);
        content.Content.CopyToAsync(fs).Wait();

        var gifImage = Image.FromFile(filePath);
        var dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
        int frameCount = gifImage.GetFrameCount(dimension);
        for (int i = 0; i < frameCount; i++) {
            gifImage.SelectActiveFrame(dimension, i);
            var frame = new Bitmap(gifImage.Width, gifImage.Height);
            System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, Point.Empty);
            var frameTexture = new Texture2D(frame.Width, frame.Height);
            for (int x = 0; x < frame.Width; x++)
                for (int y = 0; y < frame.Height; y++) {
                    System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                    frameTexture.SetPixel(frame.Width - 1 - x, y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A)); // for some reason, x is flipped
                }

            frameTexture.Apply();
            gifFrames.Add(frameTexture);

        }

    }
#else

    void Awake() {

        System.Net.ServicePointManager.ServerCertificateValidationCallback =
                 delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                 { return true; };

        WebClient client = new WebClient();
        String html = client.DownloadString("https://api.giphy.com/v1/gifs/random?api_key=8x8lSV24JClazrD4mCSy8qJ2dkDLZPiM&tag=adb&rating=PG");
        string id = "";
        //https://api.giphy.com/v1/gifs/random?api_key=8x8lSV24JClazrD4mCSy8qJ2dkDLZPiM&tag=adb&rating=PG
        for (int i = 28; i < (html.IndexOf('"', i)); i++)
            id += html[i];
        string url = "https://media3.giphy.com/media/" + id + "/giphy.gif";
        //Debug.Log(url);

        client.DownloadFile(url, filePath);
        var gifImage = Image.FromFile(filePath);
        var dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
        int frameCount = gifImage.GetFrameCount(dimension);
        for (int i = 0; i < frameCount; i++) {
            gifImage.SelectActiveFrame(dimension, i);
            var frame = new Bitmap(gifImage.Width, gifImage.Height);
            System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, Point.Empty);
            var frameTexture = new Texture2D(frame.Width, frame.Height);
            for (int x = 0; x < frame.Width; x++)
                for (int y = 0; y < frame.Height; y++) {
                    System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                    frameTexture.SetPixel(frame.Width - 1 - x, y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A)); // for some reason, x is flipped
                }

            frameTexture.Apply();
            gifFrames.Add(frameTexture);

        }

    }

#endif

    private void Update() {
        GameObject.FindGameObjectWithTag("GIF1").GetComponent<Renderer>().material.mainTexture = gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count];
        GameObject.FindGameObjectWithTag("GIF2").GetComponent<Renderer>().material.mainTexture = gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count];
        GameObject.FindGameObjectWithTag("GIF3").GetComponent<Renderer>().material.mainTexture = gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count];
    }

    void OnGUI() {

        //GUI.DrawTexture(new Rect(drawPosition.x, drawPosition.y, gifFrames[0].width, gifFrames[0].height), gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count]);

    }
}