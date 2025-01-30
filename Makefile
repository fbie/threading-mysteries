.PHONY: slides dotnet all

all: slides dotnet

slides:
	pandoc -t pptx slides/Readme.md -o slides/slides.pptx

dotnet:
	dotnet build
