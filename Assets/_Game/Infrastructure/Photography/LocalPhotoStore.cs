using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Photography;
using PequenoExplorador.Domain.Content;
using UnityEngine;

namespace PequenoExplorador.Infrastructure.Photography
{
    public sealed class LocalPhotoStore : IPhotoStore
    {
        public const int MaximumFileBytes = 524288;
        public const int MaximumEntries = 64;
        public const long MaximumTotalBytes = 33554432;
        public const string ManifestFileName = "photos-index.json";
        private readonly string _directory;
        private readonly SemaphoreSlim _operation = new SemaphoreSlim(1, 1);
        private PhotoManifestDto _manifest = PhotoManifestDto.Empty();
        private bool _initialized;

        public LocalPhotoStore(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) throw new ArgumentException("Photo directory is required.", nameof(directory));
            _directory = Path.GetFullPath(directory);
        }

        public string ServiceId => "Photos";
        public string DirectoryPath => _directory;
        public int EntryCount => _manifest.Entries?.Length ?? 0;

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await _operation.WaitAsync(cancellationToken);
            try
            {
                if (_initialized) return;
                Directory.CreateDirectory(_directory);
                foreach (string temporary in Directory.GetFiles(_directory, "*.tmp")) File.Delete(temporary);
                string manifestPath = Path.Combine(_directory, ManifestFileName);
                if (File.Exists(manifestPath))
                {
                    string json = await ReadAllTextAsync(manifestPath, cancellationToken);
                    PhotoManifestDto loaded = JsonUtility.FromJson<PhotoManifestDto>(json);
                    if (!IsValidManifest(loaded)) throw new InvalidDataException("PhotoManifestInvalid");
                    _manifest = loaded;
                }
                var referenced = new HashSet<string>(_manifest.Entries.Select(item => item.FileReference), StringComparer.Ordinal);
                foreach (string image in Directory.GetFiles(_directory, "*.png"))
                    if (!referenced.Contains(Path.GetFileName(image))) File.Delete(image);
                _initialized = true;
            }
            finally { _operation.Release(); }
        }

        public void Shutdown() { }

        public async Task<PhotoStoreResult> SaveAsync(DiscoveryId discoveryId, int scorePermille,
            PhotoThumbnail thumbnail, CancellationToken cancellationToken)
        {
            if (!discoveryId.IsValid) throw new ArgumentException("Discovery ID is invalid.", nameof(discoveryId));
            if (scorePermille < 0 || scorePermille > 1000) throw new ArgumentOutOfRangeException(nameof(scorePermille));
            if (thumbnail == null) throw new ArgumentNullException(nameof(thumbnail));
            if (thumbnail.PngBytes.Length > MaximumFileBytes) throw new IOException("PhotoFileLimitExceeded");
            await _operation.WaitAsync(cancellationToken);
            try
            {
                EnsureInitialized();
                string fileName = SafeFileName(discoveryId, scorePermille);
                PhotoManifestEntryDto existing = _manifest.Entries.FirstOrDefault(item => item.DiscoveryId == discoveryId.Value);
                int currentCount = _manifest.Entries.Length;
                long currentBytes = _manifest.Entries.Sum(item => (long)item.ByteLength);
                long nextBytes = currentBytes - (existing?.ByteLength ?? 0) + thumbnail.PngBytes.Length;
                if (existing == null && currentCount >= MaximumEntries || nextBytes > MaximumTotalBytes)
                    throw new IOException("PhotoStoreBudgetExceeded");

                string finalPath = Path.Combine(_directory, fileName);
                string imageTemp = finalPath + ".tmp";
                string manifestPath = Path.Combine(_directory, ManifestFileName);
                string manifestTemp = manifestPath + ".tmp";
                try
                {
                    await WriteBytesFlushedAsync(imageTemp, thumbnail.PngBytes, cancellationToken);
                    ReplaceOrMove(imageTemp, finalPath);
                    var entries = _manifest.Entries.Where(item => item.DiscoveryId != discoveryId.Value).ToList();
                    entries.Add(PhotoManifestEntryDto.Create(discoveryId.Value, fileName, scorePermille,
                        thumbnail.Width, thumbnail.Height, thumbnail.PngBytes.Length));
                    _manifest = PhotoManifestDto.Create(entries.OrderBy(item => item.DiscoveryId, StringComparer.Ordinal).ToArray());
                    await WriteTextFlushedAsync(manifestTemp, JsonUtility.ToJson(_manifest, false), cancellationToken);
                    ReplaceOrMove(manifestTemp, manifestPath);
                    if (existing != null && !string.Equals(existing.FileReference, fileName, StringComparison.Ordinal))
                        DeleteIfPresent(Path.Combine(_directory, existing.FileReference));
                    return new PhotoStoreResult(fileName, thumbnail.PngBytes.Length);
                }
                finally
                {
                    DeleteIfPresent(imageTemp);
                    DeleteIfPresent(manifestTemp);
                }
            }
            finally { _operation.Release(); }
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            await _operation.WaitAsync(cancellationToken);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (Directory.Exists(_directory))
                    foreach (string path in Directory.GetFiles(_directory)) File.Delete(path);
                _manifest = PhotoManifestDto.Empty();
            }
            finally { _operation.Release(); }
        }

        public async Task<PhotoLoadResult> LoadAsync(string fileReference, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileReference)) return PhotoLoadResult.Missing();
            await _operation.WaitAsync(cancellationToken);
            try
            {
                EnsureInitialized();
                PhotoManifestEntryDto entry = _manifest.Entries.FirstOrDefault(item =>
                    string.Equals(item.FileReference, fileReference, StringComparison.Ordinal));
                if (entry == null) return PhotoLoadResult.Missing();
                string path = Path.Combine(_directory, entry.FileReference);
                if (!File.Exists(path)) return PhotoLoadResult.Missing();
                byte[] bytes = await ReadAllBytesAsync(path, cancellationToken);
                if (bytes.Length != entry.ByteLength || bytes.Length < 1 || bytes.Length > MaximumFileBytes)
                    return PhotoLoadResult.Invalid();
                return PhotoLoadResult.Loaded(bytes);
            }
            catch (IOException)
            {
                return PhotoLoadResult.Invalid();
            }
            finally { _operation.Release(); }
        }

        public static string SafeFileName(DiscoveryId id, int scorePermille = 0)
        {
            if (!id.IsValid) throw new ArgumentException("Discovery ID is invalid.", nameof(id));
            if (scorePermille < 0 || scorePermille > 1000) throw new ArgumentOutOfRangeException(nameof(scorePermille));
            return id.Value.Replace('.', '_') + "-" + scorePermille.ToString(System.Globalization.CultureInfo.InvariantCulture) + ".png";
        }

        private static bool IsValidManifest(PhotoManifestDto manifest)
        {
            if (manifest?.Entries == null || manifest.Entries.Length > MaximumEntries) return false;
            if (manifest.Entries.Any(item => item == null || !DiscoveryId.TryParse(item.DiscoveryId, out DiscoveryId id) ||
                item.ScorePermille < 0 || item.ScorePermille > 1000 || item.FileReference != SafeFileName(id, item.ScorePermille) ||
                item.Width < 1 || item.Width > 512 || item.Height < 1 || item.Height > 512 ||
                item.ByteLength < 1 || item.ByteLength > MaximumFileBytes)) return false;
            return manifest.Entries.Select(item => item.DiscoveryId).Distinct(StringComparer.Ordinal).Count() == manifest.Entries.Length &&
                   manifest.Entries.Sum(item => (long)item.ByteLength) <= MaximumTotalBytes;
        }

        private static async Task<string> ReadAllTextAsync(string path, CancellationToken token)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            using var reader = new StreamReader(stream);
            string text = await reader.ReadToEndAsync();
            token.ThrowIfCancellationRequested();
            return text;
        }

        private static async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken token)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            if (stream.Length < 1 || stream.Length > MaximumFileBytes) return Array.Empty<byte>();
            var bytes = new byte[stream.Length];
            int offset = 0;
            while (offset < bytes.Length)
            {
                int read = await stream.ReadAsync(bytes, offset, bytes.Length - offset, token);
                if (read == 0) break;
                offset += read;
            }
            return offset == bytes.Length ? bytes : Array.Empty<byte>();
        }

        private static async Task WriteBytesFlushedAsync(string path, byte[] bytes, CancellationToken token)
        {
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough);
            await stream.WriteAsync(bytes, 0, bytes.Length, token);
            await stream.FlushAsync(token);
            stream.Flush(true);
        }

        private static async Task WriteTextFlushedAsync(string path, string text, CancellationToken token)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            await WriteBytesFlushedAsync(path, bytes, token);
        }

        private static void ReplaceOrMove(string source, string destination)
        {
            if (File.Exists(destination)) File.Replace(source, destination, null, true);
            else File.Move(source, destination);
        }

        private static void DeleteIfPresent(string path) { if (File.Exists(path)) File.Delete(path); }
        private void EnsureInitialized() { if (!_initialized) throw new InvalidOperationException("Photo store is not initialized."); }

        [Serializable]
        private sealed class PhotoManifestDto
        {
            [SerializeField] private PhotoManifestEntryDto[] entries;
            public PhotoManifestEntryDto[] Entries => entries;
            public static PhotoManifestDto Empty() => Create(Array.Empty<PhotoManifestEntryDto>());
            public static PhotoManifestDto Create(PhotoManifestEntryDto[] values) => new PhotoManifestDto { entries = values };
        }

        [Serializable]
        private sealed class PhotoManifestEntryDto
        {
            [SerializeField] private string discoveryId;
            [SerializeField] private string fileReference;
            [SerializeField] private int scorePermille;
            [SerializeField] private int width;
            [SerializeField] private int height;
            [SerializeField] private int byteLength;
            public string DiscoveryId => discoveryId;
            public string FileReference => fileReference;
            public int ScorePermille => scorePermille;
            public int Width => width;
            public int Height => height;
            public int ByteLength => byteLength;
            public static PhotoManifestEntryDto Create(string id, string reference, int score, int pixelWidth, int pixelHeight, int bytes) =>
                new PhotoManifestEntryDto { discoveryId = id, fileReference = reference, scorePermille = score,
                    width = pixelWidth, height = pixelHeight, byteLength = bytes };
        }
    }
}
