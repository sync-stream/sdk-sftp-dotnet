using Renci.SshNet.Sftp;
using SyncStream.Sdk.Sftp.Model;

// Define our namespace
namespace SyncStream.Sdk.Sftp;

/// <summary>
/// This interface maintains the structure of our SFTP client service provider
/// </summary>
public interface ISftpClient : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// This delegate defines the callback prototype for a deleted item
    /// </summary>
    /// <param name="item">The deleted item from the remote server</param>
    /// <param name="remotePath">The full path of the deleted item from the remote server</param>
    public delegate void ItemDeleteCallback(SftpFile item, string remotePath);

    /// <summary>
    /// This delegate defines the callback prototype for an asynchronously deleted item
    /// </summary>
    /// <param name="item">The deleted item from the remote server</param>
    /// <param name="remotePath">The full path of the deleted item from the remote server</param>
    public delegate Task ItemDeleteCallbackAsync(SftpFile item, string remotePath);

    /// <summary>
    /// This delegate defines the callback prototype for a downloaded item
    /// </summary>
    /// <param name="item">The file or directory that was downloaded</param>
    /// <param name="localPath">The file or directory's path on the local filesystem</param>
    /// <param name="remotePath">The file or directory's path on the remote filesystem</param>
    public delegate void ItemDownloadCallback(SftpFile item, string localPath, string remotePath);

    /// <summary>
    /// This delegate defines the callback prototype for an asynchronously downloaded item
    /// </summary>
    /// <param name="item">The file or directory that was downloaded</param>
    /// <param name="localPath">The file or directory's path on the local filesystem</param>
    /// <param name="remotePath">The file or directory's path on the remote filesystem</param>
    public delegate Task ItemDownloadCallbackAsync(SftpFile item, string localPath, string remotePath);

    /// <summary>
    /// This delegate defines the callback prototype for an uploaded item
    /// </summary>
    /// <param name="item">The file or directory that was uploaded</param>
    /// <param name="localPath">The file or directory's path on the local filesystem</param>
    /// <param name="remotePath">The file or directory's path on the remote filesystem</param>
    public delegate void ItemUploadCallback(FileSystemInfo item, string localPath, string remotePath);

    /// <summary>
    /// This delegate defines the callback prototype for an asynchronously uploaded item
    /// </summary>
    /// <param name="item">The file or directory that was uploaded</param>
    /// <param name="localPath">The file or directory's path on the local filesystem</param>
    /// <param name="remotePath">The file or directory's path on the remote filesystem</param>
    public delegate Task ItemUploadCallbackAsync(FileSystemInfo item, string localPath, string remotePath);

    /// <summary>
    /// This delegate defines the callback prototype for a directory listing
    /// </summary>
    public delegate void ListDirectoryCallback(List<SftpFile> files, int total);

    /// <summary>
    /// This delegate defines the callback prototype for an asynchronous directory listing
    /// </summary>
    public delegate Task ListDirectoryAsyncCallback(List<SftpFile> files, int total);

    /// <summary>
    /// This method establishes an SFTP connection
    /// </summary>
    /// <param name="configuration">An optional server connection configuration object override</param>
    /// <returns>The current instance of the service</returns>
    public ISftpClient Connect(ISftpClientConfiguration configuration = null);

    /// <summary>
    /// This method deletes multiple directories at one time
    /// </summary>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    public void DeleteDirectories(IEnumerable<string> remoteDirectories,
        ISftpClient.ItemDeleteCallback itemCallback = null);

    /// <summary>
    /// This method deletes multiple directories at one time
    /// </summary>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    public void DeleteDirectories(ISftpClient.ItemDeleteCallback itemCallback = null,
        params string[] remoteDirectories);

    /// <summary>
    /// This method asynchronously deletes multiple directories at one time
    /// </summary>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    public Task DeleteDirectoriesAsync(IEnumerable<string> remoteDirectories,
        ISftpClient.ItemDeleteCallbackAsync itemCallback = null);

    /// <summary>
    /// This method asynchronously deletes multiple directories at one time
    /// </summary>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    public Task DeleteDirectoriesAsync(ISftpClient.ItemDeleteCallbackAsync itemCallback = null,
        params string[] remoteDirectories);

    /// <summary>
    /// This method deletes the remote directory at <paramref name="remoteDirectory" /> from the server
    /// </summary>
    /// <param name="remoteDirectory">The remote path to the directory to delete from the server</param>
    /// <param name="itemCallback">The callback that is execute when an item in the directory is deleted</param>
    public void DeleteDirectory(string remoteDirectory, ItemDeleteCallback itemCallback = null);

    /// <summary>
    /// This method asynchronously deletes the remote directory at <paramref name="remoteDirectory" /> from the server
    /// </summary>
    /// <param name="remoteDirectory">The remote path to the directory to delete from the server</param>
    /// <param name="itemCallback">The callback that is execute when an item in the directory is deleted</param>
    public Task DeleteDirectoryAsync(string remoteDirectory, ItemDeleteCallbackAsync itemCallback = null);

    /// <summary>
    /// This method deletes the remote file at <paramref name="remoteFilename" /> from the server
    /// </summary>
    /// <param name="remoteFilename">The remote path to the file to delete from the server</param>
    public void DeleteFile(string remoteFilename);

    /// <summary>
    /// This method deletes multiple files at one time
    /// </summary>
    /// <param name="remoteFilenames">The list of remote paths to delete</param>
    public void DeleteFiles(IEnumerable<string> remoteFilenames);

    /// <summary>
    /// This method deletes multiple files at one time
    /// </summary>
    /// <param name="remoteFilenames">The list of remote paths to delete</param>
    public void DeleteFiles(params string[] remoteFilenames);

    /// <summary>
    /// This method asynchronously determines whether or not a directory exists
    /// </summary>
    /// <param name="remoteDirectory">The remote path to check</param>
    /// <returns>A boolean denoting whether or not the directory exists</returns>
    public bool DirectoryExists(string remoteDirectory);

    /// <summary>
    /// This method determines whether or not a directory exists
    /// </summary>
    /// <param name="remoteDirectory">The remote path to check</param>
    /// <returns>An awaitable task containing a boolean denoting whether or not the directory exists</returns>
    public Task<bool> DirectoryExistsAsync(string remoteDirectory);

    /// <summary>
    /// This method recursively downloads the remote directory <paramref name="remoteDirectory" /> to the local directory <paramref name="directory" />
    /// </summary>
    /// <param name="remoteDirectory">The path on the remote server to the directory to download</param>
    /// <param name="directory">The path on the local system to download the remote directory to</param>
    /// <param name="itemCallback">Optional callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="remoteDirectory" /> or <paramref name="directory" /> are empty</exception>
    public void DownloadDirectory(string remoteDirectory, string directory, ItemDownloadCallback itemCallback = null);

    /// <summary>
    /// This method recursively downloads the remote directory <paramref name="remoteDirectory" /> to the local directory <paramref name="directory" /> asynchronously
    /// </summary>
    /// <param name="remoteDirectory">The path on the remote server to the directory to download</param>
    /// <param name="directory">The path on the local system to download the remote directory to</param>
    /// <param name="itemCallback">Optional asynchronous callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="remoteDirectory" /> or <paramref name="directory" /> are empty</exception>
    public Task DownloadDirectoryAsync(string remoteDirectory, string directory,
        ItemDownloadCallbackAsync itemCallback = null);

    /// <summary>
    /// This method downloads the remote file from <paramref name="remoteFilename" /> to the local file <paramref name="filename" />
    /// </summary>
    /// <param name="remoteFilename">The path to the remote file on the server to download</param>
    /// <param name="filename">The path to the local file to save the remote file</param>
    public void DownloadFile(string remoteFilename, string filename);

    /// <summary>
    /// This method asynchronously downloads the remote file from <paramref name="remoteFilename" /> to the local file <paramref name="filename" />
    /// </summary>
    /// <param name="remoteFilename">The path to the remote file on the server to download</param>
    /// <param name="filename">The path to the local file to save the remote file</param>
    public Task DownloadFileAsync(string remoteFilename, string filename);

    /// <summary>
    /// This method determines whether or not a file exists
    /// </summary>
    /// <param name="remoteFilename">The remote file path to check</param>
    /// <returns>A boolean denoting whether or not the file exists</returns>
    public bool FileExists(string remoteFilename);

    /// <summary>
    /// This method asynchronously determines whether or not a file exists
    /// </summary>
    /// <param name="remoteFilename">The remote file path to check</param>
    /// <returns>An awaitable task containing a boolean denoting whether or not the file exists</returns>
    public Task<bool> FileExistsAsync(string remoteFilename);

    /// <summary>
    /// This method generates an internal SFTP client from a provided configuration object
    /// </summary>
    /// <param name="configuration">The server connection configuration object from which to generate a connection</param>
    /// <returns>A library-direct SFTP client connection</returns>
    /// <exception cref="Exception">Thrown when no password nor private-key are provided in the server connection configuration object</exception>
    public Renci.SshNet.SftpClient GetClient(ISftpClientConfiguration configuration);

    /// <summary>
    /// This method lists the directory <paramref name="path" /> and returns the files therein
    /// </summary>
    /// <param name="path">The remote folder path to list</param>
    /// <param name="callback">An optional callback that receives the directory listing</param>
    /// <returns>A list of files in the remote server directory</returns>
    public List<SftpFile> ListDirectory(string path, ListDirectoryCallback callback = null);

    /// <summary>
    /// This method asynchronously lists the directory <paramref name="path" /> and returns the files therein
    /// </summary>
    /// <param name="path">The remote folder path to list</param>
    /// <param name="callback">An optional callback that receives the directory listing</param>
    /// <returns>A list of files in the remote server directory</returns>
    public Task<List<SftpFile>> ListDirectoryAsync(string path, ListDirectoryAsyncCallback callback = null);

    /// <summary>
    /// This method creates a new directory on the remote server at <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="remoteDirectory">The path to the directory on the remote server</param>
    public void MakeDirectory(string remoteDirectory);

    /// <summary>
    /// This method asynchronously creates a new directory on the remote server at <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="remoteDirectory">The path to the directory on the remote server</param>
    /// <returns>An awaitable task with no result</returns>
    public Task MakeDirectoryAsync(string remoteDirectory);

    /// <summary>
    /// This method uploads the local directory <paramref name="directory" /> to the remote server as <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="directory">The local directory path to recursively upload</param>
    /// <param name="remoteDirectory">The remote directory path to upload to</param>
    /// <param name="overwrite">Denote whether or not files should be overwritten</param>
    /// <param name="itemCallback">Optional callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="directory" /> isn't a directory</exception>
    public void UploadDirectory(string directory, string remoteDirectory, bool overwrite = true,
        ItemUploadCallback itemCallback = null);

    /// <summary>
    /// This method asynchronously uploads the local directory <paramref name="directory" /> to the remote server as <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="directory">The local directory path to recursively upload</param>
    /// <param name="remoteDirectory">The remote directory path to upload to</param>
    /// <param name="overwrite">Denote whether or not files should be overwritten</param>
    /// <param name="itemCallback">Optional asynchronous callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="directory" /> isn't a directory</exception>
    public Task UploadDirectoryAsync(string directory, string remoteDirectory, bool overwrite = true,
        ItemUploadCallbackAsync itemCallback = null);

    /// <summary>
    /// This method uploads the local file <paramref name="filename" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="filename">The path to the local file to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    public void UploadFile(string filename, string remoteFilename, bool overwrite = true);

    /// <summary>
    /// This method uploads the local file stream <paramref name="file" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="file">The local file stream to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    public void UploadFile(FileStream file, string remoteFilename, bool overwrite = true);

    /// <summary>
    /// This method asynchronously uploads the local file <paramref name="filename" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="filename">The path to the local file to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    /// <returns>An awaitable task with void result</returns>
    public Task UploadFileAsync(string filename, string remoteFilename, bool overwrite = true);

    /// <summary>
    /// This method asynchronously uploads the local file stream <paramref name="file" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="file">The local file stream to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    /// <returns>An awaitable task with void result</returns>
    public Task UploadFileAsync(FileStream file, string remoteFilename, bool overwrite = true);

    /// <summary>
    /// This method fluidly resets the internal client into the instance
    /// </summary>
    /// <param name="client">The built internal client instance</param>
    /// <returns>The current instance of the service</returns>
    public ISftpClient WithClient(Renci.SshNet.SftpClient client);

    /// <summary>
    /// This method fluidly resets the configuration into the instance
    /// </summary>
    /// <param name="configuration">The server connection configuration object</param>
    /// <param name="skipAutoConnect">Denotes whether to automatically establish a connection to the server, regardless of the server connection configuration object values</param>
    /// <returns>The current instance of the service</returns>
    public ISftpClient WithConfiguration(ISftpClientConfiguration configuration, bool skipAutoConnect = false);
}
