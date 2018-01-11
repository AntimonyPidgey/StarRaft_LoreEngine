//----------------------------------------------------------------------
//  Ship.cs
//  by Zephyrus (31/8/2016)
//  Part of LoreEngine 0.1
//  
//  Container class to hold values associated with each non-player ship.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LoreEngine
{
    class Ship
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Vector3 Heading { get; set; }
        public Vector3 Position { get; set; }
        public Double TargetDist { get; set; }
        public double Speed { get; set; }
        public double Attenuation { get; set; }
        public XmlNode Messages { get; set; }
        public Boolean Arrived { get; set; }

        /// <param name="pShipPosition">A vector containing the position of the player ship relative to Earth (0,0,0)</param>
        /// <returns>The distance in lightyears between the player ship position and this ship's position.</returns>
        public double getDistance(Vector3 pShipPosition)
        {
            return Position.posDistance(pShipPosition);
        }
    }
}
