using System;
using System.Collections.Generic;
namespace ConstantsBuilder
{
    [Serializable]

    public class DeserializedDocument
    {
        public List<DeserializedStructure> Document{ get; set; }
    }


    [Serializable]
    public class DeserializedStructure
    {
        public string StructName { get; set; }
        public List<string> Constants { get; set; }

        public List<DeserializedStructure> SubStruct { get; set; } = new List<DeserializedStructure>();

    }
}
