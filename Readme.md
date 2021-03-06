# Threading Mysteries #

**Please do not browse this repository if you are a PCPP student at the IT University of Copenhagen!**

This repository contains a small collection of Java classes that may or may not be thread safe.  The objective is to write code that exposes the difference in each implementation and thereby to learn in an exploratory way, how subtle differences in supposedly thread safe code can affect both, correctness and performance.

For instance, a supposedly thread safe counter may be working correctly for the majority of use cases.  But there may be corner cases which the implementer did not consider, which are broken.  The task is to find these corner cases by putting the counter implementations into different artificial usage scenarios to provoke different behaviors.

Here is a possible ordering of the mysteries:

1. [MysteryCounter](MysteryCounter)
2. [MysteryArrayList](MysteryArrayList)



### Adding Mysteries ###

If you would like to add a threading mystery, please add a new folder with a name that describes your class and copy the files `build.sh` and `clean.sh` from one of the other folders.  All sources should have `package dk.itu.pcpp;` as first line.



### Acknowledgements ###

This idea is heavily inspired by the "mystery languages" approach from the [Racket Summer School 2017](https://github.com/justinpombrio/RacketSchool).  We highly recommend to participate if you get the chance!
