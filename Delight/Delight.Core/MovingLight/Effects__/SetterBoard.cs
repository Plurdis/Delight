using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects__
{
    public class SetterBoard
    {
        public SetterBoard()
        {
            SetterGroups = new List<SetterGroup>();
            SetterProperties = new List<SetterProperty>();
        }

        public List<SetterGroup> SetterGroups { get; set; }

        public List<SetterProperty> SetterProperties { get; set; }

        public void AddSetterGroup()
        {
            SetterGroups.Add(new SetterGroup());
        }

        public void AddSetterProperties(PortNumber portNumber, string propName)
        {
            SetterProperties.Add(new SetterProperty(portNumber, propName));
        }

        public SetterGroup this[int index] => SetterGroups[index]; 
    }
}
