using System.Threading;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Narrow M4-04 cancellation/publication-boundary control passed through
    /// the already-proven M4-03 transaction.
    /// </summary>
    internal sealed class SaveManualTransactionControl
    {
        private readonly CancellationToken cancellationToken;

        internal SaveManualTransactionControl(
            CancellationToken cancellationToken)
        {
            this.cancellationToken =
                cancellationToken;
        }

        internal bool IsCancellationRequested =>
            cancellationToken.IsCancellationRequested;

        internal bool PublicationStarted { get; private set; }

        internal bool TryBeginPublication()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            PublicationStarted =
                true;

            return true;
        }
    }
}
