# LimeSDR# is C# program for LimeSDR board.
- 04-03-2018 initial commit.
Requires .NET 3.5 and SlimDX (optional).
Built and tested on Windows 7 x64.
In folder ./bin/Release exists two programs with different video drivers: limeSDR# GDI.exe is built
with only GDI+ driver and LimeSDR# DirectX.exe with additional DirectX using SlimDX library (https://slimdx.org/download.php,
download and instal: 
https://storage.googleapis.com/google-code-archive-downloads/v2/code.google.com/slimdx/SlimDX%20SDK%20(January%202012).msi).
For custom built band pass filters use LimeSDR# Database Editor.exe and in LimeSDRBandFilters table change your band edges.
