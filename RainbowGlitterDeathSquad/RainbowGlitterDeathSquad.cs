using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RobotInterface;
using System.IO;
using System.Reflection;

// Just a rough copy to get working

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
            return "1.0";
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
                    if (sawVictim && victimDistance == 1)
                    {
                        return new RobotKillAction();
                    }
                    else if (atEdge)
                    {
                        return new RobotTurnAction(RobotTurnAction.Direction.LEFT);
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

        public void handleEvent(RobotInterface.RobotEvent re)
        {
            if (re.getRobotEventType() == RobotEvent.Type.LOOK) {
                RobotLookEvent rle = (RobotLookEvent)re;
                if (rle.getContactType().Equals(RobotLookEvent.ContactType.ROBOT)) {
                    sawVictim = true;
                    victimDistance = rle.getDistance();
                    atEdge = false;
                } else if(rle.getContactType().Equals(RobotLookEvent.ContactType.EDGE) &&
                    rle.getDistance() == 1) {
                    atEdge = true;
                } else {
                    sawVictim = false;
                    victimDistance = 999;
                }
            }
        }
    }
}