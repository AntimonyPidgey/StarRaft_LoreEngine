//----------------------------------------------------------------------
//  ShipMap.cs
//  by Zephyrus (31/8/2016)
//  Part of LoreEngine 0.1
//  
//  Holder class for a generated list of ship objects.
//
//  Primary function: Determine which messages the player shall receive and in what order.
//
//  Ships are represented by a heading vector and a position vector.
//  To modify the position vector, the heading vector is simply multiplied 
//  by the time passed (in years) and added to the position vector.
//  AdvanceTime() is used for this. Be sure to call it every time you advance game time.
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LoreEngine
{
    class ShipMap
    {
        public List<Ship> shipList { get; set; }
        public List<Message> activeMessages { get; set; }
        public List<Message> deliveredMessages { get; set; }
        public Double timePassed { get; private set; }
        public Vector3 PlayerShipPosition { get; set; }

        public ShipMap(int shipCount, Vector3 PlayerShipStartingPosition)
        {
            activeMessages = new List<Message>();
            deliveredMessages = new List<Message>();
            shipList = ShipFactory.generateShipList(shipCount);
            timePassed = 0;
            PlayerShipPosition = PlayerShipStartingPosition;
            getRootMessages();
        }

        /// <summary>
        /// Advance the position of each ship as if it has been travelling 
        /// for the given number of years (decimals are acceptable).
        /// 
        /// If this added distance causes a ship to arrive at its destination, 
        /// stop the ship and send out messages marked "arrival" (if any).
        /// 
        /// If any messages in the active message array have a receive time less 
        /// than or equal to timepassed, deliver one such message per calling of 
        /// the function and shift it to the deliveredmessages List.
        /// 
        /// </summary>
        /// <param name="years">Number of years to advance the travel distance.</param>
        /// <param name="PlayerShipPosition">The updated position of the player ship.</param>
        /// <returns>The first message in the active message array that has a ReceiveTime value less than timePassed (empty if none).</returns>
        public Message advanceTime(double years, Vector3 PlayerShipPosition)
        {
            this.PlayerShipPosition = PlayerShipPosition;
            for (int i = 0; i < shipList.Count; i++ )
            {
                shipList[i].Position = shipList[i].Position.Add(shipList[i].Heading * shipList[i].Speed * years);
                if (!shipList[i].Arrived) {
                    if (shipList[i].Position.getMagnitude() >= shipList[i].TargetDist)
                    {
                        shipList[i].Arrived = true;
                        shipList[i].Position = shipList[i].Heading * shipList[i].TargetDist;
                        shipList[i].Speed = 0;
                        Message mess;
                        List<string[]> replies = new List<string[]>();
                        List<string[]> events = new List<string[]>();
                        foreach (XmlNode message in shipList[i].Messages.SelectNodes("message[@id=\"arrival\"]"))
                        {
                            mess = new Message();
                            mess.Replies = new List<string[]>();
                            mess.Events = new List<string[]>();
                            mess.sendingShip = shipList[i];
                            mess.sourceMessage = null;
                            mess.ID = message.Attributes["id"].Value;
                            mess.Text = message.Attributes["text"].Value;
                            mess.ReceiveTime = timePassed + shipList[i].getDistance(PlayerShipPosition) + Double.Parse(message.Attributes["delay"].Value);
                            Console.WriteLine("Message set for year " + mess.ReceiveTime + " at distance " + shipList[i].getDistance(PlayerShipPosition) + "LY");
                            foreach (XmlNode reply in message.SelectNodes("reply"))
                            {
                                mess.Replies.Add(new string[] { reply.Attributes["text"].Value, reply.Attributes["ref"].Value });
                            }
                            foreach (XmlNode evt in message.SelectNodes("event"))
                            {
                                mess.Events.Add(new string[] { evt.Attributes["id"].Value, evt.Attributes["value"].Value });
                            }

                            activeMessages.Add(mess);
                        }
                    }
                }
            }
            timePassed += years;
            for (int i = 0; i < activeMessages.Count; i++)
            {
                if (activeMessages[i].ReceiveTime <= timePassed)
                {
                    Message temp = activeMessages[i];
                    deliveredMessages.Add(temp);
                    activeMessages.Remove(temp);
                    return temp;
                }
            }
            //Console.WriteLine("Year: " + timePassed + "\nActive Messages: " + activeMessages.Count);
            return null;
        }

        /// <summary>
        /// Iterates through all the available Ship objects to queue up all root messages and their delay times.
        /// Called at the beginning of a new ShipMap.
        /// </summary>
        private void getRootMessages()
        {
            List<Message> rootMessages = new List<Message>();
            Message mess;
            List<string[]> replies = new List<string[]>();
            List<string[]> events = new List<string[]>();
            foreach (Ship ship in shipList)
            {
                foreach (XmlNode message in ship.Messages.SelectNodes("message[@id='root']"))
                {
                    mess = new Message();
                    mess.Replies = new List<string[]>();
                    mess.Events = new List<string[]>();
                    mess.sendingShip = ship;
                    mess.sourceMessage = null;
                    mess.ID = message.Attributes["id"].Value;
                    mess.Text = message.Attributes["text"].Value; 
                    mess.ReceiveTime = timePassed + ship.getDistance(PlayerShipPosition) * 2 + Double.Parse(message.Attributes["delay"].Value);
                    Console.WriteLine("Message set for year " + mess.ReceiveTime + " at distance " + ship.getDistance(PlayerShipPosition) + "LY");
                    mess.baseQuality = ship.getDistance(PlayerShipPosition) * mess.sendingShip.Attenuation;
                    foreach (XmlNode reply in message.SelectNodes("reply"))
                    {
                        mess.Replies.Add(new string[] { reply.Attributes["text"].Value, reply.Attributes["ref"].Value });
                    }
                    foreach (XmlNode evt in message.SelectNodes("event"))
                    {
                        mess.Events.Add(new string[] { evt.Attributes["id"].Value, evt.Attributes["value"].Value });
                    }

                    activeMessages.Add(mess);
                }
            }
        }

        /// <summary>
        /// Takes a message struct and queues a new Message from the sending ship based on the reply ID selected.
        /// </summary>
        /// <param name="sourceMessage">The message being replied to.</param>
        /// <param name="replyRef">The reference ID contained in the selected reply node.</param>
        public void replyMessage(Message sourceMessage, string replyRef){
            Message mess;
            List<string[]> replies = new List<string[]>();
            List<string[]> events = new List<string[]>();
                foreach (XmlNode message in sourceMessage.sendingShip.Messages.SelectNodes("message[@id=\"" + replyRef + "\"]"))
                {
                    mess = new Message();
                    mess.Replies = new List<string[]>();
                    mess.Events = new List<string[]>();
                    mess.sendingShip = sourceMessage.sendingShip;
                    mess.sourceMessage = sourceMessage;
                    mess.ID = message.Attributes["id"].Value;
                    mess.Text = message.Attributes["text"].Value;
                    mess.ReceiveTime = timePassed + sourceMessage.sendingShip.getDistance(PlayerShipPosition) * 2 + Double.Parse(message.Attributes["delay"].Value);
                    Console.WriteLine("Message set for year " + mess.ReceiveTime + " at distance " + sourceMessage.sendingShip.getDistance(PlayerShipPosition) + "LY");
                    foreach (XmlNode reply in message.SelectNodes("reply"))
                    {
                        mess.Replies.Add(new string[] { reply.Attributes["text"].Value, reply.Attributes["ref"].Value });
                    }
                    foreach (XmlNode evt in message.SelectNodes("event"))
                    {
                        mess.Events.Add(new string[] { evt.Attributes["id"].Value, evt.Attributes["value"].Value });
                    }

                    activeMessages.Add(mess);
                }
        }
    }
}
