using OpenCvSharp;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;

namespace WebRobot.Servers
{
    public class PaddleOcrServer
    {
        private static PaddleOcrAll PaddleOcr;
        public static bool IsInit { get; private set; }
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public static async Task<bool> InitAsync()
        {
            if (IsInit)
            {
                return true;
            }
            await _semaphore.WaitAsync();
            try
            {
                if (IsInit)
                {
                    return true;
                }
                Environment.SetEnvironmentVariable("GLOG_v", "0");
                FullOcrModel model = await OnlineFullModels.ChineseServerV5.DownloadAsync();
                PaddleOcr = new(model)
                {
                    AllowRotateDetection = false,
                    Enable180Classification = false,
                };
                IsInit = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public static async Task OcrAll(Mat mat)
        {
            try
            {
                FullOcrModel model = await OnlineFullModels.ChineseServerV5.DownloadAsync();
                PaddleOcr = new(model)
                {
                    AllowRotateDetection = false,
                    Enable180Classification = false,
                };
                var paddleOcrResult = PaddleOcr.Run(mat);
            }
            catch (Exception ex)
            {

            }
           

        }
    }
}
