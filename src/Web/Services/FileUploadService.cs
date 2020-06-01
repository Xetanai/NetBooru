using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetBooru.Web.Options;

namespace NetBooru.Web.Services
{
    public class FileUploadService
    {
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<UploadOptions> _options;
        private readonly MemoryPool<byte> _pool;

        public FileUploadService(
            ILogger<FileUploadService> logger,
            IOptionsMonitor<UploadOptions> options,
            MemoryPool<byte> pool)
        {
            _logger = logger;
            _options = options;
            _pool = pool;
        }

        public async Task<byte[]> UploadFileAsync(IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var options = _options.CurrentValue;

            if (file.Length > options.MaxFileSize)
                throw new Exception("File is above maximum allowed size");

            var tempFile = Path.GetTempFileName();
            byte[] hash;
            string fileLocation;

            using (var destination = File.Create(tempFile))
            {
                using var stream = file.OpenReadStream();
                using var hasher = IncrementalHash.CreateHash(
                    HashAlgorithmName.SHA256);
                using var buffer = _pool.Rent(4096);

                while (true)
                {
                    var bytesRead = await stream.ReadAsync(buffer.Memory,
                        cancellationToken);

                    if (bytesRead <= 0)
                        break;

                    hasher.AppendData(buffer.Memory.Span.Slice(0, bytesRead));

                    await destination.WriteAsync(
                        buffer.Memory.Slice(0, bytesRead),
                        cancellationToken);
                }

                // TODO: this should throw something better
                hash = hasher.GetHashAndReset();
                fileLocation = GetFileLocation(hash, options);
            }

            File.Move(tempFile, fileLocation);
            return hash;
        }

        public string GetFileLocation(ReadOnlySpan<byte> fileHash,
            UploadOptions? options = null)
        {
            options ??= _options.CurrentValue;

            return !TryGetDestinationLocation(fileHash, options.UploadLocation,
                options.DirectorySeparationBytes, out var destinationLocation)
                ? throw new Exception("Could not compute file location")
                : destinationLocation;

            static bool TryGetDestinationLocation(ReadOnlySpan<byte> hashBytes,
                string uploadLocation, int directorySeparationBytes,
                [NotNullWhen(true)]
                out string? destinationLocation)
            {
                destinationLocation = default;
                Span<char> hash = stackalloc char[hashBytes.Length * 2];

                if (!TryConvertToHex(hashBytes, hash))
                    return false;

                var left = new string(
                    hash.Slice(0, directorySeparationBytes * 2));
                var right = new string(
                    hash.Slice(directorySeparationBytes * 2));

                destinationLocation
                    = Path.Combine(uploadLocation, left, right);
                return true;
            }

            static bool TryConvertToHex(ReadOnlySpan<byte> hash,
                Span<char> destination)
            {
                var pos = 0;
                for (int x = 0; x < hash.Length; x++)
                {
                    if (!hash[x].TryFormat(destination.Slice(pos),
                        out var charsWritten, "X2"))
                        return false;

                    pos += charsWritten;
                }

                return true;
            }
        }
    }
}
