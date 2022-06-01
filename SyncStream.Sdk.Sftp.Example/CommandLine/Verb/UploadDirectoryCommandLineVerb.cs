using CommandLine;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine.Verb;

/// <summary>
/// This class maintains the directory upload 
/// </summary>
[Verb("upload-directory", HelpText = "Upload a directory to your sftp account")]
public class UploadDirectoryCommandLineVerb : CommandLineVerb
{
    /// <summary>
    /// This method asynchronously uploads the example directory to your sftp account
    /// </summary>
    /// <returns>An awaitable task with the exit-code as the result</returns>
    public override async Task<int> ProcessAsync()
    {
        // Instantiate our SFTP client into a disposable context
        await using SftpClient client = new(Username, Key, Passphrase);
        
        // Upload the directory
        await client.UploadDirectoryAsync(Path.Combine(DataPath, LocalUploadDirectory), RemoteUploadDirectory, true,
            SkipItemCallbacks
                ? null
                : (item, path, remotePath) =>
                {
                    // Send the message
                    Console.WriteLine($"Uploaded:\t[{path}] => {remotePath}");

                    // We're done
                    return Task.CompletedTask;
                });
        
        // We're done, return the exit-code
        return 0;
    }
}
