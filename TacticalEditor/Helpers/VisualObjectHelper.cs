﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Serialization;
using TacticalEditor.Models;
using TacticalEditor.Models.ModelXml;
using TacticalEditor.Models.Points;
using TacticalEditor.VisualObject;

namespace TacticalEditor.Helpers
{
    class VisualObjectHelper
    {
        private readonly Canvas _plotter;
        private readonly CoordinateHelper _coordinateHelper;
        private int _countNavigationPoint = 1;
        private Line _lastLine;
        private AirportPoint _currentAirport;
        private RoutePoints _routePoints;
        private List<AirPoint> _airPoints;


        public VisualObjectHelper(Canvas plotter)
        {
            _plotter = plotter;
            _plotter.SizeChanged += PlotterOnSizeChanged;
            _coordinateHelper = new CoordinateHelper();
            _routePoints = new RoutePoints();
            _airPoints = new List<AirPoint>();
            EventsHelper.AddLineToRouteEvent += AddLineToRouteEvent;
            EventsHelper.ChangeAirportEvent += ChangeAirportEvent;
            LoadAirports();
        }

        public void AddVisualPpm(Point point)
        {
            var sizeMap = (uint) _plotter.Height;
            _coordinateHelper.PixelToLatLon(point, sizeMap, out var lat, out var lon);
            PpmPoint ppmPoint = new PpmPoint();
            ppmPoint.Lat = lat;
            ppmPoint.Lon = lon;
            ppmPoint.NumberInRoute = _countNavigationPoint;
            ppmPoint.Screen.RelativeX = point.X / sizeMap;
            ppmPoint.Screen.RelativeY = point.Y / sizeMap;
            ppmPoint.Screen.SizeMap = sizeMap;
            ppmPoint.Screen.RouteLineIn = _lastLine;
            _countNavigationPoint++;
            _routePoints.PPM.Add(new Ppm {RelativeX = point.X / sizeMap, RelativeY = point.Y / sizeMap});
            _airPoints.Add(ppmPoint);
            EventsHelper.OnPpmCollectionEvent(_airPoints);
            AddVisualToPlotter(new VisualPpm(ppmPoint), point);
        }

        public void Clear()
        {
            for(int i = _plotter.Children.Count - 1; i >= 0; i--)
            {
                if(_plotter.Children[i] is VisualAirport) continue;
                _plotter.Children.Remove(_plotter.Children[i]);
            }
            ChangeAirportEvent(_currentAirport);
            _countNavigationPoint = 1;
            _routePoints.PPM.Clear();
            _airPoints.Clear();
        }

        public void SaveRoute(string path)
        {
            using(var writer = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(typeof(RoutePoints));
                serializer.Serialize(writer, _routePoints);
                writer.Flush();
            }
        }

        public void OpenRoute(string path)
        {
            Clear();
            RoutePoints routePoints;
            XmlSerializer serializer = new XmlSerializer(typeof(RoutePoints));
            using(FileStream fileStream = new FileStream(path, FileMode.Open))
                routePoints = (RoutePoints)serializer.Deserialize(fileStream);
            var sizeMap = (uint)_plotter.Height;
            foreach (var point in routePoints.PPM)
               AddVisualPpm(new Point(point.RelativeX * sizeMap, point.RelativeY * sizeMap));
        }

        private void AddLineToRouteEvent(Line oldLine, Line newLine)
        {
            _plotter.Children.Add(oldLine);
            _lastLine = newLine;
        }

        private void AddVisualToPlotter(UIElement ui, Point point)
        {
            _plotter.Children.Add(ui);
            Canvas.SetLeft(ui, point.X);
            Canvas.SetTop(ui, point.Y);
            Panel.SetZIndex(ui, 10);
        }

        private void ChangeAirportEvent(AirportPoint airportPoint)
        {
            _lastLine = airportPoint.Screen.RouteLineOut;
            _currentAirport = airportPoint;
        }

        private void PlotterOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var sizeMap = (uint) e.NewSize.Height;
            EventsHelper.OnChangeOfSizeEvent(sizeMap);
        }

        private void LoadAirports()
        {

            string[] fileEntries = Directory.GetFiles("Airports");
            foreach (string fileName in fileEntries)
            {
                Airport air;
                XmlSerializer serializer = new XmlSerializer(typeof(Airport));
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                    air = (Airport) serializer.Deserialize(fileStream);
                AddVisualAirport(air);
            }
        }

        private void AddVisualAirport(Airport airport)
        {
            var sizeMap = (uint)_plotter.Height;
            _coordinateHelper.LatLonToPixel(airport.Local.latitude, airport.Local.longitude, sizeMap, out var px, out var py);
            AirportPoint airportPoint = new AirportPoint();
            if(airport.Local.name == "Lipetsk")
                airportPoint.ActiveAirport = true;
            airportPoint.Lat = airport.Runway.latitude;
            airportPoint.Lon = airport.Runway.longitude;
            airportPoint.HeadingRunway = airport.Runway.heading;
            airportPoint.Screen.SizeMap = sizeMap;
            airportPoint.Screen.RelativeX = px;
            airportPoint.Screen.RelativeY = py;
            AddVisualToPlotter(new VisualAirport(airportPoint), new Point(px, py));
        }
    }
}