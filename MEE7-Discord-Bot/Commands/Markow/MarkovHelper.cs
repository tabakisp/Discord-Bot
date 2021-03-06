﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MEE7.Commands
{
    public class NoEmptyElementException : Exception { public NoEmptyElementException(string message) : base(message) { } }

    public static class MarkovHelper
    {
        const byte inputLength = 1;
        static readonly string savePath = "markow" + inputLength + ".json";
        static Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

        public static bool SaveFileExists()
        {
            return File.Exists(savePath);
        }
        public static void SaveDict()
        {
            File.WriteAllText(savePath, JsonConvert.SerializeObject(dict));
        }
        public static void LoadDict()
        {
            if (SaveFileExists())
                dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(savePath));
        }

        public static void AddToDict(string addition)
        {
            lock (dict)
            {
                addition = addition.Replace("\n", " \n ").Trim(' ');

                List<string> presplit = new List<string>();
                presplit.AddRange(Enumerable.Repeat("", inputLength));
                presplit.AddRange(addition.Split(' '));

                Add(presplit.ToArray());
            }
        }
        public static void AddToDict(string preText, string addition)
        {
            lock (dict)
            {
                addition = addition.Replace("\n", " \n ").Trim(' ');
                preText = (preText + "\n").Replace("\n", " \n ").Trim(' ');

                string[] preTextSplit = preText.Split(' ');

                List<string> presplit = new List<string>();
                if (inputLength - preTextSplit.Length > 0)
                    presplit.AddRange(Enumerable.Repeat("", inputLength - preTextSplit.Length));
                presplit.AddRange(preTextSplit.Skip(Math.Max(0, preTextSplit.Length - inputLength)));
                presplit.AddRange(addition.Split(' '));

                Add(presplit.ToArray());
            }
        }
        static void Add(string[] processedText)
        {
            List<string> list = null;
            string[] split = processedText;
            for (int i = 0; i < split.Length - inputLength; i++)
            {
                if (!string.IsNullOrWhiteSpace(split[i + inputLength]) && split[i + inputLength] != split[i + inputLength - 1])
                {
                    string key = split.ToList().GetRange(i, inputLength).Aggregate((x, y) => { return x + " " + y; }).Trim(' ');
                    if (dict.TryGetValue(key, out list))
                        list.Add(split[i + inputLength]);
                    else
                        dict.Add(key, new string[] { split[i + inputLength] }.ToList());
                }
            }
            if (dict.TryGetValue("", out list))
            {
                list.Clear();
                list.Add("\n");
            }
            if (dict.TryGetValue("\n", out list))
                list.RemoveAll(x => x == "" || x == "\n");
        }

        public static string GetString(string start, int minLength, int maxLength)
        {
            lock (dict)
            {
                if (string.IsNullOrWhiteSpace(start))
                    start = dict.Keys.ElementAt(Program.RDM.Next(dict.Keys.Count));
                List<string> outputList = start.Replace("\n", " \n ").Trim(' ').Split(' ').ToList();

                for (int i = 0; i < minLength; i++)
                    AddWord(outputList);
                while (!outputList.Last().EndsWith(".") && !outputList.Last().EndsWith("!") && !outputList.Last().EndsWith("?") && !outputList.Last().EndsWith("\n"))
                    AddWord(outputList);

                string output = outputList.Aggregate((x, y) => { return x + " " + y; });
                if (output.Length > maxLength)
                    output = output.Substring(0, maxLength);

                return output;
            }
        }
        static void AddWord(List<string> output)
        {
            string key = output.Skip(Math.Max(0, output.Count() - inputLength)).Aggregate((x, y) => { return x + " " + y; });
            if (dict.TryGetValue(key, out List<string> list))
                output.Add(list.ElementAt(Program.RDM.Next(list.Count)));
            else
            {
                if (dict.TryGetValue("", out list))
                    output.Add(list.ElementAt(Program.RDM.Next(list.Count)));
                else
                    throw new NoEmptyElementException("No \"\" Element!?");
            }
        }
    }
}