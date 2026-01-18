// See https://aka.ms/new-console-template for more information
using ConsoleTest;
using OpenCvSharp;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;

StringSimilarity stringSimilarity = new StringSimilarity();
double similarity = StringSimilarity.CalculateSimilarity("星期", "星斯");
var currentMat = Cv2.ImRead("639039801473289806.jpeg");
//await PaddleOcrServer.OcrAll(currentMat);
Environment.SetEnvironmentVariable("GLOG_v", "0");
FullOcrModel model = await OnlineFullModels.ChineseServerV5.DownloadAsync();
using (PaddleOcrAll all = new(model)
{
    AllowRotateDetection = true,
    Enable180Classification = true,
})
{

    PaddleOcrResult result = all.Run(currentMat);
}
Console.WriteLine("Hello, World!");
