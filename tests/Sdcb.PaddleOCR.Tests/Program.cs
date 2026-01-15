using OpenCvSharp;
using OpenCvSharp.Aruco;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

//[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

namespace Sdcb.PaddleOCR.Tests;

internal class Program
{
    public static async Task Main()
    {
        Environment.SetEnvironmentVariable("GLOG_v", "0");
        FullOcrModel model = await OnlineFullModels.ChineseV5.DownloadAsync();
        FastCheck(model);
    }

    private static void FastCheck(FullOcrModel model)
    {
        // from: https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png
        var name = @"639039801473289806.jpeg";
        byte[] sampleImageData = File.ReadAllBytes(name);

        using (PaddleOcrAll all = new(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = true,
        })
        {
            // Load local file by following code:
            // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
            using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
            {
                PaddleOcrResult result = all.Run(src);
                Directory.CreateDirectory("Res");
                Console.WriteLine("Detected all texts: \n" + result.Text);
                var mat = Cv2.ImRead(name);
                int index = 1;
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    var tm = new Mat(mat, region.Rect.BoundingRect()).Clone();
                    foreach (var item in region.Chars)
                    {
                        Cv2.Rectangle(tm, item.Rec, new Scalar(0, 0, 255), 2);
                    }
                    Cv2.ImWrite($"Res/{index++}.jpeg", tm);
                    Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                }
            }
        }
    }
}
