using CommandLine;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine.Verb;

/// <summary>
/// This class maintains the directory upload 
/// </summary>
[Verb("upload-file", HelpText = "Upload a file to your sftp account")]
public class UploadFileCommandLineVerb : CommandLineVerb
{
    /// <summary>
    /// This method asynchronously uploads the example file to your sftp account
    /// </summary>
    /// <returns>An awaitable task with the exit-code as the result</returns>
    public override async Task<int> ProcessAsync()
    {
        // Define our local file path to upload
        string localFilename = Path.Combine(DataPath, LocalUploadDirectory, LocalUploadFile);
        // Define our remote file path to upload
        string remoteFilename = Path.Combine(RemoteUploadDirectory, RemoteUploadFile);
        
        // Instantiate our SFTP client into a disposable context
        await using SftpClient client = new(Username, Key, Passphrase);
        
        // Upload the file
        await client.UploadFileAsync(localFilename, remoteFilename);
        
        // Check the skip-item-callbacks flag and send the message
        if (!SkipItemCallbacks) Console.WriteLine($"Uploaded:\t[{localFilename}] => {remoteFilename}");

        // We're done, return the exit-code
        return 0;
    }
}
