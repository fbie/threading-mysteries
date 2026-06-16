# A Parallel Union Find Implementation #

For each file name, a new thread is started that

1. reads the entire file; and
2. for each line, performs a union operations on the two sets specified.


## A ##

A thread-safe implementation; it synchronizes on the parents of two sets, but only after sorting them by `Id` to avoid deadlocking in case another thread attempts to lock them in the opposite order.


## B ##

A non thread-safe implementation. This can in the worst case crash with a stack overflow error, because of spurious synchronizations s.t. `Find()` never terminates, either because the `Parent` constantly changes, or because no synchronization happens and a transient cyclic dependency seems to be in place, but only from the point of view of the crashing thread.


## C ##

A non thread-safe implementation that omits to sort sets by their `Id`, which is why it is amenable to deadlock.
