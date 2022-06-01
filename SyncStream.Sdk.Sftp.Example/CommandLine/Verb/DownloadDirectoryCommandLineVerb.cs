using CommandLine;
using Renci.SshNet.Sftp;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine.Verb;

/// <summary>
/// This class maintains our directory download verb
/// </summary>
[Verb("download-directory", HelpText = "Download a folder from your sftp account")]
public class DownloadDirectoryCommandLineVerb : CommandLineVerb
{
    /// <summary>
    /// This method cleans the download directory
    /// </summary>
    private void CleanDownloadDirectory()
    {
        // Instantiate our directory
        DirectoryInfo directory = new DirectoryInfo(Path.Combine(DataPath, LocalDownloadDirectory));
        
        // Iterate over the sub-directories and delete them
        foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories()) Directory.Delete(subDirectory.FullName);
        
        // Iterate over the files and delete them
        foreach (FileInfo file in directory.EnumerateFiles()) File.Delete(file.FullName);
    }

    /// <summary>
    /// This method asynchronously ensures that the example data has been uploaded
    /// </summary>
    /// <param name="client">The instantiated SFTP client library</param>
    /// <returns>An awaitable task with no result</returns>
    private async Task EnsureUploadedFiles(SftpClient client)
    {
        // List the remote directory
        List<SftpFile> files = await client.ListDirectoryAsync(RemoteUploadDirectory);
        
        // Check for files and upload them
        if (!files.Any())
        {
            // Send the message
            Console.Write("Uploading Example Data ... ");
            
            // Upload the example data
            await new UploadDirectoryCommandLineVerb().SkipCallbacks().ProcessAsync();
            
            // Send the message
            Console.WriteLine("Done!");
        }
    }
    
    /// <summary>
    /// This method asynchronously downloads the example directory to your sftp account
    /// </summary>
    /// <returns>An awaitable task with the exit-code as the result</returns>
    public override async Task<int> ProcessAsync()
    {

        // Clean the download directory
        CleanDownloadDirectory();
        
        // Instantiate our SFTP client into a disposable context
        await using SftpClient client = new(Username, Key, Passphrase);
        
        // Ensure files have been uploaded
        await EnsureUploadedFiles(client);

        // Download the files
        await client.DownloadDirectoryAsync(RemoteUploadDirectory, Path.Combine(DataPath, LocalDownloadDirectory),
            SkipItemCallbacks
                ? null
                : (item, path, remotePath) =>
                {
                    // Send the message
                    Console.WriteLine($"Downloaded:\t[{remotePath}] => {path}");

                    // We're done
                    return Task.CompletedTask;
                });
        
        // We're done, send the exit-code
        return 0;
    }
}
