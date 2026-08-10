using System;

namespace EchoDevGames.EchoSave
{
    internal enum SaveOperationAdmissionStatus
    {
        Admitted = 0,
        Busy = 1,
        Closed = 2
    }

    /// <summary>
    /// Root-local M4-04 admission authority for mutating Chronicle operations.
    ///
    /// There is deliberately no static/global lease and no queue. Future
    /// mutating checkpoints may reuse this authority without changing the
    /// manual-save Busy contract.
    /// </summary>
    internal sealed class SaveOperationAdmissionCoordinator
    {
        private readonly object gate =
            new object();

        private bool closed = true;
        private long nextToken;
        private long activeToken;

        internal bool IsClosed
        {
            get
            {
                lock (gate)
                {
                    return closed;
                }
            }
        }

        internal bool IsOccupied
        {
            get
            {
                lock (gate)
                {
                    return activeToken != 0L;
                }
            }
        }

        internal void Open()
        {
            lock (gate)
            {
                closed = false;
            }
        }

        internal void Close()
        {
            lock (gate)
            {
                closed = true;
            }
        }

        internal SaveOperationAdmissionStatus TryAcquire(
            out SaveOperationAdmissionLease lease)
        {
            lock (gate)
            {
                if (closed)
                {
                    lease = null;

                    return SaveOperationAdmissionStatus.Closed;
                }

                if (activeToken != 0L)
                {
                    lease = null;

                    return SaveOperationAdmissionStatus.Busy;
                }

                if (nextToken ==
                    long.MaxValue)
                {
                    nextToken =
                        0L;
                }

                long token =
                    ++nextToken;

                if (token == 0L)
                {
                    token =
                        ++nextToken;
                }

                activeToken =
                    token;

                lease =
                    new SaveOperationAdmissionLease(
                        this,
                        token);

                return SaveOperationAdmissionStatus.Admitted;
            }
        }

        internal void Release(
            long token)
        {
            lock (gate)
            {
                if (token != 0L &&
                    activeToken ==
                    token)
                {
                    activeToken =
                        0L;
                }
            }
        }
    }

    internal sealed class SaveOperationAdmissionLease :
        IDisposable
    {
        private SaveOperationAdmissionCoordinator owner;
        private readonly long token;

        internal SaveOperationAdmissionLease(
            SaveOperationAdmissionCoordinator owner,
            long token)
        {
            this.owner =
                owner;

            this.token =
                token;
        }

        public void Dispose()
        {
            SaveOperationAdmissionCoordinator current =
                owner;

            if (current == null)
            {
                return;
            }

            owner =
                null;

            current.Release(
                token);
        }
    }
}
