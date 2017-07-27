#!/bin/bash

# Everything is named as the containing folder.
NAME=$(basename $(pwd))
mkdir -p bin bin/javadoc

# Compile and build docs.
javac -d bin src/*.java
javadoc -d bin/javadoc src/*.java

# Build a JAR file.
(cd bin; jar cfv $NAME.jar dk/itu/pcpp/*.class)

# Build a distributable archive.
zip -r $NAME.zip bin/javadoc bin/$NAME.jar
