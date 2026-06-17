# Task Fallacy #

Using `Task` like `Thread` can lead to unexpected behavior. A `Task` is executed on a thread coming from a thread pool, whereas a `Thread` is a real .NET thread. If a `Task` is blocked, e.g. when waiting for a lock or a barrier, there may not be enough unblocked threads left to proceed, leading to a deadlock.

## A ##

A thread-based implementation; we create `n` threads, all calling `barrier.SignalAndAwait()`. Because they are .NET threads, they can get scheduled independently from each other and no deadlock occurs.


## B ##

A task-based implementation; we create and run `n` tasks. A task that calls `barrier.SignalAndAwait()` does not block the task, but the executing thread, leading to a deadlock.
