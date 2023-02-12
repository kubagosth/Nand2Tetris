using System.Text;

namespace AssemblerTranslator
{
    internal class Program
    {
        static string path = "C:\\Users\\jaku0065\\Desktop\\nand2tetris\\nand2tetris\\projects\\06\\Pong\\pong.asm";
        static DefinedValues definedValues = new DefinedValues();
        static StringBuilder binaryString = new StringBuilder();
        
        static void Main(string[] args)
        {
            if (binaryString.Length > 0)
            {
                binaryString.Clear();
            }

            if (File.Exists(path))
            {
                string[] file = File.ReadAllLines(path);
                RemoveComments(file);
                AddLabels(file);
                AddVariables(file);
                ConvertFile(file);
            }

            File.WriteAllText(Environment.CurrentDirectory + "/output.hack", binaryString.ToString());
        }

        private static void RemoveComments(string[] file)
        {
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i].Contains("//"))
                {
                    int index = file[i].IndexOf("//");
                    if (index >= 0)
                    {
                        file[i] = file[i].Substring(0, index);
                    }
                }
            }
        }
        private static void AddLabels(string[] file)
        {
            int counter = 0;
            for (int i = 0; i < file.Length; i++)
            {
                if (!string.IsNullOrEmpty(file[i]))
                {
                    if (file[i].Contains('('))
                    {
                        string label = file[i].Split('(', ')')[1];
                        if (!definedValues.aDictionary.ContainsKey(label))
                        {
                            definedValues.aDictionary.Add(label, definedValues.ConvertToBitString(counter));
                            file[i] = "";
                            continue;
                        }
                    }
                    counter++;
                }
            }
        }
        private static void AddVariables(string[] file)
        {
            int counter = 16;
            foreach (var line in file)
            {
                if (line.Contains('@'))
                {
                    if (!char.IsDigit(line[line.IndexOf('@') + 1]))
                    {
                        string variable = line.Split('@', ' ')[1];
                        if (!definedValues.aDictionary.ContainsKey(variable))
                        {
                            definedValues.aDictionary.Add(variable, definedValues.ConvertToBitString(counter));
                            counter++;
                        }
                    }
                }
            }
        }
        private static void ConvertFile(string[] file)
        {
            for (int i = 0; i < file.Length; i++)
            {
                if (!string.IsNullOrEmpty(file[i]))
                {
                    //A instruction
                    if (file[i].Contains('@'))
                    {
                        string variableOrNum = file[i].Split('@', ' ')[1];
                        if (definedValues.aDictionary.ContainsKey(variableOrNum))
                        {
                            binaryString.AppendLine(string.Join("", definedValues.aDictionary[variableOrNum]));
                        }
                        else
                        {
                            binaryString.AppendLine(string.Join("", definedValues.ConvertToBitString(Convert.ToInt32(variableOrNum))));
                        }
                    }
                    //C instruction 
                    else
                    {
                        string dest;
                        string comp;
                        string jump;
                        // first 3 bits are always 111
                        binaryString.Append(111);

                        if (file[i].Contains('='))
                        {
                            dest = file[i].Split(' ', '=')[0];
                        }
                        else
                        {
                            dest = "null";
                        }


                        // if file contains ; then jump is not null
                        // else it is null
                        if (file[i].Contains(';'))
                        {
                            jump = file[i].Split(';', ' ')[1];
                        }
                        else
                        {
                            jump = "null";
                        }
                        // if dest is null and jump is not null then comp is the first word
                        if (dest == "null" && jump != "null")
                        {
                            comp = file[i].Split(' ', ';')[0];
                        }
                        else if (dest != "null" && jump == "null")
                        {
                            comp = file[i].Split('=', ' ')[1];
                        }
                        else
                        {
                            comp = file[i].Split('=', ';')[1];
                        }

                        try
                        {
                            comp = string.Join("", definedValues.compDictionary[comp]);
                            dest = string.Join("", definedValues.destDictionary[dest]);
                            jump = string.Join("", definedValues.jumpDictionary[jump]);
                            binaryString.Append(comp);
                            binaryString.Append(dest);
                            binaryString.AppendLine(jump);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
        }
    }
}
