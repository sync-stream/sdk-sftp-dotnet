using Renci.SshNet;
using Renci.SshNet.Sftp;
using SyncStream.Sdk.Sftp.Model;

// Define our namespace
namespace SyncStream.Sdk.Sftp;

/// <summary>
/// This class maintains the structure of our SFTP client service provider
/// </summary>
public class SftpClient : ISftpClient
{
    /// <summary>
    /// This property contains the instance of our SFTP client
    /// </summary>
    protected Renci.SshNet.SftpClient Client;

    /// <summary>
    /// This property contains the SFTP connection configuration
    /// </summary>
    protected ISftpClientConfiguration Configuration = new SftpClientConfiguration();

    /// <summary>
    /// This method instantiates our client service with a configuration
    /// </summary>
    /// <param name="configuration">The default server connection configuration object</param>
    public SftpClient(ISftpClientConfiguration configuration = null)
    {

        // Check for a provided configuration
        if (configuration is not null) WithConfiguration(configuration);
    }

    /// <summary>
    /// This method instantiates our client service with a hostname, username, password and optional port
    /// </summary>
    /// <param name="hostname">The domain or IP address on which the SFTP server listens</param>
    /// <param name="username">The authentication username</param>
    /// <param name="password">The authentication password</param>
    /// <param name="port">Optional port on which the SFTP server listens</param>
    public SftpClient(string hostname, string username, string password, int port = 22) : this(
        new SftpClientConfiguration
        {
            // Set the hostname into the server connection configuration
            Hostname = hostname,
            // Set the authentication password into the server connection configuration
            Password = password,
            // Set the port into the server connection configuration
            Port = port,
            // Set the authentication username into the server connection configuration
            Username = username
        })
    {
    }

    /// <summary>
    /// This method instantiates our client service with a hostname, username, private key path, optional port and optional private key passphrase
    /// </summary>
    /// <param name="hostname">The domain or IP address on which the SFTP server listens</param>
    /// <param name="username">The authentication username</param>
    /// <param name="privateKeyPath">The path to the authentication private key</param>
    /// <param name="port">Optional port on which the SFTP server listens</param>
    /// <param name="privateKeyPassphrase">Optional passphrase to decrypt the authentication private key</param>
    public SftpClient(string hostname, string username, string privateKeyPath, int port = 22,
        string privateKeyPassphrase = null) : this(new SftpClientConfiguration
    {
        // Set the hostname into the server connection configuration
        Hostname = hostname,
        // Set the port into the server connection configuration
        Port = port,
        // Set the authentication private key path into the server connection configuration
        PrivateKey = privateKeyPath,
        // Set the authentication private key passphrase into the server connection configuration
        PrivateKeyPassphrase = privateKeyPassphrase,
        // Set the authentication username into the server connection configuration 
        Username = username
    })
    {
    }

    /// <summary>
    /// This method instantiates our client service with a username and password
    /// </summary>
    /// <param name="username">The authentication username</param>
    /// <param name="password">The authentication password</param>
    public SftpClient(string username, string password) : this(new SftpClientConfiguration
    {
        // Set the authentication password into the server connection configuration
        Password = password,
        // Set the authentication username into the server connection configuration
        Username = username
    })
    {
    }

    /// <summary>
    /// This method instantiates our client service with a username, private key path and optional private key passphrase
    /// </summary>
    /// <param name="username">The authentication username</param>
    /// <param name="privateKeyPath">The path to the authentication private key</param>
    /// <param name="privateKeyPassphrase">Optional passphrase to decrypt the authentication private key</param>
    public SftpClient(string username, string privateKeyPath, string privateKeyPassphrase = null) : this(
        new SftpClientConfiguration
        {
            // Set the authentication private key path into the server connection configuration
            PrivateKey = privateKeyPath,
            // Set the authentication private key passphrase into the server connection configuration
            PrivateKeyPassphrase = privateKeyPassphrase,
            // Set the authentication username into the server connection configuration
            Username = username
        })
    {
    }

    /// <summary>
    /// This method establishes an SFTP connection
    /// </summary>
    /// <param name="configuration">An optional server connection configuration object override</param>
    /// <returns>The current instance of the service</returns>
    public ISftpClient Connect(ISftpClientConfiguration configuration = null)
    {
        // Check for a provided configuration
        if (configuration is not null) WithConfiguration(configuration, true);

        // Ensure we have an internal client
        if (Client is null) WithClient(GetClient(Configuration));

        // Establish the connection
        Client.Connect();

        // We're done, return the instance
        return this;
    }

