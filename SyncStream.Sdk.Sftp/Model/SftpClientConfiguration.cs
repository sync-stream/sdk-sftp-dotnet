// Define our namespace
namespace SyncStream.Sdk.Sftp.Model;

/// <summary>
/// This class maintains the model structure for the configuration settings of the SFTP client service provider
/// </summary>
public class SftpClientConfiguration : ISftpClientConfiguration
{
    /// <summary>
    /// This property denotes whether the service should auto-connect or not
    /// </summary>
    public bool AutoConnect { get; set; } = true;

    /// <summary>
    /// This property contains the domain or IP address on which the SFTP server is listening
    /// </summary>
    public string Hostname { get; set; } = "ftp.sync-stream.com";
    
    /// <summary>
    /// This property contains the SFTP authentication password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// This property contains the port number on which the SFTP service is listening
    /// </summary>
    public int Port { get; set; } = 22;

    /// <summary>
    /// This property contains the absolute path to the private key for authentication
    /// </summary>
    public string PrivateKey { get; set; }

    /// <summary>
    /// This property contains the passphrase to the private key for authentication
    /// </summary>
    public string PrivateKeyPassphrase { get; set; } = string.Empty;

    /// <summary>
    /// This property contains the SFTP authentication username
    /// </summary>
    public string Username { get; set; } = string.Empty;
}
