﻿using OpenCvSharp;

namespace apnea
{
    class Program
    {
        static void Main(String[] args)
        {
            ApneaVidepProcess video = new ApneaVidepProcess(args[0]);
            Console.Write(video.get_fps());
        }
    }
}
