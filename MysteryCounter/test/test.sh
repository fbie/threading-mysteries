#!/bin/bash

mkdir -p ../bin
javac -d ../bin ../src/*.java Test.java
java -cp ../bin dk.itu.pcpp.Test
