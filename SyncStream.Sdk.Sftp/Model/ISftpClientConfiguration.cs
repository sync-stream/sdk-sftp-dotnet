// Define our namespace
namespace SyncStream.Sdk.Sftp.Model;

/// <summary>
/// This interface maintains the model structure for the configuration settings of the SFTP client service provider
/// </summary>
public interface ISftpClientConfiguration
{
    /// <summary>
    /// This property denotes whether the service should auto-connect or not
    /// </summary>
    public bool AutoConnect { get; set; }

    /// <summary>
    /// This property contains the domain or IP address on which the SFTP server is listening
    /// </summary>
    public string Hostname { get; set; }
    
    /// <summary>
    /// This property contains the SFTP authentication password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// This property contains the port number on which the SFTP service is listening
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// This property contains the absolute path to the private key for authentication
    /// </summary>
    public string PrivateKey { get; set; }

    /// <summary>
    /// This property contains the passphrase to the private key for authentication
    /// </summary>
    public string PrivateKeyPassphrase { get; set; }

    /// <summary>
    /// This property contains the SFTP authentication username
    /// </summary>
    public string Username { get; set; }
}
