using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProtectedService.Controllers;

[ApiController]
[Route("[controller]")]
public class StreamingController : ControllerBase
{
    private const string Content = "This is some text content to stream from memory.\nEnjoy!";
    
    [HttpGet("stream")]
    public async Task<IActionResult> StreamContent()
    {
        Response.ContentType = "application/octet-stream";
        Response.Headers.Append("Content-Disposition", "attachment; filename=sample-file.txt");

        var byteArray = Encoding.UTF8.GetBytes(Content);
        await using var memStream = new MemoryStream(byteArray);
        
        await memStream.CopyToAsync(Response.Body);

        return new EmptyResult();
    }

    [HttpGet("dynamic-stream")]
    public async Task<IActionResult> StreamDynamicContent()
    {
        // Generate content dynamically
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

        for (int i = 1; i <= 10; i++)
        {
            await writer.WriteLineAsync($"Line {i}: This is dynamically generated content.");
        }

        await writer.FlushAsync();
        memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream position to the beginning

        Response.ContentType = "application/octet-stream";
        Response.Headers.Append("Content-Disposition", "attachment; filename=dynamicContent.txt");

        await memoryStream.CopyToAsync(Response.Body);

        return new EmptyResult();
    }
    
    [HttpGet("dataset")]
    public async Task StreamDataset()
    {
        Response.ContentType = "text/plain";

        // Example: Simulate a large dataset being streamed
        await foreach (var line in GetLargeDataset())
        {
            var buffer = Encoding.UTF8.GetBytes(line + "\n");
            await Response.Body.WriteAsync(buffer, 0, buffer.Length);
            await Response.Body.FlushAsync(); // Flush to send each chunk to the client
        }
    }

    private async IAsyncEnumerable<string> GetLargeDataset()
    {
        for (int i = 1; i <= 1000; i++)
        {
            await Task.Delay(10); // Simulate data generation latency
            yield return $"Data Line {i}";
        }
    }
}