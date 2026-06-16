# A Parallel Character Histogram #

For each file name, a new thread is started that

1. reads the entire file; and
2. adds the frequency of each character in the file to a shared histogram.

## A ##

A simple non-thread safe implementation using `Dicionary<char, int>`. Trying to run with more than one thread crashes the program.

## B ##

A thread-safe implementation using `Dictionary<char, int>`. When adding an entry to the histogram, the method takes a lock on the `_chars` dictionary, which avoids concurrent modifications and thereby corrupting the state of the dictionary. However, mutual exclusion means slower run-time performance -- at least, that is the colloquial wisdom.


## C ##

A thread-safe implementation that uses `ConcurrentDictionary<char, int>` instead of locking. This dictionary implementation uses a more complicated and fine-grained mechanism to ensure synchronization and atomic updates. Colloquial wisdom says that this is faster, but that may depend very much on the workload.
