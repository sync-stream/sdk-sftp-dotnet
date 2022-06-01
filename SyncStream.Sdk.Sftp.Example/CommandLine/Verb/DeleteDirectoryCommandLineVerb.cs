using CommandLine;
using Renci.SshNet.Sftp;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine.Verb;

/// <summary>
/// This class maintains our directory delete verb
/// </summary>
[Verb("delete-directory", HelpText = "Delete a folder from your sftp account")]
public class DeleteDirectoryCommandLineVerb : CommandLineVerb
{
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
    /// This method asynchronously deletes the uploaded example directory to your sftp account
    /// </summary>
    /// <returns>An awaitable task with the exit-code as the result</returns>
    public override async Task<int> ProcessAsync()
    {
        // Instantiate our SFTP client into a disposable context
        await using SftpClient client = new(Username, Key, Passphrase);

        // Ensure files have been uploaded
        await EnsureUploadedFiles(client);

        // Delete the files
        await client.DeleteDirectoryAsync(RemoteUploadDirectory, SkipItemCallbacks
            ? null
            : (item, path) =>
            {
                // Send the message
                Console.WriteLine($"Deleted:\t{path}");

                // We're done
                return Task.CompletedTask;
            });

        // We're done, send the exit-code
        return 0;
    }
}
