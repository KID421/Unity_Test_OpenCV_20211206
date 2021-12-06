using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;

public class FaceTracking : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private Renderer ren;
    private CascadeClassifier cascade;
    private OpenCvSharp.Rect myFace;

    [Header("臉部 X、Y 值")]
    public float faceX;
    public float faceY;
    [Header("臉部世界座標")]
    public Vector3 v2FacePosition;
    public Vector3 v3FacePosition;
    [Header("要追蹤的物件")]
    public Transform traTarget;
    [Header("位移")]
    public Vector3 v3Offset = new Vector3(250, -100, 10);

    private void Awake()
    {
        WebCamDevice[] devcies = WebCamTexture.devices;

        webCamTexture = new WebCamTexture(devcies[0].name);
        webCamTexture.Play();

        ren = GetComponent<Renderer>();

        cascade = new CascadeClassifier(Application.dataPath + @"/haarcascade_frontalface_default.xml");
    }

    private void Update()
    {
        UpdateRenderer();
    }

    private void UpdateRenderer()
    {
        //ren.material.mainTexture = webCamTexture;
        Mat frame = OpenCvSharp.Unity.TextureToMat(webCamTexture);
        
        FindNewFace(frame);
        Display(frame);

        print(Input.mousePosition);
    }

    private void FindNewFace(Mat frame)
    {
        var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);

        if (faces.Length >= 1)
        {
            Debug.Log(faces[0].Location);
            myFace = faces[0];
            faceX = myFace.X;
            faceY = myFace.Y;
            v2FacePosition = new Vector3(myFace.X + v3Offset.x, Screen.height - myFace.Y + v3Offset.y, v3Offset.z);
            v3FacePosition = Camera.main.ScreenToWorldPoint(v2FacePosition);
            traTarget.position = v3FacePosition;
        }
    }

    private void Display(Mat frame)
    {
        if (myFace != null)
        {
            frame.Rectangle(myFace, new Scalar(250, 0, 0), 2);
        }

        Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
        ren.material.mainTexture = newTexture;
    }
}
