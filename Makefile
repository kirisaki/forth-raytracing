# Makefile for a gforth project (include style, fixed)

ENTRY   := src/main.fs
TESTS   := test/all-tests.fs

GFORTH       := gforth

.PHONY: all run

all: run

run:
	@$(GFORTH) $(ENTRY)
