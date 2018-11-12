using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Delight.Core.Extensions
{
    public static class VisualTreeHelperEx
    {
        public static IEnumerable<T> FindVisualParents<T>(this DependencyObject element, bool findAll = true)
            where T : DependencyObject
        {
            return Finds<T>(element, ParentSetter, findAll);
        }

        private static void ParentSetter(DependencyObject visual, Queue<DependencyObject> visualQueue)
        {
            var parent = VisualTreeHelper.GetParent(visual);

            if (parent != null)
                visualQueue.Enqueue(parent);
        }

        public static IEnumerable<T> FindVisualChildrens<T>(this DependencyObject element, bool findAll = true)
            where T : DependencyObject
        {
            return Finds<T>(element, ChildrenSetter, findAll);
        }

        private static void ChildrenSetter(DependencyObject visual, Queue<DependencyObject> visualQueue)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i);

                if (child is DependencyObject)
                    visualQueue.Enqueue(child);
            }

            if (visual is ItemsControl control)
            {
                foreach (object item in control.Items)
                {
                    if (item is Visual v)
                    {
                        visualQueue.Enqueue(v);
                    }
                    else
                    {
                        var container = control.ItemContainerGenerator.ContainerFromItem(item);

                        if (container is Visual)
                            visualQueue.Enqueue(container);
                    }
                }
            }
        }

        private static IEnumerable<T> Finds<T>(
            this DependencyObject element,
            Action<DependencyObject, Queue<DependencyObject>> elementSetter,
            bool findAll = true)
            where T : DependencyObject
        {
            var visualQueue = new Queue<DependencyObject>();
            visualQueue.Enqueue(element);

            while (visualQueue.Count > 0)
            {
                DependencyObject visual = visualQueue.Dequeue();

                if (visual is FrameworkElement frameworkElement)
                    frameworkElement.ApplyTemplate();

                if (visual is T result)
                {
                    yield return result;

                    if (!findAll)
                        break;
                }

                elementSetter(visual, visualQueue);
            }
        }
    }
}
