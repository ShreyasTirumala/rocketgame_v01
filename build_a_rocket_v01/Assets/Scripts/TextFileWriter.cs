using UnityEngine;
using System.Collections;
using System;
using System.IO;
public class TextFileWriter
{
	private const string FILE_NAME = "MyFile.txt";
	public static void Main(String[] args) 
	{
		if (File.Exists(FILE_NAME)) 
		{
			Console.WriteLine("{0} already exists.", FILE_NAME);
			return;
		}
		StreamWriter sr = File.CreateText(FILE_NAME);
		sr.WriteLine ("This is my file.");
		sr.WriteLine ("I can write ints {0} or floats {1}, and so on.", 
		              1, 4.2);
		sr.Close();
	}
}
