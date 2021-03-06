using System;
using System.Collections.Generic;
using System.Linq;
using StillDesign.PhysX;
using StillDesign.PhysX.MathPrimitives;

namespace StillDesign.PhysX.Samples
{
	public class Vehicle
	{
        public ActorDescription ActorDesc;
        private void LoadVehiclePhysics(Scene scene)
        {
            // Create a 2 ton car with 4 wheels
            BodyDescription bodyDesc = new BodyDescription()
            {
                Mass = 2000
            };
            //bodyDesc.MassLocalPose = Matrix.CreateTranslation( 0, -1.5f, 0 ); // Seems not to be working

            ActorDesc = new ActorDescription()
            {
                BodyDescription = bodyDesc,
                Shapes = { new BoxShapeDescription(5, 3, 7) },
                GlobalPose = Matrix.Translation(-50, 5, -70)
            };

            VehicleBodyActor = scene.CreateActor(ActorDesc);
            VehicleBodyActor.SetCenterOfMassOffsetLocalPosition(new Vector3(0, -1.5f, 0)); // Move the COM to the bottom of the vehicle to stop it flipping over so much

            //

            WheelShapeDescription leftFrontDesc = new WheelShapeDescription()
            {
                Radius = 0.8f,
                SuspensionTravel = 1,
                LocalPosition = new Vector3(-2.5f, -1, 3)
            };

            WheelShapeDescription leftRearDesc = new WheelShapeDescription()
            {
                Radius = 0.8f,
                SuspensionTravel = 1,
                LocalPosition = new Vector3(-2.5f, -1, -3),
            };

            WheelShapeDescription rightFrontDesc = new WheelShapeDescription()
            {
                Radius = 0.8f,
                SuspensionTravel = 1,
                LocalPosition = new Vector3(2.5f, -1, 3)
            };

            WheelShapeDescription rightRearDesc = new WheelShapeDescription()
            {
                Radius = 0.8f,
                SuspensionTravel = 1,
                LocalPosition = new Vector3(2.5f, -1, -3)
            };

            this.LeftFront = VehicleBodyActor.CreateShape(leftFrontDesc) as WheelShape;
            this.LeftRear = VehicleBodyActor.CreateShape(leftRearDesc) as WheelShape;
            this.RightFront = VehicleBodyActor.CreateShape(rightFrontDesc) as WheelShape;
            this.RightRear = VehicleBodyActor.CreateShape(rightRearDesc) as WheelShape;
        }

		public Actor VehicleBodyActor;

        public Vehicle(Scene scene)
		{
			LoadVehiclePhysics(scene);
		}

		public void Accelerate(float additionalMotorTorque)
		{
			this.LeftRear.MotorTorque += additionalMotorTorque;
			this.RightRear.MotorTorque += additionalMotorTorque;

			VehicleBodyActor.WakeUp();
		}
		public void Turn(float additionalSteeringAngle)
		{
			// Both wheels will have the same steering angle
			float currentAngle = this.LeftFront.SteeringAngle;

			// About 34 degrees of max steering lock
			float newAngle = Microsoft.Xna.Framework.MathHelper.Clamp(currentAngle + additionalSteeringAngle, -0.6f, 0.6f);

			this.LeftFront.SteeringAngle = this.RightFront.SteeringAngle = newAngle;
		}

		public WheelShape LeftFront { get; private set; }
		public WheelShape LeftRear { get; private set; }
		public WheelShape RightFront { get; private set; }
		public WheelShape RightRear { get; private set; }
	}
}