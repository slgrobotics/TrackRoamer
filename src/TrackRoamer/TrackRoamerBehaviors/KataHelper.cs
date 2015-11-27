using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    /// <summary>
    /// minimum set of parameters for turns and back or forward movements
    /// </summary>
    public class TurnAndMoveParameters
    {
        public int rotateAngle = 0;         // degrees, + right   - left
        public double rotatePower = 0.0d;   // 0 to 1.0d
        public int speed = 0;               // 0 to 1000 - mm/sec
        public int distance = 0;            // mm
        public bool success = true;         // will be set to false if action was canceled by Drive
        public MovingState desiredMovingState = MovingState.Unknown;    // on success, will set the state to this value.
    }

    /// <summary>
    /// a set of parameters describing one kata step (turn and back or forward move)
    /// can be decerialized from XML Element
    /// </summary>
    public class KataStep : TurnAndMoveParameters
    {
        public string name;
        public int neededForwardDistance { get { return distance > 0 ? distance : 0; } }
        public int neededBackupDistance { get { return distance < 0 ? -distance : 0; } }

        public KataStep()
        {
        }

        public KataStep(string nm, XDocument xdoc)
        {
            name = nm;

        }

        public KataStep(XElement xe)
        {
            name = xe.Attribute("name").Value;

            rotateAngle = xe.Descendants("rotate").Count() == 0 ? 0 : int.Parse(xe.Descendants("rotate").First().Value);
            distance = xe.Descendants("translate").Count() == 0 ? 0 : int.Parse(xe.Descendants("translate").First().Value);

            if (Math.Abs(rotateAngle) > 0)
            {
                rotatePower = TrackRoamerBehaviorsService.UnscaledModerateTurnPower;
            }

            if (Math.Abs(distance) > 0)
            {
                speed = (int)Math.Round(TrackRoamerBehaviorsService.UnscaledModerateForwardVelocityMmSec);
            }
        }

        public bool CanPerform(CollisionState collisionState)
        {
            return true;
        }
    }

    /// <summary>
    /// a collection of Kata Steps comprising full Kata dance of any complexity.
    /// </summary>
    public class Kata : List<KataStep>
    {
        public string name;
        public bool success;
        public int successfulStepsCount;

        public Kata()
        {
        }

        public Kata(string nm, IEnumerable<KataStep> steps)
        {
            name = nm;

            this.AddRange(steps);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("  kata: " + this.name);

            foreach (KataStep ks in this)
            {
                sb.AppendLine("    step: " + ks.name);

                if (ks.rotateAngle != 0)
                {
                    sb.AppendLine("        rotateAngle: " + ks.rotateAngle);
                }

                if (ks.distance != 0)
                {
                    sb.AppendLine("           distance: " + ks.distance);
                }
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// a helper class to read in initial collection of all possible Katas and retrieve ones that are needed
    /// based on the available space and requestor's desires.
    /// </summary>
    public class KataHelper
    {
        private static List<Kata> katas = new List<Kata>();

        public static IEnumerable<Kata> AllKatas { get { return katas; } }

        /// <summary>
        /// get a subset of Kata's with names matching regular expression
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static IEnumerable<Kata> KataByName(string regex)
        {
           return from k in katas
                  where Regex.Match(k.name, regex).Success
                  select k;
        }

        /// <summary>
        /// get a subset of Kata's that can be executed given a CollisionState, optionally further filtered by names matching regular expression
        /// </summary>
        /// <param name="collisionState"></param>
        /// <param name="kataGroupName"></param>
        /// <returns></returns>
        public static IEnumerable<Kata> KataByCollisionState(CollisionState collisionState, string kataGroupName)
        {
            List<Kata> ret = new List<Kata>();

            if (collisionState == null)
            {
                return ret;
            }

            if (collisionState.canMoveBackwards && collisionState.canMoveBackwardsDistanceMm > 500)
            {
                // have some room in the back, try some diagonal moves:
                if (collisionState.canTurnRight)
                {
                    ret.AddRange(from k in katas
                                 where Regex.Match(k.name, kataGroupName + " .*LeftBack.*").Success
                                 select k);
                }

                if (collisionState.canTurnLeft)
                {
                    ret.AddRange(from k in katas
                                 where Regex.Match(k.name, kataGroupName + " .*RightBack.*").Success
                                 select k);
                }
            }
            else
            {
                // can't move back; try the sides:
                if (collisionState.canTurnRight)
                {
                    ret.AddRange(from k in katas
                                 where Regex.Match(k.name, kataGroupName + " .*LeftSide.*").Success
                                 select k);
                }

                if (collisionState.canTurnLeft)
                {
                    ret.AddRange(from k in katas
                                 where Regex.Match(k.name, kataGroupName + " .*RightSide.*").Success
                                 select k);
                }
            }

            return ret;
        }

        /// <summary>
        /// static constructor will be called when any metod of the class is first called
        /// </summary>
        static KataHelper()
        {
            Console.WriteLine("IP: KataHelper() started");

            // read all the Kata's in memory:
            XDocument xKatas = XDocument.Parse(katasXml);

            var query = from c in xKatas.Descendants("kata")
                        select new Kata(c.Attribute("name").Value, from ks in c.Descendants("step") select new KataStep(ks));

            katas.AddRange(query);

            foreach (Kata k in katas)
            {
                Console.WriteLine("" + k);
            }

            Console.WriteLine("OK: KataHelper() finished");
        }

        private static string katasXml = @"
<katas>

  <kata name='avoid to Back'>
    <step name='Back600'>
        <translate>-600</translate>
    </step>
  </kata>

  <kata name='avoid to RightBack'>
    <step name='Back100'>
        <translate>-100</translate>
    </step>
    <step name='Left30'>
        <rotate>-30</rotate>
    </step>
    <step name='Back500'>
        <translate>-500</translate>
    </step>
    <step name='Right30'>
        <rotate>30</rotate>
    </step>
  </kata>

  <kata name='avoid to LeftBack'>
    <step name='Back100'>
        <translate>-100</translate>
    </step>
    <step name='Right30'>
        <rotate>30</rotate>
    </step>
    <step name='Back500'>
        <translate>-500</translate>
    </step>
    <step name='Left30'>
        <rotate>-30</rotate>
    </step>
  </kata>

  <kata name='avoid to LeftSide'>
    <step name='Back100'>
        <translate>-100</translate>
    </step>
    <step name='Right90'>
        <rotate>90</rotate>
    </step>
    <step name='Back500'>
        <translate>-500</translate>
    </step>
    <step name='Left90'>
        <rotate>-90</rotate>
    </step>
  </kata>

  <kata name='avoid to RightSide'>
    <step name='Back100'>
        <translate>-100</translate>
    </step>
    <step name='Left90'>
        <rotate>-90</rotate>
    </step>
    <step name='Back500'>
        <translate>-500</translate>
    </step>
    <step name='Right90'>
        <rotate>90</rotate>
    </step>
  </kata>

</katas>
        ";
    }
}
