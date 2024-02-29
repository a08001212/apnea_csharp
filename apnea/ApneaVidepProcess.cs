using System.Collections;
using OpenCvSharp;
using OpenCvSharp.XImgProc;

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
        // Cv2.ImShow("test", frame);
        // Cv2.WaitKey();
        var faces = detector.DetectMultiScale(frame, 2, 4);
        if (faces.Length == 0)
        {
            return new Rect(-1, -1, -1, -1);
        }
        int ans = 0, max_area = 0;
        // var face = faces[0];
        for (int i = 0; i < faces.Length; ++i)
        {
            var area = faces[i].Height * faces[i].Width;
            if (area > max_area)
            {
                ans = i;
                max_area = area;
            }
        }
        
        return faces[ans];
    }
    private List<Double> get_breath_data()
    {
        double last_data = 0.0;
        List<double> ans = new List<double>();
        Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
        while (videoCapture.Read(frame))
        {
            double frame_gray_value;
            Cv2.CvtColor(frame, frame, ColorConversionCodes.RGB2GRAY);
            var face = find_face(frame);
            
            if (face.X == -1 || face.X - (int)(face.Width * 1.2) < 0 ||(int)(face.Y + 1.3 * face.Height)+face.Height*2 >= frame.Height || face.X - (int)(face.Width * 1.2) + 4*face.Width >= frame.Width)
            {
                ans.Add(last_data);
                continue;
            }
            Rect chest = new Rect(face.X - (int)(face.Width * 1.2),(int)(face.Y + 1.3 * face.Height), face.Width * 4, face.Height * 2);
            Mat<int> chest_img = new Mat<int>(frame, chest);
            var super_pixel = SuperpixelSLIC.Create(chest_img,SLICType.SLICO, 10, 0.075F);
            // Console.WriteLine($"{super_pixel}");
            Console.WriteLine(super_pixel.GetNumberOfSuperpixels());
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


