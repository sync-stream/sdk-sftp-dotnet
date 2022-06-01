// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine;

/// <summary>
/// This interface maintains the structure of our command-line verbs
/// </summary>
public interface ICommandLineVerb
{
    /// <summary>
    /// This method asynchronously processes the command-line verb
    /// </summary>
    /// <returns>An awaitable task with the exit-code as the result</returns>
    public Task<int> ProcessAsync();

    /// <summary>
    /// This method fluidly resets the skip-item-callbacks flag into the instance
    /// </summary>
    /// <param name="flag">Optional flag value</param>
    /// <returns>The current instance of the verb</returns>
    public ICommandLineVerb SkipCallbacks(bool flag = true);
}