    /// <summary>
    /// This method deletes multiple directories at one time
    /// </summary>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    public void DeleteDirectories(IEnumerable<string> remoteDirectories,
        ISftpClient.ItemDeleteCallback itemCallback = null)
    {

        // Iterate over the directories and delete them
        foreach (string remoteDirectory in remoteDirectories) DeleteDirectory(remoteDirectory, itemCallback);
    }

    /// <summary>
    /// This method deletes multiple directories at one time
    /// </summary>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    public void DeleteDirectories(ISftpClient.ItemDeleteCallback itemCallback = null,
        params string[] remoteDirectories) =>
        DeleteDirectories(remoteDirectories.ToList(), itemCallback);

    /// <summary>
    /// This method asynchronously deletes multiple directories at one time
    /// </summary>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    public Task DeleteDirectoriesAsync(IEnumerable<string> remoteDirectories,
        ISftpClient.ItemDeleteCallbackAsync itemCallback = null)
    {
        // Define our task list
        List<Task> tasks = new();

        // Iterate over the directories and delete them
        foreach (string remoteDirectory in remoteDirectories)
            tasks.Add(DeleteDirectoryAsync(remoteDirectory, itemCallback));

        // We're done, return the awaitable tasks
        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// This method asynchronously deletes multiple directories at one time
    /// </summary>
    /// <param name="itemCallback">Optional callback to execute when a directory or file is deleted</param>
    /// <param name="remoteDirectories">The list of remote paths to delete</param>
    public Task DeleteDirectoriesAsync(ISftpClient.ItemDeleteCallbackAsync itemCallback = null,
        params string[] remoteDirectories) =>
        DeleteDirectoriesAsync(remoteDirectories.ToList(), itemCallback);

    /// <summary>
    /// This method deletes the remote directory at <paramref name="remoteDirectory" /> from the server
    /// </summary>
    /// <param name="remoteDirectory">The remote path to the directory to delete from the server</param>
    /// <param name="itemCallback">The callback that is execute when an item in the directory is deleted</param>
    public void DeleteDirectory(string remoteDirectory, ISftpClient.ItemDeleteCallback itemCallback = null)
    {
        // Ensure we have a remote directory path
        if (string.IsNullOrEmpty(remoteDirectory) || string.IsNullOrWhiteSpace(remoteDirectory))
            throw new ArgumentNullException(nameof(remoteDirectory));

        // List the contents of the remote directory
        List<SftpFile> contents = ListDirectory(remoteDirectory);

        // Iterate over the contents of the remote directory
        foreach (SftpFile item in contents.Where(i => !i.FullName.EndsWith(".") && !i.FullName.EndsWith("..")))
        {
            // Check for a directory and download it
            if (item.IsDirectory)
            {
                // Delete the directory
                DeleteDirectory(item.FullName, itemCallback);

                // Execute our callback
                itemCallback?.Invoke(item, item.FullName);
            }
            else
            {

                // Delete the file
                DeleteFile(item.FullName);

                // Execute our callback
                itemCallback?.Invoke(item, item.FullName);
            }
        }

        // Delete the main directory
        Client.DeleteDirectory(remoteDirectory);
    }

    /// <summary>
    /// This method asynchronously deletes the remote directory at <paramref name="remoteDirectory" /> from the server
    /// </summary>
    /// <param name="remoteDirectory">The remote path to the directory to delete from the server</param>
    /// <param name="itemCallback">The callback that is execute when an item in the directory is deleted</param>
    public async Task DeleteDirectoryAsync(string remoteDirectory,
        ISftpClient.ItemDeleteCallbackAsync itemCallback = null)
    {
        // Ensure we have a remote directory path
        if (string.IsNullOrEmpty(remoteDirectory) || string.IsNullOrWhiteSpace(remoteDirectory))
            throw new ArgumentNullException(nameof(remoteDirectory));

        // List the contents of the remote directory
        List<SftpFile> contents = await ListDirectoryAsync(remoteDirectory);

        // Iterate over the contents of the remote directory
        foreach (SftpFile item in contents.Where(i => !i.FullName.EndsWith(".") && !i.FullName.EndsWith("..")))
        {
            // Check for a directory and download it
            if (item.IsDirectory)
            {
                // Delete the directory
                await DeleteDirectoryAsync(item.FullName, itemCallback);

                // Check for a callback and execute it
                if (itemCallback is not null) await itemCallback.Invoke(item, item.FullName);
            }
            else
            {

                // Delete the file
                DeleteFile(item.FullName);

                // Check for a callback and execute it
                if (itemCallback is not null) await itemCallback.Invoke(item, item.FullName);
            }
        }

        // Delete the main directory
        Client.DeleteDirectory(remoteDirectory);
    }

    /// <summary>
    /// This method deletes the remote file at <paramref name="remoteFilename" /> from the server
    /// </summary>
    /// <param name="remoteFilename">The remote path to the file to delete from the server</param>
    public void DeleteFile(string remoteFilename)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // We're done, delete the file from the server
        Client.DeleteFile(remoteFilename);
    }

    /// <summary>
    /// This method deletes multiple files at one time
    /// </summary>
    /// <param name="remoteFilenames">The list of remote paths to delete</param>
    public void DeleteFiles(IEnumerable<string> remoteFilenames)
    {

        // Iterate over the files
        foreach (string remoteFilename in remoteFilenames) DeleteFile(remoteFilename);
    }

    /// <summary>
    /// This method deletes multiple files at one time
    /// </summary>
    /// <param name="remoteFilenames">The list of remote paths to delete</param>
    public void DeleteFiles(params string[] remoteFilenames) =>
        DeleteFiles(remoteFilenames.ToList());

    /// <summary>
    /// This method determines whether or not a directory exists
    /// </summary>
    /// <param name="remoteDirectory">The remote path to check</param>
    /// <returns>A boolean denoting whether or not the directory exists</returns>
    public bool DirectoryExists(string remoteDirectory)
    {
        // Try to list the directory
        try
        {
            // List the directory
            ListDirectory(remoteDirectory);

            // We're done, the directory exists
            return true;
        }
        catch (Exception)
        {
            // We're done, the directory doesn't exist
            return false;
        }
    }

    /// <summary>
    /// This method asynchronously determines whether or not a directory exists
    /// </summary>
    /// <param name="remoteDirectory">The remote path to check</param>
    /// <returns>An awaitable task containing a boolean denoting whether or not the directory exists</returns>
    public async Task<bool> DirectoryExistsAsync(string remoteDirectory)
    {
        // Try to list the directory
        try
        {
            // List the directory
            await ListDirectoryAsync(remoteDirectory);

            // We're done, the directory exists
            return true;
        }
        catch (Exception)
        {
            // We're done, the directory doesn't exist
            return false;
        }
    }

    /// <summary>
    /// This method disposes of the instance
    /// </summary>
    public void Dispose()
    {
        // Disconnect the client
        Client?.Disconnect();
        // Dispose of the client
        Client?.Dispose();
    }

    /// <summary>
    /// This method asynchronously disposes of the instance
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
        // Disconnect the client
        Client?.Disconnect();
        // Dispose of the client
        Client?.Dispose();

        // We're done, return the completed task
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// This method recursively downloads the remote directory <paramref name="remoteDirectory" /> to the local directory <paramref name="directory" />
    /// </summary>
    /// <param name="remoteDirectory">The path on the remote server to the directory to download</param>
    /// <param name="directory">The path on the local system to download the remote directory to</param>
    /// <param name="itemCallback">Optional callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="remoteDirectory" /> or <paramref name="directory" /> are empty</exception>
    public void DownloadDirectory(string remoteDirectory, string directory,
        ISftpClient.ItemDownloadCallback itemCallback = null)
    {
        // Ensure we have a remote directory path
        if (string.IsNullOrEmpty(remoteDirectory) || string.IsNullOrWhiteSpace(remoteDirectory))
            throw new ArgumentNullException(nameof(remoteDirectory));

        // Ensure we have a directory path
        if (string.IsNullOrEmpty(directory) || string.IsNullOrWhiteSpace(directory))
            throw new ArgumentNullException(nameof(directory));

        // Ensure the directory exists
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // List the contents of the remote directory
        List<SftpFile> contents = ListDirectory(remoteDirectory);

        // Iterate over the contents of the remote directory
        foreach (SftpFile item in contents.Where(i => !i.FullName.EndsWith(".") && !i.FullName.EndsWith("..")))
        {
            // Check for a directory and download it
            if (item.IsDirectory)
            {
                // Localize the local directory name
                string localDirectory = $"{directory}{Path.DirectorySeparatorChar}{item.Name}";

                // Ensure the directory exists locally
                if (!Directory.Exists(localDirectory)) Directory.CreateDirectory(localDirectory);

                // Download the directory
                DownloadDirectory(item.FullName, localDirectory, itemCallback);

                // Execute our callback
                itemCallback?.Invoke(item, localDirectory, item.FullName);
            }
            else
            {
                // Localize the local filename
                string filename = $"{directory}{Path.DirectorySeparatorChar}{item.Name}";

                // Download the file
                DownloadFile(item.FullName, filename);

                // Execute our callback
                itemCallback?.Invoke(item, filename, item.FullName);
            }
        }
    }

    /// <summary>
    /// This method recursively downloads the remote directory <paramref name="remoteDirectory" /> to the local directory <paramref name="directory" /> asynchronously
    /// </summary>
    /// <param name="remoteDirectory">The path on the remote server to the directory to download</param>
    /// <param name="directory">The path on the local system to download the remote directory to</param>
    /// <param name="itemCallback">Optional asynchronous callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="remoteDirectory" /> or <paramref name="directory" /> are empty</exception>
    public async Task DownloadDirectoryAsync(string remoteDirectory, string directory,
        ISftpClient.ItemDownloadCallbackAsync itemCallback = null)
    {
        // Ensure we have a remote directory path
        if (string.IsNullOrEmpty(remoteDirectory) || string.IsNullOrWhiteSpace(remoteDirectory))
            throw new ArgumentNullException(nameof(remoteDirectory));

        // Ensure we have a directory path
        if (string.IsNullOrEmpty(directory) || string.IsNullOrWhiteSpace(directory))
            throw new ArgumentNullException(nameof(directory));

        // Ensure the directory exists
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // List the contents of the remote directory
        List<SftpFile> contents = await ListDirectoryAsync(remoteDirectory);

        // Iterate over the contents of the remote directory
        foreach (SftpFile item in contents.Where(i => !i.FullName.EndsWith(".") && !i.FullName.EndsWith("..")))
        {
            // Check for a directory and download it
            if (item.IsDirectory)
            {
                // Localize the local directory name
                string localDirectory = $"{directory}{Path.DirectorySeparatorChar}{item.Name}";

                // Ensure the directory exists locally
                if (!Directory.Exists(localDirectory)) Directory.CreateDirectory(localDirectory);

                // Download the directory
                await DownloadDirectoryAsync(item.FullName, localDirectory, itemCallback);

                // Check for a callback and execute it
                if (itemCallback is not null) await itemCallback.Invoke(item, localDirectory, item.FullName);
            }
            else
            {
                // Localize the local filename
                string filename = $"{directory}{Path.DirectorySeparatorChar}{item.Name}";

                // Download the file
                await DownloadFileAsync(item.FullName, filename);

                // Check for a callback and execute it
                if (itemCallback is not null) await itemCallback.Invoke(item, filename, item.FullName);
            }
        }
    }

    /// <summary>
    /// This method downloads the remote file from <paramref name="remoteFilename" /> to the local file <paramref name="filename" />
    /// </summary>
    /// <param name="remoteFilename">The path to the remote file on the server to download</param>
    /// <param name="filename">The path to the local file to save the remote file</param>
    public void DownloadFile(string remoteFilename, string filename)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Create our new file
        FileStream file = File.Create(filename);

        // Download the remote file from the server
        Client.DownloadFile(remoteFilename, file);

        // We're done, flush the file to disk
        file.Flush();

        // We're done with the file, dispose of it
        file.Dispose();
    }

    /// <summary>
    /// This method asynchronously downloads the remote file from <paramref name="remoteFilename" /> to the local file <paramref name="filename" />
    /// </summary>
    /// <param name="remoteFilename">The path to the remote file on the server to download</param>
    /// <param name="filename">The path to the local file to save the remote file</param>
    public async Task DownloadFileAsync(string remoteFilename, string filename)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Create our new file
        FileStream file = File.Create(filename);

        // Download the remote file from the server
        await Task.Factory.FromAsync(Client.BeginDownloadFile(remoteFilename, file, null), Client.EndDownloadFile);

        // We're done, flush the file to disk
        await file.FlushAsync();

        // We're done with the file, dispose of it
        await file.DisposeAsync().AsTask();
    }

    /// <summary>
    /// This method determines whether or not a file exists
    /// </summary>
    /// <param name="remoteFilename">The remote file path to check</param>
    /// <returns>A boolean denoting whether or not the file exists</returns>
    public bool FileExists(string remoteFilename)
    {
        // Try to download the file
        try
        {
            // Define our filename
            string filename = Path.GetTempFileName();

            // Download the file
            DownloadFile(remoteFilename, filename);

            // Delete the file
            File.Delete(filename);

            // We're done, the directory exists
            return true;
        }
        catch (Exception)
        {
            // We're done, the directory doesn't exist
            return false;
        }
    }

    /// <summary>
    /// This method asynchronously determines whether or not a file exists
    /// </summary>
    /// <param name="remoteFilename">The remote file path to check</param>
    /// <returns>An awaitable task containing a boolean denoting whether or not the file exists</returns>
    public async Task<bool> FileExistsAsync(string remoteFilename)
    {
        // Try to download the file
        try
        {
            // Define our filename
            string filename = Path.GetTempFileName();

            // Download the file
            await DownloadFileAsync(remoteFilename, filename);

            // Delete the file
            File.Delete(filename);

            // We're done, the directory exists
            return true;
        }
        catch (Exception)
        {
            // We're done, the directory doesn't exist
            return false;
        }
    }

    /// <summary>
    /// This method generates an internal SFTP client from a provided configuration object
    /// </summary>
    /// <param name="configuration">The server connection configuration object from which to generate a connection</param>
    /// <returns>A library-direct SFTP client connection</returns>
    /// <exception cref="Exception">Thrown when no password nor private-key are provided in the server connection configuration object</exception>
    public Renci.SshNet.SftpClient GetClient(ISftpClientConfiguration configuration)
    {
        // Check for a password or private key in the configuration
        if ((string.IsNullOrEmpty(configuration.Password) || string.IsNullOrWhiteSpace(configuration.Password)) &&
            (string.IsNullOrEmpty(configuration.PrivateKey) || string.IsNullOrWhiteSpace(configuration.PrivateKey)))
            throw new Exception("A Password or PrivateKey must me provided in the configuration for Authentication.");

        // Instantiate our connection information
        ConnectionInfo connection;

        // Check for a private key
        if (!string.IsNullOrEmpty(configuration.PrivateKey) && !string.IsNullOrWhiteSpace(configuration.PrivateKey))
        {
            // Setup private key authentication for the connection
            connection = new(configuration.Hostname, configuration.Port, configuration.Username,
                new PrivateKeyAuthenticationMethod(Configuration.Username,
                    new PrivateKeyFile(configuration.PrivateKey, configuration.PrivateKeyPassphrase)));
        }
        else
        {
            // Setup password authentication for the connection
            connection = new(configuration.Hostname, configuration.Port, configuration.Username,
                new PasswordAuthenticationMethod(configuration.Username, configuration.Password));
        }

        // We're done, instantiate and return our client
        return new(connection);
    }

    /// <summary>
    /// This method lists the directory <paramref name="path" /> and returns the files therein
    /// </summary>
    /// <param name="path">The remote folder path to list</param>
    /// <param name="callback">An optional callback that receives the directory listing</param>
    /// <returns>A list of files in the remote server directory</returns>
    public List<SftpFile> ListDirectory(string path, ISftpClient.ListDirectoryCallback callback = null)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Define our total container
        int total = 0;

        // We're done, list the directory and return the results
        IEnumerable<SftpFile> files = Client.ListDirectory(path, i => total = i).ToList();

        // We're done, execute the callback
        callback?.Invoke(files.ToList(), total);

        // We're done, return the files
        return files.ToList();
    }

    /// <summary>
    /// This method asynchronously lists the directory <paramref name="path" /> and returns the files therein
    /// </summary>
    /// <param name="path">The remote folder path to list</param>
    /// <param name="callback">An optional callback that receives the directory listing</param>
    /// <returns>A list of files in the remote server directory</returns>
    public async Task<List<SftpFile>> ListDirectoryAsync(string path,
        ISftpClient.ListDirectoryAsyncCallback callback = null)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Define our total container
        int total = 0;

        // Await the directory listing
        List<SftpFile> files = (await Task.Factory.FromAsync(
            Client.BeginListDirectory(path, r => total = (r as SftpListDirectoryAsyncResult)?.FilesRead ?? 0, null),
            Client.EndListDirectory))?.ToList();

        // Check for a callback and invoke it
        if (callback is not null) await callback.Invoke(files, total);

        // We're done, return the files
        return files;
    }

    /// <summary>
    /// This method creates a new directory on the remote server at <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="remoteDirectory">The path to the directory on the remote server</param>
    public void MakeDirectory(string remoteDirectory)
    {
        // Define our reserved directories
        string[] reservedDirectories = new string[] {".", "..", "/", @"\", ""};

        // Ensure the directory isn't reserved
        if (reservedDirectories.Contains(remoteDirectory)) return;

        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Check to see if the directory exists and create it
        if (!DirectoryExists(remoteDirectory)) Client.CreateDirectory(remoteDirectory);
    }

    /// <summary>
    /// This method asynchronously creates a new directory on the remote server at <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="remoteDirectory">The path to the directory on the remote server</param>
    /// <returns>An awaitable task with no result</returns>
    public async Task MakeDirectoryAsync(string remoteDirectory)
    {
        // Define our reserved directories
        string[] reservedDirectories = new string[] {".", "..", "/", @"\", ""};

        // Ensure the directory isn't reserved
        if (reservedDirectories.Contains(remoteDirectory)) return;

        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Check to see if the directory exists and create it
        if (!await DirectoryExistsAsync(remoteDirectory)) Client.CreateDirectory(remoteDirectory);
    }

    /// <summary>
    /// This method uploads the local directory <paramref name="directory" /> to the remote server as <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="directory">The local directory path to recursively upload</param>
    /// <param name="remoteDirectory">The remote directory path to upload to</param>
    /// <param name="overwrite">Denote whether or not files should be overwritten</param>
    /// <param name="itemCallback">Optional callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="directory" /> isn't a directory</exception>
    public void UploadDirectory(string directory, string remoteDirectory, bool overwrite = true,
        ISftpClient.ItemUploadCallback itemCallback = null)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Ensure we're working is a directory
        if ((File.GetAttributes(directory) & FileAttributes.Directory) != FileAttributes.Directory)
            throw new ArgumentException("Must be a directory path", nameof(directory));

        // Read the local directory
        DirectoryInfo directoryInfo = new DirectoryInfo(directory);

        // Ensure we have directory information
        if (directoryInfo.Exists)
        {
            // Create the directory on the server
            MakeDirectory(remoteDirectory);

            // Iterate over the subdirectories and create them
            foreach (DirectoryInfo subDirectory in directoryInfo.EnumerateDirectories().OrderBy(d => d.FullName))
            {
                // Localize the remote directory path
                string path = $"{remoteDirectory}{Path.DirectorySeparatorChar}{subDirectory.Name}";

                // Upload the directory to the remote server
                UploadDirectory(subDirectory.FullName, path, overwrite, itemCallback);

                // Execute our callback
                itemCallback?.Invoke(subDirectory, subDirectory.FullName, path);
            }

            // Iterate over the files and upload them
            foreach (FileInfo file in directoryInfo.EnumerateFiles().OrderBy(f => f.FullName))
            {
                // Localize the remote file path
                string path = $"{remoteDirectory}{Path.DirectorySeparatorChar}{file.Name}";

                // Localize the file stream into a disposable context
                FileStream fileStream = file.OpenRead();

                // Upload the file
                UploadFile(fileStream, path, overwrite);

                // Execute our callback
                itemCallback?.Invoke(file, file.FullName, path);
            }
        }
    }

    /// <summary>
    /// This method asynchronously uploads the local directory <paramref name="directory" /> to the remote server as <paramref name="remoteDirectory" />
    /// </summary>
    /// <param name="directory">The local directory path to recursively upload</param>
    /// <param name="remoteDirectory">The remote directory path to upload to</param>
    /// <param name="overwrite">Denote whether or not files should be overwritten</param>
    /// <param name="itemCallback">Optional asynchronous callback to be executed upon each item downloaded</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="directory" /> isn't a directory</exception>
    public async Task UploadDirectoryAsync(string directory, string remoteDirectory, bool overwrite = true,
        ISftpClient.ItemUploadCallbackAsync itemCallback = null)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Ensure we're working is a directory
        if ((File.GetAttributes(directory) & FileAttributes.Directory) != FileAttributes.Directory)
            throw new ArgumentException("Must be a directory path", nameof(directory));

        // Read the local directory
        DirectoryInfo directoryInfo = new DirectoryInfo(directory);

        // Ensure we have directory information
        if (directoryInfo.Exists)
        {
            // Create the directory on the server
            await MakeDirectoryAsync(remoteDirectory);

            // Iterate over the subdirectories and create them
            foreach (DirectoryInfo subDirectory in directoryInfo.EnumerateDirectories().OrderBy(d => d.FullName))
            {
                // Localize our remote directory path
                string path = $"{remoteDirectory}{Path.DirectorySeparatorChar}{subDirectory.Name}";

                // Upload the directory upload task
                await UploadDirectoryAsync(subDirectory.FullName, path, overwrite, itemCallback);

                // Check for a callback and add the callback task
                if (itemCallback is not null) await itemCallback.Invoke(subDirectory, subDirectory.FullName, path);
            }

            // Iterate over the files and add the upload to the task list
            foreach (FileInfo file in directoryInfo.EnumerateFiles().OrderBy(f => f.FullName))
            {
                // Localize our remote file path
                string path = $"{remoteDirectory}{Path.DirectorySeparatorChar}{file.Name}";

                // Localize our file stream into a disposable context
                FileStream fileStream = file.OpenRead();

                // Add the file upload task
                await UploadFileAsync(fileStream, path, overwrite);

                // Check for a callback and add the callback task
                if (itemCallback is not null) await itemCallback.Invoke(file, file.FullName, path);
            }
        }
    }

    /// <summary>
    /// This method uploads the local file <paramref name="filename" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="filename">The path to the local file to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    public void UploadFile(string filename, string remoteFilename, bool overwrite = true)
    {
        // Read the file into a disposable context
        FileStream file = File.OpenRead(filename);

        // We're done, upload the file
        UploadFile(file, remoteFilename, overwrite);
    }

    /// <summary>
    /// This method uploads the local file stream <paramref name="file" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="file">The local file stream to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    public void UploadFile(FileStream file, string remoteFilename, bool overwrite = true)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // Upload the file to the server
        Client.UploadFile(file, remoteFilename, overwrite);
    }

    /// <summary>
    /// This method asynchronously uploads the local file <paramref name="filename" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="filename">The path to the local file to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    /// <returns>An awaitable task with void result</returns>
    public Task UploadFileAsync(string filename, string remoteFilename, bool overwrite = true)
    {
        // Read the file into a disposable context
        FileStream file = File.OpenRead(filename);

        // We're done, upload the file
        return UploadFileAsync(file, remoteFilename, overwrite);
    }

    /// <summary>
    /// This method asynchronously uploads the local file stream <paramref name="file" /> to the remote file path <paramref name="remoteFilename" />
    /// </summary>
    /// <param name="file">The local file stream to upload</param>
    /// <param name="remoteFilename">The path on the remote server where the file will be uploaded to</param>
    /// <param name="overwrite">Denotes whether to overwrite any existing files or not</param>
    /// <returns>An awaitable task with void result</returns>
    public Task UploadFileAsync(FileStream file, string remoteFilename, bool overwrite = true)
    {
        // Ensure we're connected to the server
        if (!Client.IsConnected) Connect();

        // We're done, upload the file asynchronously
        return Task.Factory.FromAsync(Client.BeginUploadFile(file, remoteFilename, overwrite, null, null),
            Client.EndUploadFile);
    }

    /// <summary>
    /// This method fluidly resets the internal client into the instance
    /// </summary>
    /// <param name="client">The built internal client instance</param>
    /// <returns>The current instance of the service</returns>
    public ISftpClient WithClient(Renci.SshNet.SftpClient client)
    {
        // Reset the internal client into the instance
        Client = client;

        // We're done, return the instance
        return this;
    }

    /// <summary>
    /// This method fluidly resets the configuration into the instance
    /// </summary>
    /// <param name="configuration">The server connection configuration object</param>
    /// <param name="skipAutoConnect">Denotes whether to automatically establish a connection to the server, regardless of the server connection configuration object values</param>
    /// <returns>The current instance of the service</returns>
    public ISftpClient WithConfiguration(ISftpClientConfiguration configuration, bool skipAutoConnect = false)
    {
        // Reset the configuration into the instance
        Configuration = configuration;

        // Check the auto-connect flags and connect to the server
        if (configuration.AutoConnect && !skipAutoConnect) Connect();

        // We're done, return the instance
        return this;
    }
}
