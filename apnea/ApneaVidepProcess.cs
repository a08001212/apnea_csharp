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
    private List<Double> get_breath_data()
    {
        Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
        while (videoCapture.Read(frame))
        {
            Cv2.CvtColor(frame, frame, ColorConversionCodes.RGB2GRAY);
            var face = detector.DetectMultiScale(frame, 1.1, 4);
            // Cv2.Rectangle(img, (int)); (img, (x, y), (x+w, y+h), [255,0,0], 2)
            // Cv2.Rectangle(frame, (100,100), (200,200), (255,0,0), 2 );
            // Cv2.Rectangle(frame, face, 255, 1, LineTypes.Link4, 0);
            if (face.Length >= 1)
            {
                Console.WriteLine($"{face[0].X}, {face[0].Y}, {face[0].Width}, {face[0].Height}");
            }

            
            Cv2.ImShow("img", frame);
            // Cv2.WaitKey(0);
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
        detector = new CascadeClassifier(@"..\..\..\haarcascades\haarcascade_frontalface_alt_tree.xml");
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


