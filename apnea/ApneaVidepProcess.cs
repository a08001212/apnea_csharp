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

    public ApneaVidepProcess(string video_path)
    {
        detector = new CascadeClassifier(@"..\..\..\haarcascades\haarcascade_frontalface_alt2.xml");
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


    private Mat<int> get_frame()
    {
        Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
        videoCapture.Read(frame);
        return frame;
    }
    
    private Rect find_face(Mat<int> frame)
    {

        var faces = detector.DetectMultiScale(frame, 1.1, 3);
        // can't find face
        if (faces.Length == 0)
        {
            return new Rect(-1, -1, -1, -1);
        }
        
        int ans = 0, max_area = 0;
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

    private Mat<int> get_chest_img(Mat<int> frame)
    {
        var face = find_face(frame);
        
        // can't find chest
        if (face.X == -1 )
        {
            return new Mat<int>(0, 0);
        }

        int start_x = int.Max((int)(face.X - face.Width * 1.2), 0);
        int start_y = int.Min(frame.Height-1, (int)(face.Y + 1.3 * face.Height));
        int end_x = int.Min(frame.Width-1, (int)(face.Width * 3.5 + start_x)) - start_x;
        int end_y = int.Min(frame.Height-1, face.Height * 2 + start_y) - start_y;
        Rect chest = new Rect(start_x, start_y, end_x, end_y);
        return new Mat<int>(frame, chest);
    }
    private List<Double> get_breath_data()
    {
        double last_data = 0.0;
        List<double> ans = new List<double>();
        Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
        while (videoCapture.Read(frame))
        {
            double frame_gray_value;
            // Cv2.CvtColor(frame, frame, ColorConversionCodes.RGB2GRAY);
            Mat<int> chest_img = get_chest_img(frame);
            
            // can't find chest
            if (chest_img.Height == 0)
            {
                ans.Add(last_data);
                continue;
            }
            var super_pixel = SuperpixelSLIC.Create(chest_img,SLICType.SLICO, 10, 0.075F);
            Mat<int> super_pixel_res = new Mat<int>(super_pixel.GetNumberOfSuperpixels(), super_pixel.GetNumberOfSuperpixels());
            super_pixel.GetLabels(super_pixel_res);
            // Console.WriteLine($"{super_pixel_res}");
            Cv2.ImShow("test", super_pixel_res);
            Cv2.WaitKey(1);
        }
        return new List<double>();
    }

    
    public double get_fps()
    {
        return fps;
    }
    
    

}


