using OpenCvSharp;

namespace apnea
{
    class Program
    {
        static void Main(String[] args)
        {
            ApneaVideoProcess video = new ApneaVideoProcess(args[0]);
            Console.WriteLine(video.get_fps());
            video.get_rr_rate().ForEach(val => Console.Write($"{val}, "));
            video.write_to_csv("test.csv");
            Console.WriteLine("Finish.");
            
        }            

    }
}

