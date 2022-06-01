using System.Reflection;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine;

/// <summary>
/// This class maintains the structure of our command-line verbs
/// </summary>
public abstract class CommandLineVerb : CommandLineOptions, ICommandLineVerb
{
    /// <summary>
    /// This property contains the absolute path to the example data directory
    /// </summary>
    protected string DataPath =>
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Data");

    /// <summary>
    /// This property denotes whether or not item callbacks should be skipped
    /// </summary>
    protected bool SkipItemCallbacks;

    /// <summary>
    /// This method asynchronously processes the command-line verb
    /// </summary>
    /// <returns>An awaitable task with the exit-code as the result</returns>
    public abstract Task<int> ProcessAsync();

    /// <summary>
    /// This method fluidly resets the skip-item-callbacks flag into the instance
    /// </summary>
    /// <param name="flag">Optional flag value</param>
    /// <returns>The current instance of the verb</returns>
    public ICommandLineVerb SkipCallbacks(bool flag = true)
    {
        // Reset the skip-item-callbacks flag into the instance
        SkipItemCallbacks = flag;
        
        // We're done, return the instance
        return this;
    }
}
