using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblerTranslator
{
    internal class DefinedValues
    {
        public Dictionary<string, string> aDictionary = new();
        public Dictionary<string, string> destDictionary = new()
        {
            {"null", "000" },
            {"M",    "001"  },
            {"D",    "010"  },
            {"MD",   "011"  },
            {"A",    "100"  },
            {"AM",   "101"  },
            {"AD",   "110"  },
            {"AMD",  "111"  },

        };
        public Dictionary<string, string> jumpDictionary = new()
        {
            {"null", "000" },
            {"JGT", "001" },
            {"JEQ", "010" },
            {"JGE", "011" },
            {"JLT", "100" },
            {"JNE", "101" },
            {"JLE", "110" },
            {"JMP", "111" },
        };
        public Dictionary<string, string> compDictionary = new()
        {
            {"0", "0101010" },
            {"1", "0111111" },
            {"-1", "0111010" },
            {"D", "0001100" },
            {"A", "0110000" },
            {"M", "1110000" },
            {"!D", "0001101" },
            {"!A", "0110001" },
            {"!M", "1110001" },
            {"D+1", "0011111" },
            {"A+1", "0110111" },
            {"M+1", "1110111" },
            {"D-1", "0001110" },
            {"A-1", "0110010" },
            {"M-1", "1110010" },
            {"D+A", "0000010" },
            {"D+M", "1000010" },
            {"D-A", "0010011" },
            {"D-M", "1010011" },
            {"A-D", "0000111" },
            {"M-D", "1000111" },
            {"D&A", "0000000" },
            {"D&M", "1000000" },
            {"D|A", "0010101" },
            {"D|M", "1010101" }
        };

        public DefinedValues()
        {
            aDictionary.Add("R0", ConvertToBitString(0));
            aDictionary.Add("R1", ConvertToBitString(1));
            aDictionary.Add("R2", ConvertToBitString(2));
            aDictionary.Add("R3", ConvertToBitString(3));
            aDictionary.Add("R4", ConvertToBitString(4));
            aDictionary.Add("R5", ConvertToBitString(5));
            aDictionary.Add("R6", ConvertToBitString(6));
            aDictionary.Add("R7", ConvertToBitString(7));
            aDictionary.Add("R8", ConvertToBitString(8));
            aDictionary.Add("R9", ConvertToBitString(9));
            aDictionary.Add("R10", ConvertToBitString(10));
            aDictionary.Add("R11", ConvertToBitString(11));
            aDictionary.Add("R12", ConvertToBitString(12));
            aDictionary.Add("R13", ConvertToBitString(13));
            aDictionary.Add("R14", ConvertToBitString(14));
            aDictionary.Add("R15", ConvertToBitString(15));
            aDictionary.Add("SCREEN", ConvertToBitString(16384));
            aDictionary.Add("KBD", ConvertToBitString(24576));
            aDictionary.Add("SP", ConvertToBitString(0));
            aDictionary.Add("LCL", ConvertToBitString(1));
            aDictionary.Add("ARG", ConvertToBitString(2));
            aDictionary.Add("THIS", ConvertToBitString(3));
            aDictionary.Add("THAT", ConvertToBitString(4));
        }
        public string ConvertToBitString(int value)
        {
            string s = Convert.ToString(value, 2); 

            int[] bits = s.PadLeft(16, '0').Select(c => int.Parse(c.ToString())).ToArray();
            return string.Join("", bits);
        }
    }
}
