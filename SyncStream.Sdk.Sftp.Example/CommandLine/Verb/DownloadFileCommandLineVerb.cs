using CommandLine;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine.Verb;

/// <summary>
/// This class maintains our file download verb
/// </summary>
[Verb("download-file", HelpText = "Download a file from your sftp account")]
public class DownloadFileCommandLineVerb : CommandLineVerb
{
    /// <summary>
    /// This method cleans the download directory
    /// </summary>
    private void CleanDownloadDirectory()
    {
        // Define our filename
        string filename = Path.Combine(DataPath, LocalDownloadDirectory, LocalDownloadFile);
        
        // Check to see if the file exists and delete it
        if (File.Exists(filename)) File.Delete(filename);
    }

    /// <summary>
    /// This method asynchronously ensures that the example data has been uploaded
    /// </summary>
    /// <param name="client">The instantiated SFTP client library</param>
    /// <returns>An awaitable task with no result</returns>
    private async Task EnsureUploadedFiles(SftpClient client)
    {
        // Check for files and upload them
        if (!await client.FileExistsAsync(Path.Combine(RemoteUploadDirectory, RemoteUploadFile)))
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
    /// This method asynchronously downloads the example file to your sftp account
    /// </summary>
    /// <returns>An awaitable task with the exit-code as the result</returns>
    public override async Task<int> ProcessAsync()
    {
        // Clean the download directory
        CleanDownloadDirectory();
        
        // Define our local file path to upload
        string localFilename = Path.Combine(DataPath, LocalDownloadDirectory, LocalDownloadFile);
        // Define our remote file path to upload
        string remoteFilename = Path.Combine(RemoteUploadDirectory, RemoteUploadFile);
        
        // Instantiate our SFTP client into a disposable context
        await using SftpClient client = new(Username, Key, Passphrase);
        
        // Ensure files have been uploaded
        await EnsureUploadedFiles(client);

        // Download the file
        await client.DownloadFileAsync(remoteFilename, localFilename);
        
        // Check the skip-item-callbacks flag and send the message
        if (!SkipItemCallbacks) Console.WriteLine($"Downloaded:\t[{remoteFilename}] => {localFilename}");
        
        // We're done, send the exit-code
        return 0;
    }
}
