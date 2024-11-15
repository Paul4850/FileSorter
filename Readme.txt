The solution contains two console applications.
1. FileCreator.
Generates a file of a given size in Mb. 
Example:
FileCreator.exe 1024

Default size is 1.

The tool uses a file with words to generate string. The file must be provided in appsettings.json:

{
  "WordsFileName": "Resources\\wordlist.txt",
  "OutputPath": "D:\\input\\"
}

2. FileSorter. 
Creates a sorted file next to a given file. Arguments: fileName, [optional] chunkSize
Default chunk size is 500000 lines.

Example:
FileSorter d:\input\source_10240Mb.txt

/////////////////////////////////////

Assumptions:

Lines format:
Numeric part is a random long integer. 
String part consists of latin alphabetic symbols and spaces only. Its length is from zero to 1024 symbols. The words are randomly taken from a dictionary of 70.000 words.
No empty lines.
Encoding: ASCII.

Hardware:
CPU Intel(R) Core(TM) i5-10210U CPU @ 1.60GHz, 2112 Mhz, 4 Core(s), 8 Logical Processor(s)
RAM: 16 GB
SSD drive
OS: Windows 11

Performance test reults:

1Gb
File d:\input\source_1024Mb.txt, chunk 500000
11/15/2024 4:01:21 PM
SplitAndSortChunks: 00:00:06
MergeChunks: 00:00:04
Deleting files
Files deleted: 00:00:00
Total time: 00:00:11
Sorted

10Gb
File d:\input\source_10240Mb.txt, chunk 500000
11/15/2024 3:01:54 PM
SplitAndSortChunks: 00:01:04
MergeChunks: 00:00:51
Deleting files
Files deleted: 00:00:00
Total time: 00:01:56
Sorted

File d:\input\source_10240Mb.txt, chunk 1000000
11/15/2024 3:08:49 PM
SplitAndSortChunks: 00:01:08
MergeChunks: 00:00:48
Deleting files
Files deleted: 00:00:00
Total time: 00:01:57
Sorted

Memory consumption: less than 2Gb for a 1GB file
about 2.6GB for a 10GB file
