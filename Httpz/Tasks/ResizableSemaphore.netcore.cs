﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Httpz;

internal partial class ResizableSemaphore : IDisposable
{
    private readonly Queue<TaskCompletionSource> _waiters = new();

    private void Refresh()
    {
        lock (_lock)
        {
            while (_count < MaxCount && _waiters.TryDequeue(out var waiter))
            {
                // Don't increment if the waiter has ben canceled
                if (waiter.TrySetResult())
                    _count++;
            }
        }
    }

    public async ValueTask<IDisposable> AcquireAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().Name);

        var waiter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await using (_cts.Token.Register(() => waiter.TrySetCanceled(_cts.Token)))
        await using (cancellationToken.Register(() => waiter.TrySetCanceled(cancellationToken)))
        {
            lock (_lock)
            {
                _waiters.Enqueue(waiter);
                Refresh();
            }

            await waiter.Task;

            return new AcquiredAccess(this);
        }
    }
}
