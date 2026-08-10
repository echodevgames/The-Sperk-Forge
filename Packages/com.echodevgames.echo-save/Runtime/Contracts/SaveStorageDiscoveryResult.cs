
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Structured path-redacted result for provider-neutral child discovery.
    /// </summary>
    public sealed class SaveStorageDiscoveryResult
    {
        private readonly ReadOnlyCollection<string> childNames;

        public SaveStorageDiscoveryResult(
            SaveStorageDiscoveryStatus status,
            string diagnosticCode,
            string message,
            string[] childNames)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;

            string[] copy =
                childNames == null
                    ? Array.Empty<string>()
                    : (string[])childNames.Clone();

            this.childNames =
                Array.AsReadOnly(copy);
        }

        public SaveStorageDiscoveryStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public IReadOnlyList<string> ChildNames =>
            childNames;

        public bool Succeeded =>
            Status == SaveStorageDiscoveryStatus.Succeeded ||
            Status == SaveStorageDiscoveryStatus.ParentNotFound;

        public static SaveStorageDiscoveryResult Success(
            string[] childNames,
            string message) =>
            new SaveStorageDiscoveryResult(
                SaveStorageDiscoveryStatus.Succeeded,
                string.Empty,
                message,
                childNames);

        public static SaveStorageDiscoveryResult ParentNotFound(
            string message) =>
            new SaveStorageDiscoveryResult(
                SaveStorageDiscoveryStatus.ParentNotFound,
                string.Empty,
                message,
                Array.Empty<string>());
    }
}
