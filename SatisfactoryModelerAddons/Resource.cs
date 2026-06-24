using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons
{
    public class Resource
    {
        private static readonly SemaphoreSlim _semaphore = new(4);
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        private static readonly HttpClient _http = new();

        static Resource()
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        public async static Task<HtmlDocument> GetDocAsync(string url, string dirName)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(Encoding.Default.GetString((await GetAsync(url, dirName, "")).Item2));
            return doc;
        }

        public async static Task<string> DownloadAsync(string url, string dirName, string defaultContent)
        {
            return (await GetAsync(url, dirName, defaultContent)).Item1;
        }

        public async static Task<Tuple<string, byte[]>> GetAsync(string url, string dirName, string defaultContent)
        {
            var uri = new Uri(url);
            var fileName = Path.Combine(dirName, uri.AbsolutePath.Trim('/').Replace("/", "_"));
            if (File.Exists(fileName))
            {
                if (File.GetLastWriteTimeUtc(fileName) > DateTime.UtcNow.AddDays(-1))
                {
                    return new Tuple<string, byte[]>(fileName, await File.ReadAllBytesAsync(fileName));
                }
                else
                {
                    File.Delete(fileName);
                }
            }
            var locker = _locks.GetOrAdd(fileName, _ => new SemaphoreSlim(1));
            await locker.WaitAsync();
            try
            {
                if (File.Exists(fileName))
                {
                    _locks.Remove(fileName, out _);
                    return new Tuple<string, byte[]>(fileName, await File.ReadAllBytesAsync(fileName));
                }
                try
                {
                    await _semaphore.WaitAsync();
                    byte[] content;
                    try
                    {
                        content = await _http.GetByteArrayAsync(uri);
                    }
                    catch (Exception)
                    {
                        //gif 1x1 transparent
                        content = Convert.FromBase64String(defaultContent);
                    }
                    await File.WriteAllBytesAsync(fileName + ".tmp", content);
                    File.Move(fileName + ".tmp", fileName); // atomicité du fichier
                    _locks.Remove(fileName, out _);
                    return new Tuple<string, byte[]>(fileName, content);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            finally
            {
                locker.Release();
            }
        }

    }
}


