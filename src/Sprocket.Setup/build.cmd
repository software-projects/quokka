@echo off
set NANT_DIR=\build\programs\nant-0.86-beta1\bin\
%NANT_DIR%\nant clean
%NANT_DIR%\nant %1 %2 %3 %4