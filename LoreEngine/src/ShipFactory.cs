//----------------------------------------------------------------------
//  ShipFactory.cs
//  by Zephyrus (31/8/2016)
//  Part of LoreEngine 0.1
//  
//  Primary Function: return a randomized list of Ship objects generated from the ships.xml document. 
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LoreEngine
{
    static class ShipFactory
    {
        /// <summary>
        /// Generate a List of Ship structs from "Data/ships.xml".
        /// </summary>
        /// <param name="shipCount">The number of Ships to generate.</param>
        /// <returns>A List containing a number of Ship objects equal to shipCount.</returns>
        public static List<Ship> generateShipList(int shipCount)
        {
            List<Ship> shipList = new List<Ship>(shipCount);
            XmlDocument shipsXML = new XmlDocument();
            shipsXML.Load(Environment.CurrentDirectory + "/data/ships.xml");

            int attempts = 0;
            Random shipPicker = new Random();
            XmlNode candidate;
            Ship newShip;
            Boolean validated = true;

            while (shipList.Count < shipCount && attempts < 5 * shipCount)
            {
                candidate = shipsXML.SelectNodes("ships/ship")[shipPicker.Next(shipsXML.SelectNodes("ships/ship").Count)];
                foreach (Ship ship in shipList)
                {
                    if (ship.Name.Equals(candidate.Attributes["name"].Value))
                        validated = false;
                }

                if (validated)
                {
                    newShip = new Ship();
                    newShip.Arrived = false;
                    newShip.Name = candidate.Attributes["name"].Value;
                    newShip.Description = candidate.SelectSingleNode("description").Attributes["value"].Value;
                    newShip.Heading = new Vector3(Double.Parse(candidate.SelectSingleNode("heading").Attributes["x"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("heading").Attributes["y"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("heading").Attributes["z"].Value));
                    newShip.Position = new Vector3(Double.Parse(candidate.SelectSingleNode("position").Attributes["x"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("position").Attributes["y"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("position").Attributes["z"].Value));
                    newShip.Speed = Double.Parse(candidate.SelectSingleNode("heading").Attributes["speed"].Value);
                    newShip.TargetDist = Double.Parse(candidate.SelectSingleNode("target").Attributes["distance"].Value);
                    newShip.Attenuation = Double.Parse(candidate.SelectSingleNode("attenuationfactor").Attributes["value"].Value);
                    newShip.Messages = candidate.SelectSingleNode("messages");
                    shipList.Add(newShip);
                }
                else
                {
                    validated = true;
                    attempts++;
                }
            }
            return shipList;
        }

        /// <summary>
        /// Returns a list containing all ships within Data/ships.xml
        /// </summary>
        /// <returns>List of all ships in ships.xml</returns>
        public static List<Ship> generateShipList()
        {
            List<Ship> shipList = new List<Ship>();
            XmlDocument shipsXML = new XmlDocument();
            shipsXML.Load(Environment.CurrentDirectory + "/data/ships.xml");
            Ship newShip;
                foreach (XmlNode candidate in shipsXML.SelectNodes("ships/ship"))
                {
                    newShip = new Ship();
                    newShip.Arrived = false;
                    newShip.Name = candidate.Attributes["name"].Value;
                    newShip.Description = candidate.SelectSingleNode("description").Attributes["value"].Value;
                    newShip.Heading = new Vector3(Double.Parse(candidate.SelectSingleNode("heading").Attributes["x"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("heading").Attributes["y"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("heading").Attributes["z"].Value));
                    newShip.Position = new Vector3(Double.Parse(candidate.SelectSingleNode("position").Attributes["x"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("position").Attributes["y"].Value),
                                                  Double.Parse(candidate.SelectSingleNode("position").Attributes["z"].Value));
                    newShip.Speed = Double.Parse(candidate.SelectSingleNode("heading").Attributes["speed"].Value);
                    newShip.TargetDist = Double.Parse(candidate.SelectSingleNode("target").Attributes["distance"].Value);
                    newShip.Attenuation = Double.Parse(candidate.SelectSingleNode("attenuationfactor").Attributes["value"].Value);
                    newShip.Messages = candidate.SelectSingleNode("messages");
                    shipList.Add(newShip);
                }
            return shipList;
        }
    }
}
