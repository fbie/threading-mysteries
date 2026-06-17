.PHONY: slides show dotnet test all

all: slides dotnet test

slides:
	pandoc -t pptx slides/Readme.md -o slides/slides.pptx

show: slides
	xdg-open slides/slides.pptx

build:
	dotnet build threading-mysteries.slnx -c Release

test: build
	dotnet test threading-mysteries-tests.slnx
