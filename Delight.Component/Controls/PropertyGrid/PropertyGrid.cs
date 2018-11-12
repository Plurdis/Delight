using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Delight.Component.Extensions;
using Delight.Core.Common;
using Moda.KString;

namespace Delight.Component.Controls
{
    [TemplatePart(Name = "PART_searchBox", Type = typeof(TextBox))]
    public class PropertyGrid : FilterListView
    {
        public static readonly DependencyProperty SelectedObjectsProperty =
            DependencyHelper.Register();

        public object[] SelectedObjects
        {
            get { return this.GetValue<object[]>(SelectedObjectsProperty); }
            set { SetValue(SelectedObjectsProperty, value); }
        }

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid() : base()
        {
            AddGroupProperty("Category");

            SelectedObjectsProperty.AddValueChanged(this, SelectedObjects_Changed);
        }

        private void SelectedObjects_Changed(object sender, EventArgs e)
        {
            foreach (PropertyGridItemModel item in this)
                if (item.Setter is IDisposable disposable)
                    disposable.Dispose();

            this.Clear();

            if (SelectedObjects?.Length > 0)
            {
                foreach (var group in SelectedObjects
                    .SelectMany(obj => GetHashedProperties(obj))
                    .GroupBy(
                        p => p.HashCode,
                        p => p))
                {
                    if (group.Count() == SelectedObjects.Length)
                    {
                        var prop = group.ElementAt(0).Value;

                        PropertyInfo[] props = group
                            .Select(item => item.Value.Element)
                            .ToArray();

                        // 생성
                        ISetter setter;

                        if (string.IsNullOrEmpty(prop.Attribute.Key))
                            setter = SetterManager.CreateSetter(SelectedObjects, props);
                        else
                            setter = SetterManager.CreateSetter(SelectedObjects, props, prop.Attribute.Key);

                        if (setter == null)
                            continue;

                        this.AddItem(
                            new PropertyGridItemModel(prop, setter));
                    }
                }
            }

            if (SelectedObjects?.Length == 1)
                SetPresentedObject(SelectedObjects[0]);
            else
                SetPresentedObject(null);
        }

        private IEnumerable<(int HashCode, AttributeTuple<DesignElementAttribute, PropertyInfo> Value)> GetHashedProperties(object obj)
        {
            return DesignerManager.GetProperties(obj.GetType())
                .Select(p => (CreatePropertyHash(p), p));
        }

        private int CreatePropertyHash(AttributeTuple<DesignElementAttribute, PropertyInfo> attr)
        {
            return $"{attr.Attribute.Key}{attr.Attribute.DisplayName}{attr.Element.PropertyType.MetadataToken}".GetHashCode();
        }

        private void SetPresentedObject(object obj)
        {
            this.DataContext = obj;
        }

        protected override bool OnFilter(object item)
        {
            return (item as PropertyGridItemModel).Title.KContains(FilterKeyword);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            
            GroupStyle.Add(new GroupStyle()
            {
                ContainerStyle = FindResource("DelightGroupItemStyle") as Style
            });

            // Search bar Binding
            Binding b = BindingHelper.SetBinding(
                GetTemplateChild("PART_searchBox"), TextBox.TextProperty,
                this, FilterKeywordProperty,
                sourceTrigger: UpdateSourceTrigger.PropertyChanged);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (this.View == null)
                return;

            var gridView = (GridView)this.View;

            double totColumnWidth = gridView
                .Columns
                .Cast<StarGridViewColumn>()
                .Sum(c => c.StarWidth);

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                var column = (StarGridViewColumn)gridView.Columns[i];

                column.Width = column.StarWidth / totColumnWidth * sizeInfo.NewSize.Width;
            }
        }
    }
}
