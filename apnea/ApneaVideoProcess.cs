using System.Collections;
using OpenCvSharp;
using OpenCvSharp.XImgProc;

namespace apnea;

public class ApneaVideoProcess
{
    private string video_path;
    private double fps;
    private int videoWidth, videoHeight;
    private List<double> rr_rate;
    private VideoCapture videoCapture;
    private CascadeClassifier detector;
    private Rect chest_area;
    public ApneaVideoProcess(string video_path)
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

        rr_rate = new List<double>();
        videoWidth = (int)videoCapture.Get(VideoCaptureProperties.FrameWidth);
        videoHeight = (int)videoCapture.Get(VideoCaptureProperties.FrameHeight);

        fps = videoCapture.Get(VideoCaptureProperties.Fps);
        get_breath_data();
        videoCapture.Release();
    }

    void set_video_path(string video_path)
    {
        this.video_path = video_path;
    }
    
    public void draw_data(List<Double> arr)
    {
        
    }

    public void write_to_csv(string filePath)
    {
        string s = "";
        rr_rate.ForEach(val => s += val.ToString()+",");
        // var csv = new StringBuilder();
        // csv.AppendLine(s[..^1]);
        File.WriteAllText(filePath, s);
    }

    public List<double> get_rr_rate()
    {
        return rr_rate;
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

    private void get_chest_area(Mat<int> frame)
    {
        Rect face = find_face(frame);
        int start_x = int.Max((int)(face.X - face.Width * 1.2), 0);
        int start_y = int.Min(videoHeight-1, (int)(face.Y + 1.3 * face.Height));
        int end_x = int.Min(videoWidth-1, (int)(face.Width * 3.5 + start_x)) - start_x;
        int end_y = int.Min(videoHeight-1, face.Height * 2 + start_y) - start_y;
        chest_area = new Rect(start_x, start_y, end_x, end_y);
    }

    private Mat<int> get_chest_img(Mat<int> frame)
    {
          return new Mat<int>(frame, chest_area);
    }
    private void get_breath_data()
    {
        rr_rate.Clear();
        double last_data = 0.0;
        Mat<int> frame = new Mat<int>(videoHeight, videoWidth);
        bool first_frame = true;
        while (videoCapture.Read(frame))
        {
            Cv2.CvtColor(frame, frame, ColorConversionCodes.RGB2GRAY);
            if (first_frame)
            {
                get_chest_area(frame);
                first_frame = false;
            }
            double frame_gray_value;
            Mat<int> chest_img = get_chest_img(frame);
            
            // can't find chest
            if (chest_img.Height == 0)
            {
                rr_rate.Add(last_data);
                continue;
            }
            var super_pixel = SuperpixelSLIC.Create(chest_img,SLICType.SLICO, 10, 0.075F);
            super_pixel.Iterate(10);
            Mat<int> super_pixel_res = new Mat<int>(super_pixel.GetNumberOfSuperpixels(), super_pixel.GetNumberOfSuperpixels());
            super_pixel.GetLabels(super_pixel_res);
            var data = super_pixel_res.ToArray();
            int sum = data.Sum();
            last_data = sum / (double)super_pixel.GetNumberOfSuperpixels();
            // Console.WriteLine($"{sum / (double)super_pixel.GetNumberOfSuperpixels()}");
            rr_rate.Add(last_data);
        }
    }
    public double get_fps()
    {
        return fps;
    }


    
    

}


