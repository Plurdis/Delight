using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Delight.Controls;

namespace Delight.ContextMenus
{
    public static class TrackItemMenuManager
    {
        static TrackItemMenuManager()
        {
            ContextMenu = Application.Current.FindResource("TrackItemContextMenu") as ContextMenu;
        }

        public static void Show(TrackItem trackItem)
        {
            ContextMenu.PlacementTarget = trackItem;

            // TODO: Property 추출
            // TODO: StageComponent에 Property 추출을 위한 데이터 넣기
            // TODO: Property 추출을 위한 구조 만들기

            ContextMenu.IsOpen = true;
        }

        public static ContextMenu ContextMenu { get; }
    }
}
