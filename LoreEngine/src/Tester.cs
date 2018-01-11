//----------------------------------------------------------------------
//  Tester.cs
//  by Zephyrus (31/8/2016)
//  Part of LoreEngine 0.1
//  
//  Testing class containing main(). Simple but complete implementation.
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LoreEngine
{
    class Tester
    {
        
        public static void Main(string[] args)
        {
            testFull();
        }

        // Test for XML reading and categorization by ShipFactory.
        private static void testXML()
        {
            List<Ship> shipList = ShipFactory.generateShipList();
            foreach (Ship ship in shipList)
            {
                Console.WriteLine();
                Console.WriteLine("-------------");
                Console.WriteLine(ship.Name);
                Console.WriteLine("-------------");
                Console.WriteLine("Description: " + ship.Description);
                Console.WriteLine("Speed: " + ship.Speed + "c");
                Console.WriteLine("Heading: " + ship.Heading.X + ", " + ship.Heading.Y + ", " + ship.Heading.Z);
                Console.WriteLine("Attenuation: " + ship.Attenuation*100 + "% per LY");
                Console.WriteLine();
                Console.WriteLine("----");
                Console.WriteLine("Messages:");
                foreach (XmlNode msg in ship.Messages.SelectNodes("message"))
                {
                    Console.WriteLine();
                    Console.WriteLine("  " + msg.Attributes["id"].Value + ": " + msg.Attributes["text"].Value);
                    Console.WriteLine("  " + "Delay: " + msg.Attributes["delay"].Value);
                    Console.WriteLine("  " + "Replies:");
                    foreach (XmlNode reply in msg.SelectNodes("reply"))
                    {
                        Console.WriteLine("    Ref: " + reply.Attributes["ref"].Value);
                        Console.WriteLine("    " + reply.Attributes["text"].Value);
                    }
                    Console.WriteLine("  " + "Events:");
                    foreach (XmlNode evt in msg.SelectNodes("event"))
                    {
                        Console.WriteLine("    " + evt.Attributes["id"].Value);
                        Console.WriteLine("    Value: " + evt.Attributes["value"].Value);
                    }
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }

        // Test all the features of the system with a basic text interface.
        private static void testFull()
        {
            // Generate a ship map randomly selecting from available ships in ships.xml.
            Console.WriteLine("number of ships to select from ships.xml:");
            ShipMap shipMap = new ShipMap(Int32.Parse(Console.ReadLine()), new Vector3(0,0,0));
            Message newMessage;

            // List the selected ships for easier ships.xml cross-referencing.
            Console.WriteLine("\nSelected Ships:");
            foreach (Ship ship in shipMap.shipList)
            {
                Console.WriteLine(ship.Name);
            }
            Console.WriteLine();

            // Simulate the movements of each ship at a 0.25 year timescale loop.
            Boolean exitSim = false;
            int itercount = 0;
            while (!exitSim)
            {
                itercount++;
                if ((newMessage=shipMap.advanceTime(0.25, new Vector3(shipMap.PlayerShipPosition.X, shipMap.PlayerShipPosition.Y, shipMap.PlayerShipPosition.Z + 0.01))) != null)
                {
                    Console.WriteLine("----------");
                    Console.WriteLine(newMessage.sendingShip.Name + ", Year " + shipMap.timePassed + ": " + newMessage.Text);
                    Console.WriteLine("\nEvents:");
                    foreach (string[] evt in newMessage.Events){
                        Console.WriteLine("   " + evt[0] + " value:" + evt[1]);
                    }
                    Console.WriteLine("\nReply (x to exit, Enter to continue, any other string to crash): ");
                    for (int i = 0; i < newMessage.Replies.Count; i++ )
                    {
                        Console.WriteLine("   " + i + ") " + newMessage.Replies[i][1] + ": " + newMessage.Replies[i][0]);
                    }
                    string reply = Console.ReadLine();
                    if (reply.Equals("x"))
                    {
                        exitSim = true;
                    }
                    else if (reply.Equals(""))
                    {

                    }
                    else
                    {
                        shipMap.replyMessage(newMessage, newMessage.Replies[Int32.Parse(reply)][1]);
                    }
                    newMessage = null;
                }
                if (itercount >= 1000)
                {
                    itercount = 0;
                    Console.ReadLine();
                }
            }

        }
    }
}
