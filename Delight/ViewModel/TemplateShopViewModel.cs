using Delight.Core.Template;
using Delight.Core.Template.Items;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Delight.ViewModel
{
    public class TemplateShopViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TemplateShopViewModel()
        {
            Sources = new ObservableCollection<BaseSource>();
        }

        public void LoadTemplates(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ExternalSources));
            
            var reader = new StreamReader(filePath);
            var sources = serializer.Deserialize(reader) as ExternalSources;

            sources.Sources.ForEach(i => Sources.Add(i));
        }

        public ObservableCollection<BaseSource> Sources { get; private set; }

    }
}
