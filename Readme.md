

This is a test MonoGame project to demonstrate how I figured out how to build C# projects (and use the MonoGame Pipeline Tool) from CMAKE.

The effect/shader should show a model of the earth with continents and a day/night shader/effect.

To build:

Make sure MonoGame SDK 3.6 is installed and mgcb.exe is on the search PATH
Make sure Nuget is installed and nuget.exe is on the search PATH
Make sure CMAKE 3.8 or later is installed and cmake is on the search PATH

mkdir build
cd build
cmake ..
cmake --build .

