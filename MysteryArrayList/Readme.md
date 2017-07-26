# Mystery Array List #

1. Behaves as intended.
2. Synchronization in `append` is too optimistic; volatile does not guarantee atomicity, hence non-atomic check-then-act sequence.
3. If the caller keeps a reference to `array`, they can modify the array list from the outside, effectively breaking the thread safe interface.
