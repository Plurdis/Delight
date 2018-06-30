using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Debug
{
    public class TestDataPack : ICloneable
    {
        public TestDataPack(string name)
        {
            this.Name = name;
        }
        public int Size { get; set; } = 60 * 24;
        public string Name { get; set; }
        public int Offset { get; set; }

        public object Clone()
        {
            return new TestDataPack(Name)
            {
                Offset = Offset,
            };
        }
    }
}
