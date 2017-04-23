# ccl
Utility for counting code line's count.

## Usage

    ccl <RootFolder> <Suffix> [<Suffix>,...] [<Options>]

## Params
RootFolder: Folder to count.

Suffix: Suffix of code file. Start with '.'.

Options:
* --blank-lines, -b       Count blank lines.
* --hidden-folders, -f    Count hidden folders (Start with '.').
* --verbose, -v           Show process log.

## Example

	ccl ProjectRoot cpp c h -v

This will count all **.cpp/.c/.h file**s' **non-blank line**s' count in folder **ProjectRoot**, and **log will be print** during the procedures.

## Binaries
[Windows](http://www.vicey.cn/files/ccl.exe)
