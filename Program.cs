﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CopyUpdatedFiles {
	internal class Program {
		private static void Main() {
			Console.WriteLine(@"***********************************************************************************
***
***This Application will copy all the files whose last modified time is more than provided time
***This excluded .vs folder, .dll and .exe, .exe.config, .cache, .CopyComplete, .user, .pdb, FileListAbsolute.txt files
***This also list down the file which have todos
***Will be useful while sharing the changed files for release
***
***********************************************************************************");
			Console.WriteLine("\n");
			Console.WriteLine("Enter Source Path: ");
			string sourcePath = Console.ReadLine();

			Console.WriteLine("Enter Destination Path: ");
			string destinationPath = Console.ReadLine();

			Console.WriteLine("Enter start time (yyyy-MM-dd hh:mm tt)");
			string startTime = Console.ReadLine();

			DateTime minTime;
			if (Directory.Exists(sourcePath) && DateTime.TryParse(startTime, out minTime)) {
				string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
				if (files.Length > 0) {
					int fileCount = 0;
					IList<string> fileWithTodos = new List<string>();
					foreach (string file in files) {
						if (file.Contains("\\.vs\\") || file.EndsWith(".dll") || file.EndsWith(".exe") || file.EndsWith(".exe.config") || file.EndsWith(".cache") || file.EndsWith(".CopyComplete") || file.EndsWith(".user") || file.EndsWith(".pdb") || file.EndsWith(".FileListAbsolute.txt"))
							continue;

						FileInfo info = new FileInfo(file);
						if (info.LastWriteTime > minTime) {
							string destinationFileName = file.Replace(sourcePath, destinationPath);
							string directoryName = Path.GetDirectoryName(destinationFileName);
							if (Directory.Exists(directoryName) == false)
								Directory.CreateDirectory(directoryName);

							File.Copy(file, destinationFileName, true);
							string content = File.ReadAllText(destinationFileName);
							if (content.Contains("todo"))
								fileWithTodos.Add(destinationFileName);
							fileCount++;
						}
					}

					Console.WriteLine("{0} files copied", fileCount);
					if (fileWithTodos.Count > 0) {
						Console.WriteLine("{0} files has todos, below are the file names", fileWithTodos.Count);
						foreach (string fileName in fileWithTodos)
							Console.WriteLine(fileName);
					}

					Console.WriteLine("Press any key to exit");
					Console.ReadKey();
				}
			}
		}
	}
}