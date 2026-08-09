
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primitive byte-read result.
    /// </summary>
    public readonly struct SaveStorageReadResult
    {
        private readonly byte[] data;

        public SaveStorageReadResult(
            SaveStorageResult result,
            byte[] data)
        {
            Result = result;
            this.data =
                data == null
                    ? Array.Empty<byte>()
                    : (byte[])data.Clone();
        }

        public SaveStorageResult Result { get; }

        public bool Succeeded =>
            Result.Succeeded;

        public byte[] Data =>
            data == null
                ? Array.Empty<byte>()
                : (byte[])data.Clone();
    }
}
