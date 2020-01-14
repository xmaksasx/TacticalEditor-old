﻿using System;
using System.Runtime.InteropServices;

namespace TacticalEditor.Models
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class AirPoint: GeodesicCoordinate
    {
        /// <summary>
        /// Тип точки маршрута
        /// </summary>
        public long Type;
    }
}
