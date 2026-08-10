namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public bounded ticket for one accepted autosave request.
    ///
    /// Chronicle mutates ticket state internally so callers can distinguish
    /// pending, superseded, discarded, and terminal save outcomes without
    /// exposing mutable queue internals or retaining unbounded history.
    /// </summary>
    public sealed class AutosaveTicket
    {
        private readonly object gate =
            new object();

        private AutosaveTicketState state;
        private string diagnosticCode;
        private string message;
        private bool hasSaveResult;
        private SaveOperationResult saveResult;

        internal AutosaveTicket(
            long id)
        {
            Id =
                id;

            state =
                AutosaveTicketState.Pending;

            diagnosticCode =
                string.Empty;

            message =
                string.Empty;
        }

        public long Id { get; }

        public AutosaveTicketState State
        {
            get
            {
                lock (gate)
                {
                    return state;
                }
            }
        }

        public string DiagnosticCode
        {
            get
            {
                lock (gate)
                {
                    return diagnosticCode;
                }
            }
        }

        public string Message
        {
            get
            {
                lock (gate)
                {
                    return message;
                }
            }
        }

        public bool HasSaveResult
        {
            get
            {
                lock (gate)
                {
                    return hasSaveResult;
                }
            }
        }

        public SaveOperationResult SaveResult
        {
            get
            {
                lock (gate)
                {
                    return saveResult;
                }
            }
        }

        public bool IsTerminal
        {
            get
            {
                lock (gate)
                {
                    return state !=
                               AutosaveTicketState.Pending &&
                           state !=
                               AutosaveTicketState.Executing;
                }
            }
        }

        internal void MarkPending(
            string diagnosticCode,
            string message)
        {
            lock (gate)
            {
                state =
                    AutosaveTicketState.Pending;

                this.diagnosticCode =
                    diagnosticCode ??
                    string.Empty;

                this.message =
                    message ??
                    string.Empty;
            }
        }

        internal void MarkExecuting()
        {
            lock (gate)
            {
                state =
                    AutosaveTicketState.Executing;

                diagnosticCode =
                    string.Empty;

                message =
                    "The Chronicle admitted this autosave request for execution.";
            }
        }

        internal void Complete(
            SaveOperationResult result)
        {
            lock (gate)
            {
                saveResult =
                    result;

                hasSaveResult =
                    true;

                if (result.Status ==
                    SaveOperationStatus.Canceled)
                {
                    state =
                        AutosaveTicketState.Canceled;
                }
                else
                {
                    state =
                        result.Succeeded
                            ? AutosaveTicketState.Succeeded
                            : AutosaveTicketState.Failed;
                }

                diagnosticCode =
                    result.DiagnosticCode;

                message =
                    result.Message;
            }
        }

        internal void MarkSuperseded()
        {
            lock (gate)
            {
                state =
                    AutosaveTicketState.Superseded;

                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .AutosaveSuperseded;

                message =
                    "A newer Chronicle autosave request replaced this pending request before execution.";
            }
        }

        internal void MarkDiscarded(
            string diagnosticCode,
            string message,
            bool canceled)
        {
            lock (gate)
            {
                state =
                    canceled
                        ? AutosaveTicketState.Canceled
                        : AutosaveTicketState.Discarded;

                this.diagnosticCode =
                    diagnosticCode ??
                    string.Empty;

                this.message =
                    message ??
                    string.Empty;
            }
        }
    }
}
