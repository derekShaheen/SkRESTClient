﻿namespace WickerREST
{
    internal static class Utilities
    {
        internal static async System.Threading.Tasks.Task EnsureFileExists(string filePath, string url, bool isAsync = true, bool isBinary = false)
        {
            if (!File.Exists(filePath))
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        if (isBinary)
                        {
                            var response = await httpClient.GetAsync(url);
                            if (response.IsSuccessStatusCode)
                            {
                                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                                if(isAsync)
                                {
                                    await File.WriteAllBytesAsync(filePath, contentBytes); 
                                }
                                else { 
                                    File.WriteAllBytes(filePath, contentBytes);
                                }
                                WickerServer.Instance.LogMessage($"Downloaded binary file to {filePath}", 1);
                            }
                            else
                            {
                                WickerServer.Instance.LogMessage($"Failed to download binary file. Status code: {response.StatusCode}", 1);
                            }
                        }
                        else
                        {
                            var contentString = await httpClient.GetStringAsync(url);
                            if (isAsync)
                            {
                                await File.WriteAllTextAsync(filePath, contentString);
                            }
                            else
                            {
                                File.WriteAllText(filePath, contentString);
                            }
                            WickerServer.Instance.LogMessage($"Downloaded text file to {filePath}", 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WickerServer.Instance.LogMessage($"Exception during file download: {ex.Message}", 1);
                }
            }
        }

        public static string EnsureUniqueKey(IEnumerable<string> existingKeys, string originalKey)
        {
            string uniqueKey = originalKey;
            int counter = 2;
            while (existingKeys.Contains(uniqueKey))
            {
                uniqueKey = $"{originalKey}_{counter}";
                counter++;
            }
            return uniqueKey;
        }
    }
}
