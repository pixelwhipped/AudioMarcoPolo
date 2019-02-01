﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.Utilities
{
    public static class SteeringBehaviours
    {
        //The seek steering behavior returns a force that directs an agent toward a target position
        public static Vector2 Seek(ref Vector2 targetPosition, ref Vector2 currentPosition, ref Vector2 Velocity, int max_speed)
        {
            Vector2 desired_V = Vector2.Normalize(Vector2.Subtract(targetPosition, currentPosition)) * max_speed;
            return Vector2.Subtract(desired_V, Velocity);
        }

        //Flee is the opposite of seek. Instead of producing a steering force to steer the agent toward a target position, flee creates a force that steers the agent away. 
        public static Vector2 Flee(ref Vector2 targetPosition, ref Vector2 currentPosition, ref Vector2 Velocity, int max_speed, int FOV, int vehicleNo)
        {

            if (VectorHelpers.Length(Vector2.Subtract(targetPosition, currentPosition)) > FOV)
            {
                return Vector2.Zero;
            }
            Vector2 desired_V = Vector2.Normalize(Vector2.Subtract(currentPosition, targetPosition)) * max_speed;
            return Vector2.Subtract(desired_V, Velocity);            
        }

        //Seek is useful for getting an agent moving in the right direction, but often you'll want your agents
        //to come to a gentle halt at the target position, and as you've seen, seek is not too great at stopping gracefully.
        //Arrive is a behavior that steers the agent in such a way it decelerates onto the target position.
        public static Vector2 Arrive(ref Vector2 targetPosition, ref Vector2 currentPosition, ref Vector2 Velocity, int arriveRadius, int max_speed, int vehicleNo)
        {
            Vector2 toTarget = Vector2.Subtract(targetPosition, currentPosition);
            double distance = toTarget.Length();
            if (distance > 0)
            {
                double speed = max_speed * (distance / arriveRadius);
                speed = Math.Min(speed, max_speed);
                Vector2 desired_V = toTarget * (float)(speed / distance);
                return Vector2.Subtract(desired_V, Velocity);
            }
            return new Vector2(0, 0);
        }

        //You'll often find wander a useful ingredient when creating an agent's behavior.
        //It's designed to produce a steering force that will give the impression of a random walk through the agent's environment.
        public static Vector2 Wander(ref Vector2 wanderTarget, ref Vector2 currentPosition, ref Vector2 Velocity, ref Vector2 heading, float wanderRadius, float wanderDistance, int wanderJitter)
        {
            heading = Vector2.Normalize(Velocity);
            wanderTarget += new Vector2(BaseGame.Random.Next(-wanderJitter, wanderJitter), BaseGame.Random.Next(-wanderJitter, wanderJitter));
            wanderTarget = Vector2.Normalize(wanderTarget);
            wanderTarget *= wanderRadius / 2;          
            Vector2 circleCenterM = new Vector2((heading.X * wanderDistance) + currentPosition.X, (heading.Y * wanderDistance) + currentPosition.Y);
            Vector2 pointOnCircle = new Vector2(circleCenterM.X + wanderTarget.X, circleCenterM.Y + wanderTarget.Y);
            return Vector2.Subtract(pointOnCircle, currentPosition);
        }

        //Path following creates a steering force that moves a vehicle along a series of waypoints forming a path.
        //Sometimes paths have a start and end point, and other times they loop back around on themselves forming a never-ending, closed path. 
        public static Vector2 PathFollowing(ref Vector2 targetPosition, ref Vector2 currentPosition, ref Vector2 Velocity, ref Vector2[] pathPoints, ref int currentPathPoint, int maxPathPoints, int max_speed)
        {
            if (currentPathPoint > maxPathPoints - 1)
            {
                currentPathPoint = 0;
            }
            int nextPathPoint = currentPathPoint + 1;
            if ((nextPathPoint > (maxPathPoints - 1)))
            {
                nextPathPoint = 0;
            }
            Vector2 currentPath = new Vector2(pathPoints[currentPathPoint].X, pathPoints[currentPathPoint].Y);
            Vector2 differenceV = Vector2.Subtract(currentPosition, currentPath);
            if (Math.Abs(differenceV.Length()) < 5)
            {
                targetPosition = new Vector2(pathPoints[nextPathPoint].X, pathPoints[nextPathPoint].Y);
                ++currentPathPoint;
                return Seek(ref targetPosition, ref currentPosition, ref Velocity, max_speed);
            }
            else
            {
                targetPosition = new Vector2(pathPoints[currentPathPoint].X, pathPoints[currentPathPoint].Y);
                //currentPathPoint++;
                return Seek(ref targetPosition, ref currentPosition, ref Velocity, max_speed);
            }
        }
        //Cohesion produces a steering force that moves a vehicle toward the center of mass of its neighbors
        //A sheep running after its flock is demonstrating cohesive behavior. Use this force to keep a group of vehicles together.
        public static Vector2 Cohesion(ref Vehicle[] allCars, Vehicle me, Vector2 currentPosition, Vector2 velocity, int max_speed, int cohesionRadius)
        {
            int j = 0;
            Vector2 averagePosition = Vector2.Zero;
            Vector2 distance = Vector2.Zero;
            for (int i = 0; i < allCars.Length; i++)
            {
                distance = Vector2.Subtract(currentPosition, allCars[i].CurrentPosition);
                if (VectorHelpers.Length(distance) < cohesionRadius && allCars[i] != me)
                {
                    j++;
                    averagePosition = Vector2.Add(averagePosition, allCars[i].CurrentPosition);
                }
            }
            if (j == 0)
            {
                return Vector2.Zero;
            }
            else
            {
                averagePosition = averagePosition / j;
                return Seek(ref averagePosition, ref currentPosition, ref velocity, max_speed);
            }
        }

        //Alignment attempts to keep a vehicle's heading aligned with its neighbors
        //The force is calculated by first iterating through all the neighbors and averaging their heading vectors. 
        //This value is the desired heading, so we just subtract the vehicle's heading to get the steering force.
        public static Vector2 Alignment(ref Vehicle[] allCars, Vehicle me, ref Vector2 currentPosition, ref Vector2 velocity, int max_speed)
        {
            int j = 0;
            Vector2 averageDirection = new Vector2(0);
            Vector2 distance = new Vector2(0);
            for (int i = 0; i < allCars.Length; i++)
            {
                distance = Vector2.Subtract(currentPosition, allCars[i].CurrentPosition);
                if (VectorHelpers.Length(distance) < 100 && allCars[i] != me)
                {
                    j++;
                    averageDirection = Vector2.Add(averageDirection, allCars[i].Velocity);
                }
            }
            if (j == 0)
            {
                return Vector2.Zero;
            }
            else
            {
                averageDirection = averageDirection / j;
                return Vector2.Subtract(averageDirection, velocity);
            }
        }

        //Separation creates a force that steers a vehicle away from those in its neighborhood region. 
        //When applied to a number of vehicles, they will spread out, trying to maximize their distance from every other vehicle
        public static Vector2 Separation(ref Vehicle[] allCars, Vehicle me, ref Vector2 currentPosition, ref Vector2 velocity, int max_speed)
        {
            int j = 0;
            Vector2 separationForce = new Vector2(0);
            Vector2 averageDirection = new Vector2(0);
            Vector2 distance = new Vector2(0);
            for (int i = 0; i < allCars.Length; i++)
            {
                distance = Vector2.Subtract(currentPosition, allCars[i].CurrentPosition);
                if (VectorHelpers.Length(distance) < 100 && allCars[i] != me)
                {
                    j++;
                    separationForce += Vector2.Subtract(currentPosition, allCars[i].CurrentPosition);
                    separationForce = Vector2.Normalize(separationForce);
                    separationForce = Vector2.Multiply(separationForce, 1 / .7f);
                    averageDirection = Vector2.Add(averageDirection, separationForce);
                }
            }
            if (j == 0)
            {
                return Vector2.Zero;
            }
            else
            {
                //averageDirection = averageDirection / j;
                return averageDirection;
            }
        }
    }
   
}
