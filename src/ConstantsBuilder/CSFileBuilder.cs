using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstantsBuilder
{
    public static class CSFileBuilder
    {
        public const string StructPrefix = "public struct ";
        public static List<string> ConvertSerializedStructure(DeserializedStructure structure, string space = "")
        {
            Console.Write(space + "+ Struct ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(structure.StructName);
            Console.ResetColor();

            bool isConditional = false;

            var strings = new List<string>();

            if(structure.StructName.Contains("<"))
            {
                var s = structure.StructName.Replace(" ", "");
                var filtered = structure.StructName.Split('<', '>');
                strings.Add($"\n#if {filtered[1]}\n\n");
                isConditional = true;
                strings.Add(space + StructPrefix + filtered[2] + "\n");
                
            }
            else
                strings.Add(space + StructPrefix + structure.StructName + "\n");

            strings.Add(space + "{\n");

            if (structure.Constants != null)
            {
                var myDict = new Dictionary<string, List<string>>();

                Console.WriteLine(space + $"   + Conditional Fields");

                foreach (var cconstant in structure.Constants.FindAll(s => s.StartsWith("<")))
                {
                    var withoutWhitespace = cconstant.Replace(" ", "");

                    var filtered = withoutWhitespace.Split(new[] { '<', '>' });

                    if(filtered[1].Equals(string.Empty)) Console.WriteLine("EmptyString!!!!");

                    if(!myDict.ContainsKey(filtered[1]))
                    {
                        myDict.Add(filtered[1], new List<string>{filtered[2]});
                    }
                    else
                    {
                        if(myDict.TryGetValue(filtered[1],out List<string> buffer))
                        {
                            buffer.Add(filtered[2]);
                        }
                    }

                }

                int index = 0;
                int max = myDict.Count;


                foreach (var KVP in myDict)
                {
                    if(index == 0)
                        strings.Add($"\n#if {KVP.Key}\n\n");
                    else
                        strings.Add($"\n#elif {KVP.Key}\n\n");

                    foreach (var constant in KVP.Value)
                    {
                            string con = constant.Replace(" ","");
                            if (con.Contains("="))
                            {
                                var s = con.Split('=');
                                strings.Add(space + "\tpublic const string " + s[0] + " = " + $"\"{s[1]}\";\n");

                            }   
                            else
                                strings.Add(space + "\tpublic const string " + con + " = " + $"\"{con}\";\n");
                    }


                    if(index == max -1)
                        strings.Add("\n#endif\n\n");
                    index++;

                }

                Console.WriteLine(space + $"   + Unconditional Fields");

                foreach (var constant in structure.Constants.FindAll(s => !s.StartsWith("<")))
                {
                    string con = constant.Replace(" ", "");
                    if (con.Contains("="))
                    {
                        var s = con.Split('=');
                        strings.Add(space + "\tpublic const string " + s[0] + " = " + $"\"{s[1]}\";\n");

                    }
                    else
                        strings.Add(space + "\tpublic const string " + con + " = " + $"\"{con}\";\n");
                }
            }

            strings.Add("\n");

            Console.WriteLine(space + $"   + Sub Structs");

            foreach (var serializedStructure in structure.SubStruct)
            {
                strings.AddRange(ConvertSerializedStructure(serializedStructure,space + "\t"));
            }

            

            strings.Add( space + "}\n");

            if(isConditional)
            {
                strings.Add("\n#endif\n\n");
            }

            return strings;
        }


        public static List<string> AddNameSpace(List<string> CSCode, string Namespace)
        {
            var l = new List<string>();
            l.AddRange(new List<string>{$"namespace {Namespace}\n","{\n"});
            foreach (var line in CSCode)
            {
                if(line.StartsWith("#"))
                    l.Add(line);
                else
                {
                    l.Add("\t" + line);
                }
            }

            l.Add("}\n");

            Console.WriteLine($"Added Namespace {Namespace}");

            return l;
        }


    }
}
