## 目录I/O操作

本文将介绍C#处理文件的知识点，在.NET开发领域，文件系统I/O是一个至关重要的主题，尤其是在处理文件、目录和数据存储方面。

C#作为.NET平台的主要编程语言，提供了丰富而强大的文件系统I/O功能，为开发人员提供了灵活的工具，使其能够高效地处理文件操作。本文将介绍C#处理文件系统I/O知识点。

### 1、文件和目录的创建与删除

在C#中，使用`File`和`Directory`类可以轻松地创建和删除文件及目录。开发者应了解如何使用这两个类的方法。

案例如下：

```C#
// 创建文件
File.Create("path/to/file.txt");
// 删除文件
File.Delete("path/to/file.txt");
// 创建目录
Directory.CreateDirectory("path/to/directory");
// 删除目录
Directory.Delete("path/to/directory", true); // 第二个参数表示是否递归删除子目录和文件
```

### 2、文件读写操作

C#提供了强大的文件读写功能，开发者需要熟悉StreamReader和StreamWriter等类，以实现对文件的读写操作。

下面是一个简单的例子：

```C#
// 读取文件内容
using (StreamReader reader = new StreamReader("path/to/file.txt"))
{
    string content = reader.ReadToEnd();
    Console.WriteLine(content);
}

// 写入文件内容
using (StreamWriter writer = new StreamWriter("path/to/file.txt"))
{
    writer.WriteLine("Hello, C# File I/O!");
}
```

### 3、文件复制和移动

在处理文件时，复制和移动是常见的操作。C#提供了File.Copy和File.Move等方法，可以轻松实现文件的复制和移动：

```C#
// 复制文件
File.Copy("source/path/file.txt", "destination/path/file.txt");

// 移动文件
File.Move("old/path/file.txt", "new/path/file.txt");
```

### 4、文件信息和属性

使用FileInfo类可以获取文件的详细信息和属性，例如文件大小、创建时间等：

```C#
FileInfo fileInfo = new FileInfo("path/to/file.txt");
Console.WriteLine($"File Size: {fileInfo.Length} bytes");
Console.WriteLine($"Creation Time: {fileInfo.CreationTime}");
```

### 5、目录遍历

了解如何遍历目录以获取文件列表是一个重要的技能。Directory类提供了GetFiles`和GetDirectories方法，可以返回指定目录下的文件和子目录。

案例如下：

```C#
// 获取所有文件
string[] files = Directory.GetFiles("path/to/directory");

// 获取所有子目录
string[] directories = Directory.GetDirectories("path/to/directory");
```

### 6、异常处理

在进行文件系统I/O操作时，处理可能发生的异常是不可或缺的。可能的异常包括文件不存在、权限不足等。使用try-catch块来捕获这些异常，以确保应用程序的稳定性。

```C#
try
{
    // 文件操作代码
}
catch (IOException ex)
{
    Console.WriteLine($"An IO exception occurred: {ex.Message}");
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Unauthorized access: {ex.Message}");
}
```

### 7、文件存在性检查

在C#可以使用`File.Exists`方法检查文件是否存在。

```C#
if (File.Exists("path/to/file.txt")){// 文件存在，执行相应操作}
```

### 8、路径操作

路径对于读取自定义配置文件等有非常重要的作用。在C#可以使用`Path`类来进行路径的合并、获取文件名等操作。

```C#
codestring fullPath = Path.Combine("folder", "subfolder", "file.txt");
string fileName = Path.GetFileName(fullPath);
```

### 9、异步文件读写

可以利用C#中的`StreamReader`和`StreamWriter`的异步方法，实现异步文件读写操作。

```C#
// 异步读取文件
using (StreamReader reader = new StreamReader("path/to/file.txt"))
{    
string content = await reader.ReadToEndAsync();    
Console.WriteLine(content);
}
// 异步写入文件
using (StreamWriter writer = new StreamWriter("path/to/file.txt"))
{    
await writer.WriteLineAsync("Hello, C# File I/O!");
}
```

### 10、 特殊文件夹路径获取

可以使用`Environment.SpecialFolder`枚举和`Environment.GetFolderPath`方法获取特殊文件夹的路径。下面案例是获取桌面文件路径。

```C#
string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
Console.WriteLine($"Desktop Path: {desktopPath}");
```

### 11、文件属性设置

使用`File.SetAttributes`方法设置文件属性，例如将文件设置为只读等。案例如下：

```C#
File.SetAttributes("path/to/file.txt", FileAttributes.ReadOnly);
```

### 12、文件锁定检查

大家常常会遇到文件锁定的问题不能读写文件，在C#中可以用以下方法检查文件是否被其他进程锁定。

```C#
private static bool IsFileLocked(string filePath)
{
    try
    {
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {           
         // 文件未被锁定
         return false;     
        }    
    }    
    catch (IOException)    
    {        
         // 文件被锁定        
         return true; 
    }
}
//调用bool isFileLocked = IsFileLocked("path/to/file.txt");
 //当然还有其它方法，见文章：《C#判断文件是否占用的2种方法》
```

### 13、文件流操

使用`FileStream`进行文件流操作，例如读取和写入文件。

```C#
using (FileStream fs = new FileStream("path/to/file.txt", FileMode.Open, FileAccess.Read))
{    
// 执行文件流操作
}
```

### 14、监视文件变化

在C#中可以使用`FileSystemWatcher`类监视文件变化，例如文件内容的修改。

```C#
FileSystemWatcher watcher = new FileSystemWatcher("path/to/directory");
watcher.EnableRaisingEvents = true;
watcher.Changed += (sender, e) => Console.WriteLine($"File {e.FullPath} changed");
```

### 15、文件内容比较

怎么比较两个文件呢？可以使用File.ReadAllBytes方法转换成字节，然后用SequenceEqual方法来比较两个文件是否相同。案例如下：

```C#
private static bool FileEquals(string filePath1, string filePath2) 
{ 
    byte[] file1 = File.ReadAllBytes(filePath1); 
    byte[] file2 = File.ReadAllBytes(filePath2); return file1.SequenceEqual(file2); 
}
//调用bool areFilesEqual = FileEquals("file1.txt", "file2.txt");
```

### 16、文件压缩与解压缩

在c#中可以使用`ZipFile`类进行文件压缩和解压缩操作，目前官方只支持zip文件。案例如下：

```C#
ZipFile.CreateFromDirectory("source/path", "archive.zip");ZipFile.ExtractToDirectory("archive.zip", "destination/path");
```

### 17、文件路径规范化（文件路径）

使用`Path.GetFullPath`方法规范化文件路径，解析相对路径等。在项目中可以获取完整路径。

```C#
string normalizedPath = Path.GetFullPath("path/to/../file.txt");
```

### 18、使用MemoryMappedFile进行内存映射文件操作

利用`MemoryMappedFile`进行大文件的内存映射操作，提高文件读写性能。

```C#
using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile("path/to/file.txt"))
{    
   // 执行内存映射文件操作
}
```

### 19、文件流异步操作

使用`FileStream`的异步方法进行文件流的异步读写操作。

```C#
using (FileStream fs = new FileStream("path/to/file.txt", FileMode.Open, FileAccess.Read))
{
    byte[] buffer = new byte[1024]; int bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length);    // 处理读取的数据
}
```

### 20、使用TransactionScope进行文件事务操作

使用`TransactionScope`进行多个文件操作的事务管理，确保一组文件操作要么全部成功，要么全部失败。

```C#
using (TransactionScope scope = new TransactionScope())
{
    File.Move("old/path/file.txt", "new/path/file.txt");
    // 其他事务操作
    scope.Complete();
}
```