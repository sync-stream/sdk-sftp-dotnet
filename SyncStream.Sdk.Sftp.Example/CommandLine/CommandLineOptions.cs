using CommandLine;

// Define our namespace
namespace SyncStream.Sdk.Sftp.Example.CommandLine;

/// <summary>
/// This class maintains the structure of our command line options
/// </summary>
public class CommandLineOptions
{
    /// <summary>
    /// This constant defines the relative path of the local download directory
    /// </summary>
    public const string LocalDownloadDirectory = "download-directory";

    /// <summary>
    /// This constant defines the relative path of the local download file
    /// </summary>
    public const string LocalDownloadFile = "downloaded-1.txt";

    /// <summary>
    /// This constant defines the relative path of the local directory to upload
    /// </summary>
    public const string LocalUploadDirectory = "upload-directory";

    /// <summary>
    /// This constant defines the relative path of the local file to upload
    /// </summary>
    public const string LocalUploadFile = "1.txt";

    /// <summary>
    /// This constant defines the relative path of the remote upload directory
    /// </summary>
    public const string RemoteUploadDirectory = "uploaded-example-data";

    /// <summary>
    /// This constant defines the relative path of the remote upload file
    /// </summary>
    public const string RemoteUploadFile = "uploaded-1.txt";
    
    /// <summary>
    /// This property contains the path to the authentication private key
    /// </summary>
    [Option("key", Required = true, HelpText = "The path to the authentication private key")]
    public string Key { get; set; }
    
    /// <summary>
    /// This property contains the passphrase for the authentication private key
    /// </summary>
    [Option("passphrase", Required = false, HelpText = "Optional passphrase with which to decrypt the authentication private key")]
    public string Passphrase { get; set; }
    
    /// <summary>
    /// This property contains the authentication username
    /// </summary>
    [Option("username", Required = true, HelpText = "The authentication username")]
    public string Username { get; set; }
}
