# A Parallel Sequence Generator #

Two threads are trying to construct a sequence of numbers, going from 1 to n.


## A ##

A very naive implementation. The two threads do not synchronize. Therefore, we have a race condition.

```mermaid
sequenceDiagram
    participant T0
    participant T1
    participant _counter
    participant _ns

    T0->>_counter: read
    _counter-->>T0: 42
    T1->>_counter: read
    _counter-->>T1: 42
    T1->>_counter: set(43)
    T1->>_ns: read next index
    _ns-->>T1:71
    T1->>_ns: set(71, 44)
    T0->>_counter: set(43)
    T0->>_ns: read next index
    _ns-->>T0:72
    T0->>_ns: set(72, 43)
```

## B ##

An attempt at synchronizing; notice that `_current` is marked as `volatile`. But the code in `Next()` performs a non-atomic check-then-act sequence, and updates are lost.


## C ##

An implementation using the `lock` keyword on `_ns`. This corrects both, the missing synchronization as well as the non-atomic check-then-act sequence.

```mermaid
sequenceDiagram
    participant T0
    participant T1
    participant _counter
    participant _ns

    T0->>+_ns: lock()
    activate _ns
    T0->>_counter: read
    _counter-->>T0: 42
    T0->>_counter: set(43)
    T0->>_ns: read next index
    _ns-->>T0:71
    T0->>_ns: set(71, 43)
    T0->>_ns: release()
    deactivate _ns

    T1->>+_ns: lock()
    activate _ns
    T1->>_counter: read
    _counter-->>T1: 43
    T1->>_counter: set(44)
    T1->>_ns: read next index
    _ns-->>T1:72
    T1->>_ns: set(72, 44)
    T1->>_ns: release()
    deactivate _ns
```
