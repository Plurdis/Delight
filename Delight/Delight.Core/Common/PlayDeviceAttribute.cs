using System;

namespace Delight.Core.Common
{
    public class PlayDeviceAttribute : Attribute
    {
        public PlayDeviceAttribute(PlayDevice device)
        {
            PlayDevice = device;
        }

        public PlayDevice PlayDevice { get; set; }
    }
}