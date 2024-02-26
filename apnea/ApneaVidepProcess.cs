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
        try
        {
            videoCapture = new VideoCapture(video_path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        fps = videoCapture.Get(VideoCaptureProperties.Fps);
        // string path = "apnea/haarcascades/haarcascade_frontalface_alt2.xml";
        // detector.Load("apnea/haarcascades/haarcascade_frontalface_alt2.xml");
        while (true)
        {
            ret, frame = videoCapture.Read();
            if(!ret)    break;
            Cv2.CvtColor(frame, Cv2.Color);
        }
    }

    public double get_fps()
    {
        return fps;
    }
    
    

}


