

This is a test MonoGame project to show a problem with MonoGame on Windows 10 that doesn't occur on Windows 7.

The effect/shader should show a model of the earth with continents and a day/night shader/effect.

This project also being used to try to see if there is a way to compile a DEBUG version of MonoGame 3.6 and step into the MonoGame classes in the Visual Studio 2017 debugger.

The two MonoGame issues that use this code are at:

    https://github.com/MonoGame/MonoGame/issues/6169
    https://github.com/MonoGame/MonoGame/issues/6171

To build:

Make sure MonoGame SDK 3.6 is installed and mgcb.exe is on the search PATH
Make sure Nuget is installed and nuget.exe is on the search PATH
Make sure CMAKE 3.8 or later is installed and cmake is on the search PATH

mkdir build
cd build
cmake ..
cmake --build .

