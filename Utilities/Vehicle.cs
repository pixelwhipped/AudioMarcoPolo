using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.Utilities
{
    public struct Obstacles
    {
        public Vector2 Location;
        public int Size;
        public Obstacles(Vector2 location, int size)
        {
            Location = location;
            Size = size;
        }
    }


    enum SpecialPaths
    {
        AI
    }

    public enum SB
    {
        None,
        Seek,
        Flee,
        Arrive,
        Pursuit,
        Evade,
        Wander,
        PathFollowing,
        Cohesion,
        Alignment,
        Separation,
        CF,
        FCAS,
        FCS,
        CS,
        CA,
        CAS
    }


    //represents a single vehicle
    public class Vehicle
    {
        private static Vehicle[] allCars;

        //=========================================================//
        //general variables
        public static bool enableSpecialPath = false;
        private bool targetChanged;
        private float mass;
        private int max_speed, max_force, vehicleNo;
        private Vector2 currentPosition, velocity, acceleration, heading, steerForce, targetPosition;
        private Rectangle clientSize;
        private SB SB;
        public static Obstacles[] obstacles = new Obstacles[10];    //not used
        //=========================================================//

        //=========================================================//
        //wander specific variables
        private Vector2 wanderTarget;
        public static float WanderRadius = 10, WanderDistance = 100;
        public static int WanderJitter = 1;
        //=========================================================//

        //=========================================================//
        //arrive specific variables
        public static int ArriveRadius = 100;
        //=========================================================//

        //=========================================================//
        //pathfollowing specific varibles
        Vector2[] specialPathPoints, pathPoints, sP1, sP2;
        int currentPathPoint, maxSpecialPathPoints, maxPathPoints;

        //=========================================================//
        //cohesion specific variables
        public static int CohesionRadius = 100;
        //=========================================================//

        //=========================================================//
        //flee specific variables
        public static int FOV = 300;    //field of view
        //=========================================================//

        public static bool weightedSum = true;
        //the constructor
        public Vehicle(Rectangle ClientSize, SB SteeringBehavior, int VehicleNumber, Vector2 target, Vector2 initialVelocity)
        {
            //=========================================================//
            //general initialization
            clientSize = ClientSize;
            mass = 30;
            max_force = 15;
            vehicleNo = VehicleNumber;
            max_speed = 6;
            velocity = initialVelocity;
            
            acceleration = new Vector2(0, 0);
            heading = Vector2.Normalize(velocity);
            currentPosition = new Vector2(BaseGame.Random.Next(clientSize.Width), BaseGame.Random.Next(clientSize.Height));
            steerForce = new Vector2(0);
            targetPosition = target;
            targetChanged = false;
            SB = SteeringBehavior;
            //=========================================================//

            //=========================================================//
            //wander specific variables
            wanderTarget = new Vector2(0);
            //=========================================================//

            //=========================================================//

            //=========================================================//
            //pathfollowing s
            maxSpecialPathPoints = 40;
            maxPathPoints = 10;
            currentPathPoint = 0;
            specialPathPoints = new Vector2[maxSpecialPathPoints];
            pathPoints = new Vector2[maxPathPoints];
            sP1 = new Vector2[22];
            sP2 = new Vector2[18];
            NewPath();

        }

        //=========================================================//




        #region Properties
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
        }

        public Vector2 CurrentPosition
        {
            get
            {
                return currentPosition;
            }
            set
            {
                currentPosition = value;
            }
        }

        public SB SteeringBehaviour
        {
            get { return SB; }
            set { SB = value; }
        }

        public bool TargetChanged
        {
            get { return targetChanged; }
            set { targetChanged = value; }
        }

        public int MaxPathPoints
        {
            get { return maxSpecialPathPoints; }
            set { maxSpecialPathPoints = value; }
        }

        public Vector2 TargetPosition
        {
            get { return targetPosition; }
            set { targetPosition = value; }
        }

        public int MaxForce
        {
            get { return max_force; }
            set { max_force = value; }
        }

        public int MaxSpeed
        {
            get { return max_speed; }
            set { max_speed = value; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        #endregion

        //=========================================================//


        //calculate force and update position
        private void UpdatePosition(ref Vector2 targetPosition)
        {
            heading = Vector2.Normalize(velocity);
            switch (SB)
            {
                case SB.None:
                    steerForce = Vector2.Zero;
                    break;
                case SB.Seek:
                    steerForce = SteeringBehaviours.Seek(ref targetPosition, ref currentPosition, ref velocity, max_speed);
                    break;
                case SB.Flee:
                    steerForce = SteeringBehaviours.Flee(ref targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo);
                    break;
                case SB.Arrive:
                    steerForce = SteeringBehaviours.Arrive(ref targetPosition, ref currentPosition, ref velocity, ArriveRadius, max_speed, vehicleNo);
                    break;
                case SB.Pursuit:
                    break;
                case SB.Evade:
                    break;
                case SB.Wander:
                    steerForce = SteeringBehaviours.Wander(ref wanderTarget, ref currentPosition, ref velocity, ref heading, WanderRadius, WanderDistance, WanderJitter);
                    break;
                case SB.PathFollowing:
                    if (!enableSpecialPath)
                    {
                        steerForce = SteeringBehaviours.PathFollowing(ref targetPosition, ref currentPosition, ref velocity, ref pathPoints, ref currentPathPoint, maxPathPoints, max_speed);
                    }
                    else
                        steerForce = SteeringBehaviours.PathFollowing(ref targetPosition, ref currentPosition, ref velocity, ref specialPathPoints, ref currentPathPoint, maxSpecialPathPoints, max_speed);
                    break;
                case SB.Cohesion:
                    steerForce = SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius);
                    break;
                case SB.Alignment:
                    steerForce = SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed);
                    break;
                case SB.Separation:
                    steerForce = SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed);
                    break;
                case SB.CF:
                    steerForce = Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Flee(ref targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo));
                    break;
                case SB.CA:
                    if (weightedSum)
                    {
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), .1f);
                    }
                    else
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed));
                    }
                    break;
                case SB.CAS:
                    if (weightedSum)
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed)));
                    }
                    else
                    {
                        steerForce = Vector2.Multiply(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), .3f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), .5f);
                    }

                    break;
                case SB.CS:
                    if (weightedSum)
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed));
                    }
                    else
                    {
                        steerForce = Vector2.Multiply(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), .8f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                    }
                    break;
                case SB.FCAS:
                    if (weightedSum)
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Flee(ref targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo))));
                    }
                    else
                    {
                        steerForce = Vector2.Multiply(SteeringBehaviours.Flee(ref targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo), .4f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), .3f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), .5f);
                    }
                    break;
                case SB.FCS:
                    steerForce = Vector2.Add(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Flee(ref targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo)));
                    break;
                default:
                    break;
            }
            steerForce = VectorHelpers.Truncate(steerForce, max_force);
            acceleration = steerForce / mass;
            velocity = VectorHelpers.Truncate(velocity + acceleration, max_speed);
            currentPosition = Vector2.Add(velocity, currentPosition);

            var mirrored = true;
            if (!mirrored)
            {
                if (currentPosition.X > clientSize.Width)
                    currentPosition.X = 0;
                if (currentPosition.Y > clientSize.Height)
                    currentPosition.Y = 0;
                if (currentPosition.X < 0)
                    currentPosition.X = clientSize.Width;
                if (currentPosition.Y < 0)
                    currentPosition.Y = clientSize.Height;
            }
            else
            {
                if (currentPosition.X > clientSize.Width)
                    velocity.X *= -1;
                if (currentPosition.Y > clientSize.Height)
                    velocity.Y *= -1;
                if (currentPosition.X < 0)
                    velocity.X *= -1; ;
                if (currentPosition.Y < 0)
                    velocity.Y *= -1; ;
            }
            currentPosition = new Vector2(MathHelper.Clamp(currentPosition.X, 0, clientSize.Width),
                MathHelper.Clamp(currentPosition.Y, 0, clientSize.Height));
            targetChanged = false;
        }

        //=========================================================//

        public void NewPath()
        {
            for (int i = 0, j = 0, k = 0; i < 10; i++, j += 10, k += 30)
            {
                specialPathPoints[i] = new Vector2(100 + j, 400 - k);
            }
            for (int i = 10, j = 0, k = 0; i < 20; i++, j += 10, k += 30)
            {
                specialPathPoints[i] = new Vector2(specialPathPoints[9].X + j, specialPathPoints[9].Y + k);
            }
            specialPathPoints[20] = new Vector2(specialPathPoints[16].X, specialPathPoints[16].Y);
            specialPathPoints[21] = new Vector2(specialPathPoints[3].X, specialPathPoints[3].Y);
            for (int i = 22, j = 0, k = 0; i < 26; i++, j += 40, k += 30)
            {
                specialPathPoints[i] = new Vector2(450 + j, specialPathPoints[9].Y);
            }
            for (int i = 26, j = 0, k = 0; i < 36; i++, j += 10, k += 30)
            {
                specialPathPoints[i] = new Vector2((specialPathPoints[25].X + specialPathPoints[22].X) / 2, specialPathPoints[9].Y + k);
            }
            for (int i = 36, j = 0, k = 0; i < maxSpecialPathPoints; i++, j += 40, k += 30)
            {
                specialPathPoints[i] = new Vector2(450 + j, specialPathPoints[0].Y);
            }

            for (int i = 0; i < 22; i++)
            {
                sP1[i] = specialPathPoints[i];
            }
            for (int i = 0, j = 22; i < 18; i++, j++)
            {
                sP2[i] = specialPathPoints[j];
            }
            for (int i = 0; i < maxPathPoints; i++)
            {
                pathPoints[i] = new Vector2(BaseGame.Random.Next(100, clientSize.Width - 100), BaseGame.Random.Next(100, clientSize.Height - 100));
            }
        }

        public void Step(ref Vector2 target)
        {
            UpdatePosition(ref target);            
        }

        public static void SetCarsData(ref Vehicle[] cars)
        {
            allCars = cars;
        }

        public void CreateObstacles()
        {
            for (int i = 0; i < Vehicle.obstacles.Length; i++)
            {
                obstacles[i].Location = new Vector2((float)BaseGame.Random.Next(clientSize.Width - 100), (float)BaseGame.Random.Next(clientSize.Height - 100));
                obstacles[i].Size = BaseGame.Random.Next(20, 100);
            }
        }
    }
}
