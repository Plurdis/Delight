using Delight.Core.Sources;
using Delight.Core.Template;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Delight.ViewModel
{
    public class ExternalSourceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ExternalSourceViewModel()
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //if (ofd.ShowDialog().Value)
            //{
            //    LoadTemplates(ofd.FileName);
            //}

            Sources = new ObservableCollection<BaseSource>();

            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=0pXYp72dwl0&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&ab_channel=MatthiasM.de"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=nM0xDI5R50E&ab_channel=1theK%28%EC%9B%90%EB%8D%94%EC%BC%80%EC%9D%B4%29"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=BLgqyQMjd5s&ab_channel=Eve"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=PvzBWFGEz8M&ab_channel=Eve"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=nROvY9uiYYk&ab_channel=Eve"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=layW4vGC2uM&ab_channel=piano77man77"));
            //AddFromYoutubeSource(ys);
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
