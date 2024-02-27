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
    private List<Mat<int>> frames;
    public ApneaVidepProcess(string video_path)
    {
        frames = new List<Mat<int>>();
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
            Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
            // var ret = videoCapture.Read(frame);
            // if(!ret)    break;

            if (!videoCapture.Read(frame))
            {
                break;
            }

            // Cv2.CvtColor(frame, Cv2.Color);
            Cv2.CvtColor(frame, frame, ColorConversionCodes.RGB2GRAY);
            
            frames.Add(frame);
        }
        Cv2.ImShow("img", frames[500]);
        // Cv2::waitKey(0);
        Cv2.WaitKey(0);
    }
    

    public double get_fps()
    {
        return fps;
    }
    
    

}


