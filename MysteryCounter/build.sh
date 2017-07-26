#!/bin/bash

# Everything is named as the containing folder.
NAME=$(basename $(pwd))
mkdir -p bin javadoc

# Compile and build docs.
javac -d bin *.java
javadoc -d javadoc *.java

# Build a JAR file.
jar cf $NAME.jar bin/dk/itu/pcpp/*.class

# Build a distributable archive.
zip -r $NAME.zip javadoc $NAME.jar
