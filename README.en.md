# SvnPathIndependencyMeasure
[中文](README.md)

## Command Line Options
### --help, -p
Show this help.

### - Progress, -p
Accept an optional integer `n' so that the program reports progress every `n' second. The default value is 10.

### --exclude, -e
Specifies a .NET regular expression that excludes files matched by the regular expression, when the program is calculating independency.

If a commit changes both the target folder and the file that matches `--exclude' outside of the target folder, the commit is also considered as an independent commit.

### --range,-r
Accepting an integer `n' and asks the program to calculate folder independeny from `n' years ago to now.

### --limit,-l
Accept an integer `n' and asks the program to calculate folder independeny of most recent `n' commits.

### The last parameter
Subfolder path of an SVN project

##Examples
```
SvnPathIndependencyMeasure -p 20 -e "test.*\.mdb" -r 5 "D:\Source Code\CSharp\HelloWorld\BusinessObject"
```

Calculate the independency of folder `BusinessObject' in the HelloWorld project. The program reports progress every 20 seconds, ignoring modifications of "Test.*\.MDB" outside of the BusinessObject folder. Only calculate the independency from 5 years ago to now.

```
SvnPathIndependencyMeasure -p -- "D:\Source Code\CSharp\HelloWorld\BusinessObject"
```

Calculate the independency of folder `BusinessObject' in the HelloWorld project. The program reports progress every 10 seconds.
