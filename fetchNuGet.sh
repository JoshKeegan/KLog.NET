#!/bin/bash
wget http://nuget.org/nuget.exe
mkdir -p ./.nuget
cp nuget.exe ./.nuget/NuGet.exe
chmod a+x ./.nuget/NuGet.exe