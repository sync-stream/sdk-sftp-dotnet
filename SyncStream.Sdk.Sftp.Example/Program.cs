using CommandLine;
using SyncStream.Sdk.Sftp.Example.CommandLine.Verb;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example;

/// <summary>
/// This class maintains the ingress into our console application and main event loop
/// </summary>
public static class Program
{
    /// <summary>
    /// This method provides our asynchronous main event loop
    /// </summary>
    /// <param name="arguments">The arguments provided on the command-line</param>
    /// <returns>An awaitable task containing an integer exit-code</returns>
    public static Task<int> Main(string[] arguments) =>

        // Setup our command-line parser
        Parser.Default

            // Parse the arguments into our options
            .ParseArguments<DeleteDirectoryCommandLineVerb, DeleteFileCommandLineVerb, DownloadDirectoryCommandLineVerb,
                DownloadFileCommandLineVerb, UploadDirectoryCommandLineVerb, UploadFileCommandLineVerb>(arguments)

            // Map the results to our verbs
            .MapResult(

                // Register our delete-directory verb
                (DeleteDirectoryCommandLineVerb v) => v.ProcessAsync(),

                // Register our delete-file verb
                (DeleteFileCommandLineVerb v) => v.ProcessAsync(),

                // Register our download-directory verb
                (DownloadDirectoryCommandLineVerb v) => v.ProcessAsync(),

                // Register our download-file verb
                (DownloadFileCommandLineVerb v) => v.ProcessAsync(),

                // Register our upload-directory verb
                (UploadDirectoryCommandLineVerb v) => v.ProcessAsync(),

                // Register our upload-file verb
                (UploadFileCommandLineVerb v) => v.ProcessAsync(),

                // Register our error handler
                errors =>
                {
                    // Iterate over the errors and log them
                    foreach (Error error in errors)
                    {
                        // Buffer the console space
                        Console.WriteLine("");
                        
                        // Send the error to console
                        Console.Write(error);
                        
                        // Buffer the console space
                        Console.WriteLine("\n");
                    }

                    // We're done
                    return Task.FromResult(1);
                }
            );
}
