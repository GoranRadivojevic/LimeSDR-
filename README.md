--------------------------------------------------------------
- Initial information
--------------------------------------------------------------
# LimeSDR# is C# program for LimeSDR-USB board.
- 04-03-2018 initial commit.
- 17-03-2018 update
- 18-03-2018 update

--------------------------------------------------------------
- Prerequisites
--------------------------------------------------------------
- .NET 4.6.1 (with minimal changes can be built for .NET3.5)
- SlimDX (optional).
DirectX using SlimDX library https://slimdx.org/download.php,
download and instal: 
https://storage.googleapis.com/google-code-archive-downloads/v2/code.google.com/slimdx/SlimDX%20SDK%20(January%202012).msi.

--------------------------------------------------------------
- Build
--------------------------------------------------------------
- Built on Windows 10 Home x64 with Visual Studio Comunity 2017 (tested on Windows  x64).
- In folder ./bin/Release exists two programs with different video drivers: limeSDR# GDI.exe is built
with only GDI+ driver and LimeSDR# DirectX.exe with additional - For custom built band pass filters use LimeSDR# Database Editor.exe and in LimeSDRBandFilters table change your band edges.

--------------------------------------------------------------
- J12 connector
--------------------------------------------------------------
- Outputs: 0-3 BPF (4 bits), 4-PTT (MOX)
- Inputs: 5-PTT microphone, 6-keyer DASH, 7-keyer DOT 
