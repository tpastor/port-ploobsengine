﻿#region License
/*
    PloobsEngine Game Engine Version 0.3 Beta
    Copyright (C) 2011  Ploobs

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using PloobsEngine.DataStructure;
using PloobsEngine.Utils;

namespace PloobsEngine.IA
{
    /// <summary>
    /// Interface to user handling waypoints
    /// </summary>
    public class WaypointHandler
    {
        static int id = 0;
        WaypointsCollection col;       

        /// <summary>
        ///Restart the handler
        /// </summary>
        public void Clear()
        {
            col = new WaypointsCollection();        
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaypointHandler"/> class.
        /// </summary>
        /// <param name="mapName">Name of the map.</param>
        public WaypointHandler(String mapName = null)
        {
            col = new WaypointsCollection();
            col.MapName = mapName;
        }        

        private int getId()
        {
            return id++;
        }

        /// <summary>
        /// Adds the an unconnected waypoint to the map .
        /// </summary>
        /// <param name="worldPos">The world pos.</param>
        /// <param name="type">The type.</param>
        public void AddDefaultWaypointUnconnected(Vector3 worldPos,WAYPOINTTYPE type)
        {
            Waypoint w = new Waypoint();
            w.Id = getId();
            w.WorldPos = worldPos;
            w.WayType = type;
            w.NeightBorWaypointsId = null;
            col.IdWaypoint.Add(w.Id, w);
            col.State = WaypointsState.UnConnected;
        }

        /// <summary>
        /// Adds the an unconnected waypoint to the map .
        /// </summary>
        /// <param name="waypoint">The waypoint.</param>
        public void AddWaypointUnconnected(Waypoint waypoint)
        {
            if (waypoint == null)
                throw new NullReferenceException("waypoint cannot be null");
            waypoint.Id = getId();
            waypoint.NeightBorWaypointsId = null;
            col.IdWaypoint.Add(waypoint.Id, waypoint);
            col.State = WaypointsState.UnConnected;
        }

        /// <summary>
        /// Removes the waypoint.
        /// </summary>
        /// <param name="id">The id.</param>
        public void RemoveWaypoint(int id)
        {
            col.IdWaypoint.Remove(id);
        }

        /// <summary>
        /// Saves the connected waypoints to a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveConnectedWaypoints(String fileName)
        {
            if (col.State != WaypointsState.Connected)
            {
                throw new Exception("Waypoints are already Connected");
            }
            else if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("MapName cannot be null or empty");
            }
            XmlContentLoader.SaveXmlContent(col, col.GetType(), fileName);
        }

        /// <summary>
        /// Saves the unconnected waypoints to a file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveUnconnectedWaypoints(String fileName)
        {
            if (col.State == WaypointsState.Connected)
            {
                throw new Exception("Waypoints are already Connected");
            }
            else if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("MapName cannot be null or empty");
            }

            XmlContentLoader.SaveXmlContent(col, col.GetType(), fileName);
        }


        /// <summary>
        /// Loads the connected waypoints  from a file
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        public void LoadConnectedWaypoints(String FileName)        
        {
            if (FileName == null)
            {
                throw new Exception("MapName cannot be null");
            }            
            
#if !WINRT
            this.col = XmlContentLoader.LoadXmlContent(FileName, col.GetType()) as WaypointsCollection;
#else
            this.col = XmlContentLoader.LoadXmlContent(FileName, col.GetType()).Result as WaypointsCollection;
#endif

            foreach (Waypoint item in col.GetWaypointsList())
            {             
                if (item.Id > id)
                {
                    id = item.Id + 1;
                }
            }
            col.State = WaypointsState.Connected;

        }

        /// <summary>
        /// Loads the unconnected waypoints.
        /// If it is connected, it will be unconnected
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        public void LoadUnconnectedWaypoints(String FileName)
        {
            if (FileName == null)
            {
                throw new Exception("MapName cannot be null");
            }  
#if !WINRT
            this.col =  XmlContentLoader.LoadXmlContent(FileName,col.GetType()) as WaypointsCollection;
#else 
            this.col  = ( XmlContentLoader.LoadXmlContent(FileName, col.GetType()) ).Result as WaypointsCollection;            
#endif
            if(col.State == WaypointsState.Connected)
            Unconnect();

        }

        /// <summary>
        /// Loads the connected waypoints islands.
        /// </summary>
        /// <param name="waypoints">The waypoints.</param>
        /// <param name="connector">The connector.</param>
        public void LoadConnectedWaypointsIslands(String[] waypoints , IWaypointConnector connector)
        {
            if (waypoints==null || waypoints.Length == 0 )
            {
                throw new Exception("MapName cannot be null");
            }
            else if (connector.ConnectorType != ConnectorType.BETWEEN_COLLECTIONS_CONNECTEC)
            {
                throw new Exception("Wrong Type of Connector");
            }
            int toadd = 0;
            foreach (var item in waypoints)
            {
#if WINRT
                WaypointsCollection w = (XmlContentLoader.LoadXmlContent(item, col.GetType())).Result as WaypointsCollection;
#else
                WaypointsCollection w = XmlContentLoader.LoadXmlContent(item, col.GetType()) as WaypointsCollection;
#endif                
                foreach (Waypoint way in w.IdWaypoint.Values)
                {
                    way.Id = way.Id + toadd;
                    way.NeightBorWaypointsId =  way.NeightBorWaypointsId.Select( (p1 , p2) => p1 + toadd).ToList<int>();                                                            
                }

                IDictionary<int,Waypoint> xx = w.IdWaypoint.ToDictionary(t => t.Key + toadd, u => u.Value);
                w.IdWaypoint = new SerializableDictionary<int, Waypoint>(xx);                   
                toadd+= w.IdWaypoint.Keys.Max();
                this.col.IdWaypoint.Concate(w.IdWaypoint);
            }

            this.col = connector.ConnectWaypoints(col);

        }

        /// <summary>
        /// Unconnects teh waypoints in this handler.
        /// </summary>
        public void Unconnect()
        {
            col.State = WaypointsState.UnConnected;
            id = 0;
            foreach (Waypoint item in col.GetWaypointsList())
            {
                item.NeightBorWaypointsId = null;
                if (item.Id > id)
                {
                    id = item.Id + 1;
                }
            }
        }

        /// <summary>
        /// Connects the waypoints in this handler
        /// </summary>
        /// <param name="connector">The connector.</param>
        public void ConnectWaypoints(IWaypointConnector connector)
        {
            if (connector == null)
                throw new NullReferenceException("connector cannot be null");

            if (connector.ConnectorType != ConnectorType.BETWEEN_WAYPOINTS_UNCONNECTED)
            {
                throw new Exception("Wrong Type of Connector");
            }

            if (col.State == WaypointsState.UnConnected)
            {
                col = connector.ConnectWaypoints(col);
                col.State = WaypointsState.Connected;
            }            
        }

        /// <summary>
        /// Gets the current waypoints collection.
        /// </summary>
        public WaypointsCollection CurrentWaypointsCollection
        {
            get
            {
                return col;
            }
        }
    }

#if WINDOWS
    /// <summary>
    /// Handler Waypoints state
    /// </summary>
    [Serializable]
#endif
    public enum WaypointsState
    {
        Connected,UnConnected,Empty
    }
}
