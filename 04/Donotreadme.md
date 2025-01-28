# Collaborative Counting #

A bit contrived, but an important point about hardware.

We start `<number of threads>` tasks that increment the same counter concurrently. The different counter implementations have different performance characteristics, even though superficially, only one should appear to be slow under high contention.


## A ##

A lock-free implementation of a counter. All synchronization happens through intrinsic methods that generate atomic CPU instructions.


## B ##

A locking implementation, simple but inefficient under high load. Each increment locks, so does reading the current value.


## C ##

A lock-free implementation that is cache-aware. To avoid the "false sharing" by invalidating the cache when writing, the counter's value is split across multiple values in an array. We only access indices in the array whose values are not on the same cache line. Values are striped, meaning it is possible that two threads write to the same index in an array, but not extremely likely.
