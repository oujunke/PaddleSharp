using OpenCvSharp;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using System.Drawing;
using WebRobot.Servers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
//if (!await PaddleOcrServer.InitAsync())
//{

//}
//var bitmap = Image.FromFile("639039801473289806.jpeg");
//var currentBitmap = new Bitmap(bitmap);
//var currentMat = new Mat(currentBitmap.Height, currentBitmap.Width, MatType.CV_8UC(3));
//BitmapConverter.ToMat(currentBitmap, currentMat);
var currentMat = Cv2.ImRead("639039801473289806.jpeg");
//await PaddleOcrServer.OcrAll(currentMat);
//Environment.SetEnvironmentVariable("GLOG_v", "0");
FullOcrModel model = await OnlineFullModels.ChineseServerV5.DownloadAsync();
Console.WriteLine("Success-2:" + DateTime.Now);
using (PaddleOcrAll all = new(model)
{
    AllowRotateDetection = true,
    Enable180Classification = true,
})
{
    Console.WriteLine("Success-1:" + DateTime.Now);
    PaddleOcrResult result = all.Run(currentMat);
}
Console.WriteLine("Success:"+DateTime.Now);
app.MapGet("/weatherforecast", () =>
{
var forecast = Enumerable.Range(1, 5).Select(index =>
    new WeatherForecast
    (
        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
    ))
    .ToArray();
return forecast;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
