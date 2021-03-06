#region License
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PloobsEngine.Utils;

namespace Bnoerj.AI.Steering
{
	public class SimpleVehicle : SteerLibrary
	{
		// give each vehicle a unique number
		public readonly int SerialNumber;
		static int serialNumberCounter = 0;

		// Mass (defaults to unity so acceleration=force)
		float mass;

		// size of bounding sphere, for obstacle avoidance, etc.
		float radius;

		// speed along Forward direction. Because local space is
		// velocity-aligned, velocity = Forward * Speed
		float speed;

		// the maximum steering force this vehicle can apply
		// (steering force is clipped to this magnitude)
		float maxForce;

		// the maximum speed this vehicle is allowed to move
		// (velocity is clipped to this magnitude)
		float maxSpeed;

		float curvature;
        Vector3 lastForward;
        Vector3 lastPosition;
        Vector3 smoothedPosition;
		float smoothedCurvature;
		// The acceleration is smoothed
        Vector3 acceleration;

		// constructor
		public SimpleVehicle(bool reset = true)
            : base(reset)
		{
			// set inital state
            if(reset)
			    Reset();

			// maintain unique serial numbers
			SerialNumber = serialNumberCounter++;
		}

		// reset vehicle state
		public override void Reset()
		{
			// reset LocalSpace state
			ResetLocalSpace();

			// reset SteerLibraryMixin state
			//FIXME: this is really fragile, needs to be redesigned
			base.Reset();

			Mass = 1;          // Mass (defaults to 1 so acceleration=force)
			Speed = 0;         // speed along Forward direction.

			Radius = 0.5f;     // size of bounding sphere

			MaxForce = 0.1f;   // steering force is clipped to this magnitude
			MaxSpeed = 1.0f;   // velocity is clipped to this magnitude

			// reset bookkeeping to do running averages of these quanities
			ResetSmoothedPosition();
			ResetSmoothedCurvature();
			ResetAcceleration();
		}

		// get/set Mass
		public override float Mass
		{
			get { return mass; }
			set { mass = value; }
		}

		// get velocity of vehicle
        public override Vector3 Velocity
		{
			get { return Forward * speed; }
		}

		// get/set speed of vehicle  (may be faster than taking mag of velocity)
		public override float Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		// size of bounding sphere, for obstacle avoidance, etc.
		public override float Radius
		{
			get { return radius; }
			set { radius = value; }
		}

		// get/set maxForce
		public override float MaxForce
		{
			get { return maxForce; }
			set { maxForce = value; }
		}

		// get/set maxSpeed
		public override float MaxSpeed
		{
			get { return maxSpeed; }
			set { maxSpeed = value; }
		}

        public Vector3 ClippedForce
        {
            get;
            set;
        }
        

		// apply a given steering force to our momentum,
		// adjusting our orientation to maintain velocity-alignment.
        public void ApplySteeringForce(Vector3 force, float elapsedTime)
		{
			//Vector3 adjustedForce = AdjustRawSteeringForce(force, elapsedTime);

			// enforce limit on magnitude of steering force
            ClippedForce = VectorUtils.TruncateLength(force, MaxForce);

			// compute acceleration and velocity
            Vector3 newAcceleration = (ClippedForce / Mass);
			Vector3 newVelocity = Velocity;

			// damp out abrupt changes and oscillations in steering acceleration
			// (rate is proportional to time step, then clipped into useful range)
			if (elapsedTime > 0)
			{
				float smoothRate = MathUtils.Clip(9 * elapsedTime, 0.15f, 0.4f);
				MathUtils.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
			}

			// Euler integrate (per frame) acceleration into velocity
			newVelocity += acceleration * elapsedTime;

			// enforce speed limit
            newVelocity = VectorUtils.TruncateLength(newVelocity, MaxSpeed);

            /////////////////////////////////SO A SPEED EH ALTERADA

			// update Speed
			Speed = (newVelocity.Length());   //aqui

			// Euler integrate (per frame) velocity into position
            Position = (Position + (newVelocity * elapsedTime));   //aqui




			// regenerate local space (by default: align vehicle's forward axis with
			// new velocity, but this behavior may be overridden by derived classes.)
            RegenerateLocalSpace(newVelocity, elapsedTime);                //aqui

			// maintain path curvature information
            MeasurePathCurvature(elapsedTime);                             //aqui   usa POSITION

			// running average of recent positions
			MathUtils.BlendIntoAccumulator(elapsedTime * 0.06f, // QQQ
								  Position,
                                  ref smoothedPosition); //aqui
		}

		// the default version: keep FORWARD parallel to velocity, change
		// UP as little as possible.
        public virtual void RegenerateLocalSpace(Vector3 newVelocity, float elapsedTime)
		{
			// adjust orthonormal basis vectors to be aligned with new velocity
			if (Speed > 0)
			{
				RegenerateOrthonormalBasisUF(newVelocity / Speed);
			}
		}

