# MarkdownWinExtended
A perfect portable small program, for everyone to view your documentation on the fileserver. or whatever you want to use it for.

## Extended?
Yes, I extended this fork with some handy features I needed for how I want to use this software.

### Extensions:
* Command Line Arguments for opening markdown files and setting window title
* Rewrote Navigation to support markdown files with relative paths (relative from where the application is)
* A modified version of MarkdownSharp was used to support codeblocks [modified MarkdownSharp](https://www.nuget.org/packages/MarkdownSharp-GithubCodeBlocks)
* CSS has been edited to sans-serif and DarkMode
* I changed the icon to a banana. Yes I did that, nevermind the reason.

### How to use CMD arguments
You can either do:

`MarkdownWin "path-to-md"` to just open md \
or \
`MarkdownWin "path-to-md" "window-title"` to open md and set window title

(This command only works when the application name matches and you are in the same directory as the application. Unless you make a PATH entry or something)

### Rewrote navigation
The navigation now allows you to link other .md files relative to the applications directory.
So if the MarkdownWin is in a folder, in which is another folder called cool, in which is a md file called bruh.md, you can call it with the path *cool/bruh.md*. Normal web URLs are now opened externally in your default Web Browser

### So what does it look like now?
Well have a look:
![New Preview](Screenshot2.png)

### How to get it?
You could clone the repository into VisualStudio and compile it yourself.
Or you could just [Download](https://github.com/Obama360/MarkdownWinExtended/raw/master/MarkdownWin.exe) the executable exe here.
