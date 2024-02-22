using OpenCvSharp;

namespace apnea;

public class ApneaVidepProcess
{
    private string video_path;
    private double fps;
    private VideoCapture videoCapture;
    private CascadeClassifier detector;
    public ApneaVidepProcess(string video_path)
    {
        this.video_path = video_path;
        videoCapture = new VideoCapture(video_path);
        fps = videoCapture.Get(VideoCaptureProperties.Fps);
        // string path = "apnea/haarcascades/haarcascade_frontalface_alt2.xml";
        detector.Load("apnea/haarcascades/haarcascade_frontalface_alt2.xml");
        
    }

    public double get_fps()
    {
        return fps;
    }
    
    

}


