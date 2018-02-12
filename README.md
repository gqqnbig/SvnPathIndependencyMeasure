# SvnPathIndependencyMeasure

## 命令行参数
### --help, -p
显示本帮助

### --progress, -p
接受一个可选的整数n，每隔n秒报告进度。n的默认值是10。

### --exclude, -e
指定一个.NET正则表达式，在计算独立性时排除正则表达式所匹配的文件。

如果一个提交同时修改了目标文件夹和目标文件夹外的与`--exclude`匹配的文件，则该提交也被认为是独立提交。

### --range, -r
接受一个整数n，表示计算n年前到现在的独立性。

### --limit, -l
接受一个整数n，表示计算最后n的提交的独立性。

### 最后一个参数
svn项目的子文件夹路径

## 示例
```
SvnPathIndependencyMeasure -p 20 -e "test.*\.mdb" -r 5 "D:\Source Code\CSharp\HelloWorld\BusinessObject"
```

计算HelloWorld项目里BusinessObject文件夹的独立性，每20秒报告进度，忽略对BusinessObject文件夹外的"test.*\\.mdb"文件修改。只计算5年前到现在的提交的独立性。

```
SvnPathIndependencyMeasure -p -- "D:\Source Code\CSharp\HelloWorld\BusinessObject"
```

计算HelloWorld项目里BusinessObject文件夹的独立性，每10秒报告进度。
