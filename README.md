# TitanUnpacker
Koei Tecmo games unpacker/repacker.
## Tested on:
* Attack on Titan / A.O.T. Wings of Freedom
## Instructions
* USAGE: TitanUnpacker.exe -unpack/-repack/-zlib 'file' 'folder'
* Unpack the file: TitanUnpacker.exe -unpack LINKDATA_EU_A.bin Unpacked
* Pack the folder: TitanUnpacker.exe -pack Unpacked Generated.bin
* Unpack the compressed file (WIP): TitanUnpacker.exe -zlib file.bin
## Changelog
### 1.0
* Initial release
## WIP
* Full zlib support (unpack another zlib streams and repack all zlib files).
* Big endian support.
## Thanks to Leeg and IlDucci for some help.