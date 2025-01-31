.PHONY: slides show dotnet all

all: slides dotnet

slides:
	pandoc -t pptx slides/Readme.md -o slides/slides.pptx

show: slides
	wslview slides/slides.pptx

dotnet:
	dotnet build
