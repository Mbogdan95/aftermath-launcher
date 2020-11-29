using System.Net.Http;

namespace Ignition.Api
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Downloader
    {
        private static Settings Settings { get; } = Settings.Instance;
        public static Dictionary<uint, KeyValuePair<string, byte[]>> ComputedHashes { get; private set; }

        private static Action<string> SetCurrentAction { get; set; }
        private static Action<double, string, string> SetCurrentProgress { get; set; }
        private static long fileIndex;
        private static int DownloadCount { get; set; }

        public static void DownloadFiles(Dictionary<string, string> files, CancellationTokenSource cts)
        {
            DownloadCount = files.Count;
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 2,
                CancellationToken = cts.Token,
            };

            try
            {
                Parallel.ForEach(files, options, (x) =>
                {
                    DownloadFile(x.Key, x.Value);
                    options.CancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Download cancellation was requested by the user.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unknown error caused during download.");
                Debug.WriteLine(ex.Message + "\n");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private static void DownloadFile(string downloadPath, string outputPath)
        {
            // TODO: Catch any exceptions that may come up (permission errors, internet offline, etc)
            var client = new HttpClient();
            var res = client.GetAsync(downloadPath);
            res.Wait();
            var bytes = res.Result.Content.ReadAsByteArrayAsync();
            string filePath = outputPath;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            FileStream fileHandle = File.Create(filePath);
            bytes.Wait();
            fileHandle.Write(bytes.Result);
            fileHandle.Close();

            // File download complete
            double localIndex = Interlocked.CompareExchange(ref fileIndex, 0, 0);
            double progress = localIndex / DownloadCount * 100;

            string progressString = $"{localIndex}/{DownloadCount}";
            string currentFile = "Downloaded File: " + outputPath;
            SetCurrentProgress(progress, progressString, currentFile);

            Interlocked.Increment(ref fileIndex);
        }

        private static uint StringToFnvHash(string str)
        {
            const uint fnvPrime32 = 16777619;
            const uint fnvOffset32 = 2166136261;

            IEnumerable<byte> bytesToHash = str.ToCharArray().Select(Convert.ToByte);
            uint hash = fnvOffset32;

            foreach (var chunk in bytesToHash)
            {
                hash ^= chunk;
                hash *= fnvPrime32;
            }

            return hash;
        }

        public static async Task IntegrityCheck(Action<string> currentAction, Action<double, string, string> setProgress, CancellationTokenSource cts)
        {
            SetCurrentAction = currentAction;
            SetCurrentProgress = setProgress;

            ComputedHashes = new Dictionary<uint, KeyValuePair<string, byte[]>>();

            string fileDir = Settings.Instance.LauncherData.AftermathInstall;

            string[] ignoredDirs = new[] { "SAVES", "SCREENSHOTS", "TOOLS", ".GIT", ".GITLAB" };

            var checksums = (await WebRequest.GetRequest("/api/game/integrity")).Value.ToObject<Dictionary<uint, KeyValuePair<string, byte[]>>>();

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            SetCurrentAction("Checking file integrity...");

            double fileIndex;

            foreach (string dir in Directory.EnumerateDirectories(fileDir, "*", SearchOption.AllDirectories).Where(x => ignoredDirs.All(dir => !x.ToUpper().Contains(dir))))
            {
                fileIndex = 1;

                foreach (string file in Directory.GetFiles(dir))
                {
                    await using FileStream fs = new FileStream(file, FileMode.Open);
                    await using BufferedStream bs = new BufferedStream(fs, 10 * 1024);
                    using SHA1Managed sha1 = new SHA1Managed();

                    byte[] hash = sha1.ComputeHash(bs);
                    string relativeDir = Path.GetRelativePath(fileDir, file).Replace('\\', '/');
                    uint nameHash = StringToFnvHash(relativeDir);
                    try
                    {
                        ComputedHashes.Add(nameHash, new KeyValuePair<string, byte[]>(relativeDir, hash));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to add hash to list. Likely collision.\nHash: {nameHash}\nPath: {relativeDir}\nEx: {ex.Message}");
                    }

                    double progress = fileIndex / Directory.GetFiles(dir).Length * 100;
                    string progressString = $"{fileIndex}/{Directory.GetFiles(dir).Length}";
                    string currentFile = relativeDir;
                    SetCurrentProgress(progress, progressString, currentFile);

                    fileIndex++;
                }
            }

            SetCurrentAction("Downloading Files...");

            Dictionary<string, string> toDownload = new Dictionary<string, string>();

            foreach (var checksum in checksums)
            {
                Directory.CreateDirectory(fileDir + "/" + Path.GetDirectoryName(checksum.Value.Key));

                if (!File.Exists(fileDir + "/" + checksum.Value.Key))
                {
                    toDownload.Add(Settings.Instance.LauncherData.PatchServer + "/" + checksum.Value.Key, fileDir + "/" + checksum.Value.Key);
                }
                else
                {
                    if (ComputedHashes.Keys.Contains(checksum.Key))
                    {
                        var check = ComputedHashes[checksum.Key];
                        if (check.Value.SequenceEqual(checksum.Value.Value))
                        {
                            continue;
                        }
                    }

                    toDownload.Add(Settings.Instance.LauncherData.PatchServer + "/" + checksum.Value.Key, fileDir + "/" + checksum.Value.Key);
                }
            }

            foreach (var computedHash in ComputedHashes)
            {
                if (!checksums.Keys.Contains(computedHash.Key) || !checksums[computedHash.Key].Value.SequenceEqual(computedHash.Value.Value))
                {
                    string path = $"{fileDir}/{computedHash.Value.Key}";
                    File.Delete(path);
                }
            }

            DownloadFiles(toDownload, cts);

            void RemoveEmptyDirectories(string path)
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    RemoveEmptyDirectories(directory);
                    if (Directory.GetFileSystemEntries(directory).Length == 0)
                    {
                        Directory.Delete(directory, false);
                    }
                }
            }

            RemoveEmptyDirectories(fileDir);
        }
    }
}
