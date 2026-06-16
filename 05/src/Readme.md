# Parallel Integer Factorization #

The cost of threads.

Depending on variant, we start `<number of factors>`

- threads;
- tasks; or
- execute a parallel for loop.

The higher the workload, the more the cost of allocating threads starts to dominate.

## A ##

A thread-based implementation; one thread is created per integer to factorize.


## B ##

A task-based implementation; one task is created per integer to factorize. The task is executed on a thread pool, so no new threads are started up as the number of tasks increases.


## C ##

An implementation based on `Parallel.ForEach()`. It has a bit higher cost than tasks (at least on my machine), and the API is a bit cumbersome to work with.
