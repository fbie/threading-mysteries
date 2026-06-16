# Collaborative Counting #

A bit contrived, but an important point about hardware.

We start `<number of threads>` tasks that increment the same counter concurrently. The different counter implementations have different performance characteristics, even though superficially, only one should appear to be slow under high contention.


## A ##

A lock-free implementation of a counter. All synchronization happens through intrinsic methods that generate atomic CPU instructions.


## B ##

A lock-free implementation that is cache-aware. To avoid the "false sharing" by invalidating the cache across all cores when writing, the counter's value is split across multiple values in an array. Each value in the array occupies 64 bytes, which is usually the size of a cache-line on modern CPUs.


## C ##

A locking implementation, simple but inefficient under high load. Each increment locks, so does reading the current value.
