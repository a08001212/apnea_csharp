using System.Collections;
using OpenCvSharp;

namespace apnea;

public class ApneaVidepProcess
{
    private string video_path;
    private double fps;
    private int videoWidth, videoHeight;

    private VideoCapture videoCapture;
    private CascadeClassifier detector;

    private Rect find_face(Mat<int> frame)
    {
        var faces = detector.DetectMultiScale(frame, 1.1, 4);
        int ans = 0, max_area = 0;
        var face = faces[0];
        for (int i = 0; i < faces.Length; ++i)
        {
            var area = faces[i].Height * faces[i].Width;
            if (area > max_area)
            {
                ans = i;
                max_area = area;
            }
        }

        return faces[max_area];
    }
    private List<Double> get_breath_data()
    {
        Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
        int count = 0;
        while (videoCapture.Read(frame))
        {
            Cv2.CvtColor(frame, frame, ColorConversionCodes.RGB2GRAY);
            var face = find_face(frame);
            Rect chest = new Rect(int.Max(face.X - face.Width, 0), int.Min(videoHeight, (int)(face.Y + 1.25 * face.Height)), face.Width * 3, (int)(face
                .Height * 1.25));
            

        }
        return new List<double>();
    }

    private Mat<int> get_frame()
    {
        Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
        videoCapture.Read(frame);
        return frame;
    }
    public ApneaVidepProcess(string video_path)
    {
        detector = new CascadeClassifier(@"..\..\..\haarcascades\haarcascade_frontalface_alt.xml");
        this.video_path = video_path;
        try
        {
            videoCapture = new VideoCapture(video_path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        videoWidth = (int)videoCapture.Get(VideoCaptureProperties.FrameWidth);
        videoHeight = (int)videoCapture.Get(VideoCaptureProperties.FrameHeight);

        fps = videoCapture.Get(VideoCaptureProperties.Fps);
        get_breath_data();
        videoCapture.Release();
    }
    public double get_fps()
    {
        return fps;
    }
    
    

}


