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
    private List<OutputArray> frames;
    public ApneaVidepProcess(string video_path)
    {
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
        // string path = "apnea/haarcascades/haarcascade_frontalface_alt2.xml";
        // detector.Load("apnea/haarcascades/haarcascade_frontalface_alt2.xml");
        while (true)
        {
            OutputArray frame = new OutputArray(Mat<int>(videoHeight, videoWidth));
            var ret = videoCapture.Read(frame);
            if(!ret)    break;
            // Cv2.CvtColor(frame, Cv2.Color);
            frames.Add(frame);
        }
    }

    public double get_fps()
    {
        return fps;
    }
    
    

}


