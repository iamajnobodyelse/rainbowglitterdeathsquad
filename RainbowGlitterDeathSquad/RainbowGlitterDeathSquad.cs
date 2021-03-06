﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RobotInterface;
using System.IO;
using System.Reflection;

// Trying to fix edge and kill bot problems

namespace RainbowGlitterDeathSquad
{
    public class RainbowGlitterDeathSquad : IRobot
    {
        public string getName()
        {
            return "RainbowGlitterDeathSquad";
        }

        public string getVersion()
        {
            return "1.1";
        }

        public string getAuthor()
        {
            return "Jamie K Smith";
        }
        public Bitmap getImage()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("RainbowGlitterDeathSquad.Resources.sailor_moon.png");
            if (s != null)
            {
                Image.FromStream(s);
                Bitmap image = Bitmap.FromStream(s) as Bitmap;
                s.Close();
                return image;
            }
            else { return null; }
        }

        public RainbowGlitterDeathSquad() 
         {
            state = State.INIT;
            sawVictim = false;
            victimDistance = 999;
            atEdge = false;
            nearEdge = false;
         }

        private int RandomNumber(int min, int max)
        {
        Random random = new Random();
        return random.Next(min, max);
        }

        private enum State { INIT = 0, LOOK, LOOKED };
        private State state;

        public RobotInterface.RobotAction takeTurn()
        {
            switch (state)
            {
                case State.INIT:
                    state = State.LOOK;
                    return new RobotScanAction();
                case State.LOOK:
                    state = State.LOOKED;
                    return new RobotLookAction();
                case State.LOOKED:
                    state = State.LOOK;
                    if (sawVictim && victimDistance < 10)
                    {
                        if (sawVictim && victimDistance == 1)
                        {
                            sawVictim = false;
                            return new RobotKillAction();
                        }
                        else
                        {
                            sawVictim = false;
                            return new RobotWalkAction();
                        }
                    }
                    else if (nearEdge)
                    {
                        nearEdge = false;
                        int ToGoOrNotToGo = RandomNumber(1, 4);
                        if (ToGoOrNotToGo == 1)
                        {
                            return new RobotTurnAction(RobotTurnAction.Direction.LEFT);
                        }
                        else if (ToGoOrNotToGo == 2)
                        {
                            return new RobotWalkAction();
                        }
                        else if (ToGoOrNotToGo == 3)
                        {
                            return new RobotTurnAction(RobotTurnAction.Direction.RIGHT);
                        }
                        else
                        {
                            return new RobotWalkAction();
                        }

                    }
                    else if (atEdge)
                    {
                        atEdge = false;
                        int LeftOrRight = RandomNumber(1, 2);
                        if (LeftOrRight == 1)
                        {
                            return new RobotTurnAction(RobotTurnAction.Direction.LEFT);
                        }
                        else
                        {
                            return new RobotTurnAction(RobotTurnAction.Direction.RIGHT);
                        }

                    }
                    else
                    {
                        int ConfuseMove = RandomNumber(1, 5);
                        if (ConfuseMove == 1)
                        {
                            return new RobotWalkAction();
                        }
                        else if (ConfuseMove == 2)
                        {
                            return new RobotTurnAction(RobotTurnAction.Direction.LEFT);
                        }
                        else if (ConfuseMove == 3)
                        {
                            return new RobotWalkAction();
                        }
                        else if (ConfuseMove == 4)
                        {
                            return new RobotTurnAction(RobotTurnAction.Direction.RIGHT);
                        }
                        else
                        {
                            return new RobotWalkAction();
                        }
                    }
                default:
                    return new RobotWalkAction();
            }
        }

        bool sawVictim;
        int victimDistance;
        bool atEdge;
        bool nearEdge;

        public void handleEvent(RobotInterface.RobotEvent re)
        {
            if (re.getRobotEventType() == RobotEvent.Type.LOOK) 
            {
                RobotLookEvent rle = (RobotLookEvent)re;
                if (rle.getContactType().Equals(RobotLookEvent.ContactType.ROBOT)) {
                    sawVictim = true;
                    victimDistance = rle.getDistance();
                    atEdge = false;
                    nearEdge = false;
                }
                else if (rle.getContactType().Equals(RobotLookEvent.ContactType.EDGE) &&
                    rle.getDistance() == 1)
                {
                    atEdge = true;
                }
                else if (rle.getContactType().Equals(RobotLookEvent.ContactType.EDGE) &&
                    rle.getDistance() < 3)
                {
                    nearEdge = true;
                }
                else
                {
                    sawVictim = false;
                    victimDistance = 999;
                }
            }
        }
    }
}