﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConstantsBuilder
{

    


    class Program
    {
        
        public static string AutoGenText(string JsonFileName, string csfile)
        {
            return "\n\n"
                   + "// This File is AUTO-GENERATED by ConstantsBuilder.exe!!\n"
                   + "// Do not change this file manually, otherwise changes may get lost if ConstantsBuilder\n"
                   + "// regenerate the File.\n"
                   + $"// If you need to change values please look at the Json Build Document \"{JsonFileName}\"!\n//\n"
                   + $"// File \"{csfile}\" was last updated on {DateTime.Now.ToString("R")}\n\n\n";
        }
        private static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine(" USE -h for Help !!");
                return;
            }

            if(args.Contains("-h"))
            {
                Console.WriteLine("CommandBuilder.exe <JsonFilePath> <CSFilePath> <params>");
                Console.WriteLine("-N <NameSpace>");
            }

            string JsonSourceFile;
            string CSFile;

            if(args[0].Contains(@":\"))
                JsonSourceFile = args[0];
            else
            {
               JsonSourceFile =  Directory.GetCurrentDirectory() + @"\" + args[0];
            }

            if (args[1].Contains(@":\"))
                CSFile = args[1];
            else
            {
                CSFile = Directory.GetCurrentDirectory() + @"\"+ args[1];
            }

            if(args.Length > 2)
            {
                if(args[2] == "-N")
                {
                    ConvertJsonToCsFile(JsonSourceFile,CSFile,args[3]);
                }
            }
            else

                ConvertJsonToCsFile(JsonSourceFile, CSFile);
        }
        private static void CreateJsonFile(string PathandFileName)
        {
            var obj = new DeserializedStructure();
            obj.StructName = "Constants";
            obj.SubStruct = new List<DeserializedStructure>();



            var obj1 = new DeserializedStructure();
            obj1.StructName = "<MyCondition>SubConstants";
            obj1.Constants = new List<string> { "constant1", "constant2 = Value" };
            obj.SubStruct.Add(obj1);

            var obj2 = new DeserializedStructure();
            obj2.StructName = "SubsubConstants";
            obj2.Constants = new List<string> { "<MyPreprocessorCondition>constant1" };
            obj1.SubStruct.Add(obj2);

            var obj3 = new DeserializedStructure();
            obj3.StructName = "SubSubSubConstants";
            obj3.Constants = new List<string> { "myConstant" };
            obj2.SubStruct.Add(obj3);

            var ob = new DeserializedDocument();
            ob.Document = new List<DeserializedStructure>{obj};

            string str = JsonConvert.SerializeObject(ob, Formatting.Indented);

            File.WriteAllText(PathandFileName,str);

        }


        private static void ConvertJsonToCsFile(string JsonFilePath, string CSFilePath, string Namespace = "")
        {
            if(!File.Exists(JsonFilePath))
            {
                CreateJsonFile(JsonFilePath);
            }

            var x = JsonConvert.DeserializeObject<DeserializedDocument>(File.ReadAllText(JsonFilePath));


            List<string> stings = new List<string>();

            var jsonf = Path.GetFileName(JsonFilePath);
            var csf = Path.GetFileName(CSFilePath);


            

            foreach (var deserializedStructure in x.Document)
            {
                stings.AddRange(CSFileBuilder.ConvertSerializedStructure(deserializedStructure));
            }


            string output;

            if (!string.IsNullOrEmpty(Namespace))
                stings = CSFileBuilder.AddNameSpace(stings, Namespace);


            stings.Insert(0,AutoGenText(jsonf,csf));

            output = string.Join("", stings);


            if(!Directory.Exists(CSFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CSFilePath));
            }

            File.WriteAllText(CSFilePath, output);

        }

    }

}