namespace VMTranslator
{
    /*
       Aritmetik / logical kommandoer
       add
       sub
       neg
       eq
       gt
       lt
       and
       or
       not
    
       Memory access kommandoer
       pop segment i
       push segment i
       
     */
    class Program
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\jaku0065\\Desktop\\nand2tetris\\nand2tetris\\projects\\08\\";
            string[] file = ReadFile(path);
            file = RemoveComments(file);

            string[] newFile = Array.Empty<string>();
            newFile = Parse(file);
            SaveFile(newFile);
        }
        // Read file and return array of string lines
        private static string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }
        
        // Save file to output.asm in current directory
        private static void SaveFile(string[] file)
        {
            File.WriteAllLines(Environment.CurrentDirectory + "\\output.asm", file);
        }
        
        // Remove comments from file
        private static string[] RemoveComments(string[] file)
        {
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i].Contains("//"))
                {
                    file[i] = file[i].Remove(file[i].IndexOf("//"));
                }
            }
            return file;
        }

        // Add starting pointers to file
        // stack pointer and base pointers
        private static string[] StartingPointers(string[] file)
        {
            Dictionary<string, string> pointers = new Dictionary<string, string>()
            {
                { "SP", "256" },
                { "LCL", "300" },
                { "ARG", "400" },
                { "THIS", "3000" },
                { "THAT", "3010" },
            };
            
            foreach (var pointer in pointers)
            {
                file = file.Append($"@{pointer.Value}").ToArray();
                file = file.Append($"D=A").ToArray();
                file = file.Append($"@{pointer.Key}").ToArray();
                file = file.Append($"M=D").ToArray();
            }
            return file;
        }

        // Find index of key in dictionary
        private static int FindIndex(Dictionary<string, string[]> dictionary,string key)
        {
            int index = 0;

            for (int i = 0; i < dictionary.Count; i++)
            {
                if (dictionary.ElementAt(i).Key == key)
                {
                    index = i;
                    i = dictionary.Count;
                }
            }

            return index;
        }

        private static string[] AddToFile(Dictionary<string, string[]> arguments,string key,string[] file)
        {
            foreach (var s in arguments.ElementAt(FindIndex(arguments, key)).Value)
            {
                file = file.Append($"" + s).ToArray();
            }

            return file;
        }
        
        private static string[] Parse(string[] asmFile)
        {
            Dictionary<string, string[]> pushArguments = new Dictionary<string, string[]>()
            {
                { "constant",new string[] { "D=A","@SP","A=M","M=D","@SP","M=M+1"} },
                { "temp",new string[] { "D=A","@5","A=A+D","D=M","@SP","A=M","M=D","@SP","M=M+1"} },
                { "pointer",new string[] { "D=A","@3","A=A+D","D=M","@SP","A=M","M=D"} },
            };
            Dictionary<string, string[]> popArguments = new Dictionary<string, string[]>()
            {
                { "temp",new string[] { "D=A","@5","D=A+D","@R13","M=D","@SP","AM=M-1","D=M","@R13,","A=M","M=D"} },
                { "pointer",new string[] { "D=A","@3","D=A+D","@R13","M=D","@SP","AM=M-1","D=M","@R13,","A=M","M=D"} },
            };
            Dictionary<string, string[]> arguments = new Dictionary<string, string[]>()
            {
                { "add", new string[] { "@SP", "AM=M-1", "D=M", "A=A-1","M=D+M" } },
                { "sub", new string[] { "@SP", "AM=M-1", "D=M", "A=A-1","M=M-D" } },
                { "neg", new string[] { "@SP", "A=M-1", "M=-M" } },
                { "eq", new string[] { "@SP", "AM=M-1", "D=M", "A=A-1", "D=M-D", "@TRUE", "D;JEQ", "@SP", "A=M-1", "M=0", "@END", "0;JMP", "(TRUE)", "@SP", "A=M-1", "M=-1", "(END)" } },
                { "gt", new string[] { "@SP", "AM=M-1", "D=M", "A=A-1", "D=M-D", "@TRUE", "D;JGT", "@SP", "A=M-1", "M=0", "@END", "0;JMP", "(TRUE)", "@SP", "A=M-1", "M=-1", "(END)" } },
                { "lt", new string[] { "@SP", "AM=M-1", "D=M", "A=A-1", "D=M-D", "@TRUE", "D;JLT", "@SP", "A=M-1", "M=0", "@END", "0;JMP", "(TRUE)", "@SP", "A=M-1", "M=-1", "(END)" } },
                { "and", new string[] { "@SP", "AM=M-1", "D=M", "A=A-1", "M=D&M" } },
                { "or", new string[] { "@SP", "AM=M-1", "D=M", "A=A-1", "M=D|M" } },
                { "not", new string[] { "@SP", "A=M-1", "M=!M" } },
            };

            int returnCounter = 0;
            string[] file = Array.Empty<string>();
            file = StartingPointers(file);
            foreach (var line in asmFile)
            {
                if (line.Contains("push"))
                {
                    string[] split = line.Split(" "); //Push 
                    string argument = split[1]; // Argument 
                    string index = split[2]; //Index

                    if (argument == "constant")
                    {
                        file = file.Append($"@{index}").ToArray();
                        file = AddToFile(pushArguments, argument, file);
                    }
                    else if (argument == "local" || argument == "argument" || argument == "this" || argument == "that")
                    {
                        file = file.Append($"@{index}").ToArray();
                        file = file.Append($"D=A").ToArray();
                        file = file.Append($"@{argument}").ToArray();
                        file = file.Append($"A=M+D").ToArray();
                        file = file.Append($"D=M").ToArray();
                        file = file.Append($"@SP").ToArray();
                        file = file.Append($"A=M").ToArray();
                        file = file.Append($"M=D").ToArray();
                        file = file.Append($"@SP").ToArray();
                        file = file.Append($"M=M+1").ToArray();
                    }
                    else if (argument == "temp")
                    {
                        file = file.Append($"@{index}").ToArray();
                        file = AddToFile(pushArguments, argument, file);
                    }
                    else if (argument == "pointer")
                    {
                        file = file.Append($"@{index}").ToArray();
                        file = AddToFile(pushArguments, argument, file);
                    }
                }
                else if (line.Contains("pop"))
                {
                    string[] split = line.Split(" "); //Pop
                    string argument = split[1]; //Argument
                    string index = split[2]; //Index
                    if (argument == "local" || argument == "argument" || argument == "this" || argument == "that")
                    {
                        file = file.Append($"@{index}").ToArray();
                        file = file.Append($"D=A").ToArray();
                        file = file.Append($"@{argument}").ToArray();
                        file = file.Append($"D=M+D").ToArray();
                        file = file.Append($"@R13").ToArray();
                        file = file.Append($"M=D").ToArray();
                        file = file.Append($"@SP").ToArray();
                        file = file.Append($"AM=M-1").ToArray();
                        file = file.Append($"D=M").ToArray();
                        file = file.Append($"@R13").ToArray();
                        file = file.Append($"A=M").ToArray();
                        file = file.Append($"M=D").ToArray();
                    }
                    else if (argument == "temp")
                    {
                        file = file.Append($"@{index}").ToArray();
                        file = AddToFile(popArguments, argument, file);
                    }
                    else if (argument == "pointer")
                    {
                        file = file.Append($"@{index}").ToArray();
                        file = AddToFile(popArguments, argument, file);
                    }
                }
                else if (line.Contains("add"))
                {
                    file = AddToFile(arguments, "add", file);
                }
                else if (line.Contains("sub"))
                {
                    file = AddToFile(arguments, "sub", file);
                }
                else if (line.Contains("neg"))
                {
                    file = AddToFile(arguments, "neg", file);
                }
                else if (line.Contains("eq"))
                {
                    file = AddToFile(arguments, "eq", file);
                }
                else if (line.Contains("gt"))
                {
                    file = AddToFile(arguments, "gt", file);
                }
                else if (line.Contains("lt"))
                {
                    file = AddToFile(arguments, "lt", file);
                }
                else if (line.Contains("and"))
                {
                    file = AddToFile(arguments, "and", file);
                }
                else if (line.Contains("or"))
                {
                    file = AddToFile(arguments, "or", file);
                }
                else if (line.Contains("not"))
                {
                    file = AddToFile(arguments, "not", file);
                }

                ////////////////////// VM Part 2 /////////////////////////////

                else if (line.Contains("label"))
                {
                    string[] split = line.Split(" ");
                    string label = split[1];
                    file = file.Append($"({label})").ToArray();
                }
                else if (line.Contains("goto"))
                {
                    string[] split = line.Split(" ");
                    string label = split[1];
                    file = file.Append($"@{label}").ToArray();
                    file = file.Append($"0;JMP").ToArray();
                }
                else if (line.Contains("if-goto"))
                {
                    string[] split = line.Split(" ");
                    string label = split[1];
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"AM=M-1").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@{label}").ToArray();
                    file = file.Append($"D;JNE").ToArray();
                }
                else if (line.Contains("function"))
                {
                    string[] split = line.Split(" ");
                    string functionName = split[1];
                    string numberOfLocals = split[2];
                    file = file.Append($"({functionName})").ToArray();
                    for (int i = 0; i < int.Parse(numberOfLocals); i++)
                    {
                        file = file.Append($"@SP").ToArray();
                        file = file.Append($"A=M").ToArray();
                        file = file.Append($"M=0").ToArray();
                        file = file.Append($"@SP").ToArray();
                        file = file.Append($"M=M+1").ToArray();
                    }
                }
                else if (line.Contains("call"))
                {
                    string[] split = line.Split(" ");
                    string functionName = split[1];
                    string numberOfArguments = split[2];
                    file = file.Append($"@{functionName}$ret.{returnCounter}").ToArray();
                    file = file.Append($"D=A").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"A=M").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"M=M+1").ToArray();
                    
                    file = file.Append($"@LCL").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"A=M").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"M=M+1").ToArray();
                    
                    file = file.Append($"@ARG").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"A=M").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"M=M+1").ToArray();
                    
                    file = file.Append($"@THIS").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"A=M").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"M=M+1").ToArray();
                    
                    file = file.Append($"@THAT").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"A=M").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"M=M+1").ToArray();
                    
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@5").ToArray();
                    file = file.Append($"D=D-A").ToArray();
                    file = file.Append($"@{numberOfArguments}").ToArray();
                    file = file.Append($"D=D-A").ToArray();
                    file = file.Append($"@ARG").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@LCL").ToArray();
                    file = file.Append($"M=D").ToArray();
                    
                    file = file.Append($"@{functionName}").ToArray();
                    file = file.Append($"0;JMP").ToArray();
                    file = file.Append($"({functionName}$ret.{returnCounter})").ToArray();
                    returnCounter++;
                }
                else if (line.Contains("return"))
                {
                    file = file.Append($"@LCL").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@R13").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@5").ToArray();
                    file = file.Append($"A=D-A").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@R14").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"AM=M-1").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@ARG").ToArray();
                    file = file.Append($"A=M").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@ARG").ToArray();
                    file = file.Append($"D=M+1").ToArray();
                    file = file.Append($"@SP").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@R13").ToArray();
                    file = file.Append($"AM=M-1").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@THAT").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@R13").ToArray();
                    file = file.Append($"AM=M-1").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@THIS").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@R13").ToArray();
                    file = file.Append($"AM=M-1").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@ARG").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@R13").ToArray();
                    file = file.Append($"AM=M-1").ToArray();
                    file = file.Append($"D=M").ToArray();
                    file = file.Append($"@LCL").ToArray();
                    file = file.Append($"M=D").ToArray();
                    file = file.Append($"@R14").ToArray();
                    file = file.Append($"A=M").ToArray();
                    file = file.Append($"0;JMP").ToArray();
                }
                else
                {
                    Console.WriteLine(line);
                }
            }
            return file;
        }
    }
}