		// alternate version: keep FORWARD parallel to velocity, adjust UP
		// according to a no-basis-in-reality "banking" behavior, something
		// like what birds and airplanes do.  (XXX experimental cwr 6-5-03)
        public void RegenerateLocalSpaceForBanking(Vector3 newVelocity, float elapsedTime)
		{
			// the length of this global-upward-pointing vector controls the vehicle's
			// tendency to right itself as it is rolled over from turning acceleration
			Vector3 globalUp = new Vector3(0, 0.2f, 0);

			// acceleration points toward the center of local path curvature, the
			// length determines how much the vehicle will roll while turning
			Vector3 accelUp = acceleration * 0.05f;

			// combined banking, sum of UP due to turning and global UP
			Vector3 bankUp = accelUp + globalUp;

			// blend bankUp into vehicle's UP basis vector
			float smoothRate = elapsedTime * 3;
			Vector3 tempUp = Up;
            MathUtils.BlendIntoAccumulator(smoothRate, bankUp, ref tempUp);
			Up = tempUp;
            Up.Normalize();

			annotation.Line(Position, Position + (globalUp * 4), Color.White);
			annotation.Line(Position, Position + (bankUp * 4), Color.Orange);
			annotation.Line(Position, Position + (accelUp * 4), Color.Red);
			annotation.Line(Position, Position + (Up * 1), Color.Yellow);

			// adjust orthonormal basis vectors to be aligned with new velocity
			if (Speed > 0) RegenerateOrthonormalBasisUF(newVelocity / Speed);
		}

		// adjust the steering force passed to applySteeringForce.
		// allows a specific vehicle class to redefine this adjustment.
		// default is to disallow backward-facing steering at low speed.
		// xxx experimental 8-20-02
        public virtual Vector3 AdjustRawSteeringForce(Vector3 force, float deltaTime)
		{
			float maxAdjustedSpeed = 0.2f * MaxSpeed;

			if ((Speed > maxAdjustedSpeed) || (force == Vector3.Zero))
			{
				return force;
			}
			else
			{
				float range = Speed / maxAdjustedSpeed;
                float cosine = MathUtils.Interpolate((float)Math.Pow(range, 20), 1.0f, -1.0f);
				return VectorUtils.LimitMaxDeviationAngle(force, cosine, Forward);
			}
		}

		// apply a given braking force (for a given dt) to our momentum.
		// xxx experimental 9-6-02
		public void ApplyBrakingForce(float rate, float deltaTime)
		{
			float rawBraking = Speed * rate;
			float clipBraking = ((rawBraking < MaxForce) ? rawBraking : MaxForce);
			Speed = (Speed - (clipBraking * deltaTime));
		}

		// predict position of this vehicle at some time in the future
		// (assumes velocity remains constant)
        public override Vector3 PredictFuturePosition(float predictionTime)
		{
			return Position + (Velocity * predictionTime);
		}

		// get instantaneous curvature (since last update)
		public float Curvature
		{
			get { return curvature; }
		}

		// get/reset smoothedCurvature, smoothedAcceleration and smoothedPosition
		public float SmoothedCurvature
		{
			get { return smoothedCurvature; }
		}
		public float ResetSmoothedCurvature()
		{
			return ResetSmoothedCurvature(0);
		}
		public float ResetSmoothedCurvature(float value)
		{
			lastForward = Vector3.Zero;
			lastPosition = Vector3.Zero;
			return smoothedCurvature = curvature = value;
		}

		public override Vector3 Acceleration
		{
			get { return acceleration; }
		}
        public Vector3 ResetAcceleration()
		{
			return ResetAcceleration(Vector3.Zero);
		}
        public Vector3 ResetAcceleration(Vector3 value)
		{
			return acceleration = value;
		}

        public Vector3 SmoothedPosition
		{
			get { return smoothedPosition; }
		}
        public Vector3 ResetSmoothedPosition()
		{
			return ResetSmoothedPosition(Vector3.Zero);
		}
        public Vector3 ResetSmoothedPosition(Vector3 value)
		{
			return smoothedPosition = value;
		}

		// set a random "2D" heading: set local Up to global Y, then effectively
		// rotate about it by a random angle (pick random forward, derive side).
		public void RandomizeHeadingOnXZPlane()
		{
			Up = Vector3.Up;
            Forward = VectorUtils.RandomUnitVectorOnXZPlane();
			Side = LocalRotateForwardToSide(Forward);
		}

		// measure path curvature (1/turning-radius), maintain smoothed version
		void MeasurePathCurvature(float elapsedTime)
		{
			if (elapsedTime > 0)
			{
				Vector3 dP = lastPosition - Position;
				Vector3 dF = (lastForward - Forward) / dP.Length();
                Vector3 lateral = VectorUtils.PerpendicularComponent(dF, Forward);
                float sign = (Vector3.Dot(lateral, Side) < 0) ? 1.0f : -1.0f;
				curvature = lateral.Length() * sign;
                MathUtils.BlendIntoAccumulator(elapsedTime * 4.0f, curvature, ref smoothedCurvature);
				lastForward = Forward;
				lastPosition = Position;
			}
		}
	}
}
