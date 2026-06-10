using System;
using System.Linq;
using System.Net.Http;
using System.Threading;

if (args.Length <= 1)
{
    Console.WriteLine("Usage: 07.exe [A|B|C] <url> ...");
    return;
}

var client = new HttpClient();

IDownloader downloader = args[0] switch
{
    "A" => new ThreadDownloader(client),
    "B" => new TaskDownloader(client),
    "C" => new AsyncDownloader(client),
    var x => throw new ArgumentException(x)
};

var bytes = Task.WhenAll(args[1..].Select(url => downloader.Download(Fixup(url)))).Result.Sum();
Console.WriteLine($"Downloaded {bytes} bytes.");

string Fixup(string url)
    => url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
    ? url
    : $"http://{url}";

interface IDownloader {
    public Task<int> Download(string url);
}

class ThreadDownloader(HttpClient client) : IDownloader {
    public Task<int> Download(string url) {
        int bytes = 0;
        var t = new Thread(() => {
            bytes = client.GetByteArrayAsync(url).Result.Length;
        });
        t.Start();
        t.Join();
        return Task.FromResult(bytes);
    }
}

class TaskDownloader(HttpClient client) : IDownloader {
    public Task<int> Download(string url)
        => Task.Run(() => client.GetByteArrayAsync(url).ContinueWith(t => t.Result.Length));
}

class AsyncDownloader(HttpClient client) : IDownloader
{
    public async Task<int> Download(string url)
    {
        var bytes = await client.GetByteArrayAsync(url);
        return bytes.Length;
    }
}
