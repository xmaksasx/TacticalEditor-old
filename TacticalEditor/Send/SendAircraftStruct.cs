﻿using TacticalEditor.Helpers;
using TacticalEditor.Models;
using TacticalEditor.VisualObject.VisAerodrome;

namespace TacticalEditor.Send
{
    class SendAircraftStruct
    {

        private AerodromePoint _aerodromePoint;
        private CoordinateHelper _coordinateHelper;
        private AircraftPosition _aircraft;

        public SendAircraftStruct()
        {
           
            _coordinateHelper = new CoordinateHelper();
            _aircraft= new AircraftPosition();
            _aircraft.Head = _aircraft.GetHead("Aircraft_Position");
            EventsHelper.ChangeAerodromeEvent += ChangeAerodrome;
        }

        private void ChangeAerodrome(AerodromePoint aerodromePoint)
        {
            _aerodromePoint = aerodromePoint;
        }

        public void SetAircraft(AircraftPosition aircraft)
        {
            if (_aerodromePoint == null) return;
            double lat = aircraft.GeoCoordinate.Latitude;
            double lon = aircraft.GeoCoordinate.Longitude;
            double x = aircraft.GeoCoordinate.X;
            double z = aircraft.GeoCoordinate.Z;

            if (aircraft.IsDegree==0)
			{
                _coordinateHelper.LocalCordToLatLon(
                    _aerodromePoint.AerodromeInfo.Runway.Threshold.Latitude, 
                    _aerodromePoint.AerodromeInfo.Runway.Threshold.Longitude, 
                    aircraft.GeoCoordinate.X, 
                    aircraft.GeoCoordinate.Z,
                    out lat, 
                    out lon);
            }
            if (aircraft.IsDegree == 1)
            {
	            _coordinateHelper.LocalCordToXZ(
		            _aerodromePoint.AerodromeInfo.Runway.Threshold.Latitude,
		            _aerodromePoint.AerodromeInfo.Runway.Threshold.Longitude,
		            aircraft.GeoCoordinate.Latitude,
		            aircraft.GeoCoordinate.Longitude,
		            out x,
		            out z);
            }

            _aircraft.Kren = aircraft.Kren;
            _aircraft.Risk = aircraft.Risk;
            _aircraft.Tang = aircraft.Tang;
            _aircraft.HLand = aircraft.GeoCoordinate.H;
            _aircraft.GeoCoordinate.Latitude = lat;
            _aircraft.GeoCoordinate.Longitude = lon;
            _aircraft.GeoCoordinate.X = x;
            _aircraft.GeoCoordinate.Z = z;
            _aircraft.GeoCoordinate.H = aircraft.GeoCoordinate.H;
            _aircraft.V = aircraft.V;

            EventsHelper.OnChangeAircraftCoordinateEvent(_aircraft);
        }



        public byte[] GetByte(AircraftPosition aircraft)
        {
            _coordinateHelper.LocalCordToLatLon(_aerodromePoint.AerodromeInfo.Runway.Threshold.Latitude,_aerodromePoint.AerodromeInfo.Runway.Threshold.Longitude, aircraft.GeoCoordinate.X, aircraft.GeoCoordinate.Z,
                out var lat, out var lon
            );
            
            _aircraft.Kren = aircraft.Kren;
            _aircraft.Risk = aircraft.Risk;
            _aircraft.Tang = aircraft.Tang;
            _aircraft.HLand = _coordinateHelper.GetElevation(lat, lon, _aerodromePoint.NavigationPoint.GeoCoordinate.H);
            _aircraft.GeoCoordinate.Latitude = lat;
            _aircraft.GeoCoordinate.Longitude = lon;
            _aircraft.GeoCoordinate.X = aircraft.GeoCoordinate.X;
            _aircraft.GeoCoordinate.Z = aircraft.GeoCoordinate.Z;
            _aircraft.GeoCoordinate.H = aircraft.GeoCoordinate.H;
            _aircraft.V= aircraft.V;


            DebugParameters.LatLA = lat;
            DebugParameters.LonLA = lon;
            DebugParameters.HLA = _aircraft.HLand;
            DebugParameters.PsiLA = _aircraft.Risk;
            DebugParameters.HbarLA = _aircraft.GeoCoordinate.H- _aircraft.HLand;

            EventsHelper.OnChangeAircraftCoordinateEvent(_aircraft);

            return ConvertHelper.ObjectToByte(_aircraft);

        }
    }
}