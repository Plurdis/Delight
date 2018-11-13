using Delight.Core.Stage;
using Delight.Core.Stage.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class ComponentsToTextConverter : ValueConverter<ObservableCollection<StageComponent>, string>
    {
        public override string Convert(ObservableCollection<StageComponent> value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder();
            if (value == null)
            {
                sb.AppendLine("아이템이 선택되지 않았습니다.");
                return sb.ToString();
            }

            

            IEnumerable<string> checkedItem = value.Where(i => i.Checked).Select(i => $"● {i.Identifier} ({i.TypeText})");
            if (checkedItem.Count() != 0)
            {
                sb.AppendLine(string.Join(Environment.NewLine + Environment.NewLine, checkedItem));
            }
            else
            {
                sb.AppendLine("아이템이 선택되지 않았습니다.");   
            }

            return sb.ToString();
        }

        public override ObservableCollection<StageComponent> ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
