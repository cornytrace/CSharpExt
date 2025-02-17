using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Utility
{
    public interface ITempFolder : IDisposable
    {
        DirectoryPath Dir { get; }
    }

    public class TempFolder : ITempFolder
    {
        public DirectoryPath Dir { get; private set; }
        public bool DeleteAfter = true;
        public bool ThrowIfUnsuccessfulDisposal = true;
        private readonly IFileSystem _fileSystem;

        protected TempFolder(
            DirectoryPath dir, 
            bool deleteAfter = true,
            bool throwIfUnsuccessfulDisposal = true,
            IFileSystem? fileSystem = null)
        {
            Dir = dir;
            _fileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
            DeleteAfter = deleteAfter;
            ThrowIfUnsuccessfulDisposal = throwIfUnsuccessfulDisposal;
            
            _fileSystem.Directory.CreateDirectory(Dir);
        }

        public void Dispose()
        {
            if (DeleteAfter)
            {
                try
                {
                    _fileSystem.Directory.DeleteEntireFolder(Dir.Path);
                }
                catch when(!ThrowIfUnsuccessfulDisposal)
                {
                }
            }
        }

        public static TempFolder Factory(
            bool deleteAfter = true, 
            bool throwIfUnsuccessfulDisposal = true,
            IFileSystem? fileSystem = null)
        {
            return new TempFolder(
                new DirectoryPath(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())),
                deleteAfter: deleteAfter, 
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
                fileSystem: fileSystem);
        }

        public static TempFolder FactoryByPath(
            string path,
            bool deleteAfter = true, 
            bool throwIfUnsuccessfulDisposal = true,
            IFileSystem? fileSystem = null)
        {
            return new TempFolder(
                new DirectoryPath(path),
                deleteAfter: deleteAfter,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
                fileSystem: fileSystem);
        }

        public static TempFolder FactoryByAddedPath(
            string addedFolderPath, 
            bool deleteAfter = true, 
            bool throwIfUnsuccessfulDisposal = true,
            IFileSystem? fileSystem = null)
        {
            return new TempFolder(
                new DirectoryPath(Path.Combine(Path.GetTempPath(), addedFolderPath)),
                deleteAfter: deleteAfter,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
                fileSystem: fileSystem);
        }

#if NETSTANDARD2_0
#else
        public static AsyncTempFolder Factory(int retryCount, TimeSpan delay, bool throwIfUnsuccessfulDisposal = true)
        {
            return new AsyncTempFolder(
                new DirectoryPath(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())),
                retryCount: retryCount,
                delay: delay,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal);
        }

        public static AsyncTempFolder FactoryByPath(string path, int retryCount, TimeSpan delay, bool throwIfUnsuccessfulDisposal = true)
        {
            return new AsyncTempFolder(
                new DirectoryPath(path),
                retryCount: retryCount,
                delay: delay,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal);
        }

        public static AsyncTempFolder FactoryByAddedPath(string addedFolderPath, int retryCount, TimeSpan delay, bool throwIfUnsuccessfulDisposal = true)
        {
            return new AsyncTempFolder(
                new DirectoryPath(Path.Combine(Path.GetTempPath(), addedFolderPath)),
                retryCount: retryCount, 
                delay: delay,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal);
        }
#endif
    }

#if NETSTANDARD2_0
#else
    public class AsyncTempFolder : TempFolder, IAsyncDisposable
    {
        public int RetryCount;
        public TimeSpan Delay;

        public AsyncTempFolder(DirectoryPath dir, int retryCount, TimeSpan delay, bool throwIfUnsuccessfulDisposal = true)
            : base(dir, deleteAfter: true, throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal)
        {
            RetryCount = retryCount;
            Delay = delay;
        }

        public async ValueTask DisposeAsync()
        {
            Exception? ex = null;
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    Dir.DeleteEntireFolder();
                }
                catch (Exception e)
                {
                    ex = e;
                }
                if (!Dir.Exists) return;
                await Task.Delay(Delay).ConfigureAwait(false);
            }
            if (this.ThrowIfUnsuccessfulDisposal && Dir.Exists)
            {
                if (ex != null)
                {
                    throw ex;
                }
                throw new Exception($"Could not clean up temp directory: {Dir.Path}");
            }
        }
    }
#endif
}
