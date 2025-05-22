using Silk.NET.Maths;
using static GrafikaSzeminarium.CameraDescriptor;

namespace GrafikaSzeminarium
{
    internal class CameraDescriptor
    {
        private double DistanceToOrigin = 4;

        private double AngleToZYPlane = 0;

        private double AngleToZXPlane = 0;

        private const double DistanceScaleFactor = 1.1;

        private const double AngleChangeStepSize = Math.PI / 180 * 5;

        public enum CameraMode { Default, RedBallFirstPerson, RedBallThirdPerson }
        private CameraMode currentMode = CameraMode.Default;

        private Vector3D<float> redballPosition = Vector3D<float>.Zero;


        private Vector3D<float> redBallPosition = Vector3D<float>.Zero;
        private Vector3D<float> redBallForward = new(0, 0, 1); // alapértelmezett előre néző irány


        public void UpdateRedBallPosition(Vector3D<float> position)
        {
            redBallPosition = position;
        }

        public void UpdateRedBallForward(Vector3D<float> forward)
        {
            redBallForward = Vector3D.Normalize(forward);
        }

        /// <summary>
        /// Gets the position of the camera.
        /// </summary>


        public Vector3D<float> Position
        {
            get
            {
                return currentMode switch
                {
                    CameraMode.RedBallFirstPerson => redBallPosition + new Vector3D<float>(0, 2f, 0), // szemmagasság
                    CameraMode.RedBallThirdPerson => redBallPosition + new Vector3D<float>(0, 4f, -6f), // hátul-felül
                    _ => GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane)
                };
            }
        }

        /// <summary>
        /// Gets the up vector of the camera.
        /// </summary>
        public Vector3D<float> UpVector
        {
            get
            {
                return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane + Math.PI / 2));
            }
        }

        /// <summary>
        /// Gets the target point of the camera view.
        /// </summary>
        public Vector3D<float> Target
        {
            get
            {
                return currentMode switch
                {
                    CameraMode.RedBallFirstPerson => redBallPosition + redBallForward * 10f,
                    CameraMode.RedBallThirdPerson => redBallPosition,
                    _ => Vector3D<float>.Zero
                };
            }
        }

        public void IncreaseZXAngle()
        {
            AngleToZXPlane += AngleChangeStepSize;
        }

        public void DecreaseZXAngle()
        {
            AngleToZXPlane -= AngleChangeStepSize;
        }

        public void IncreaseZYAngle()
        {
            AngleToZYPlane += AngleChangeStepSize;

        }

        public void DecreaseZYAngle()
        {
            AngleToZYPlane -= AngleChangeStepSize;
        }

        public void IncreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin * DistanceScaleFactor;
        }

        public void DecreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin / DistanceScaleFactor;
        }

        private static Vector3D<float> GetPointFromAngles(double distanceToOrigin, double angleToMinZYPlane, double angleToMinZXPlane)
        {
            var x = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Sin(angleToMinZYPlane);
            var z = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Cos(angleToMinZYPlane);
            var y = distanceToOrigin * Math.Sin(angleToMinZXPlane);

            return new Vector3D<float>((float)x, (float)y, (float)z);
        }

        public void SetCameraMode(CameraMode mode)
        {
            currentMode = mode;
        }

        private Vector3D<float> GetFirstPersonPosition()
        {
            return redballPosition + new Vector3D<float>(0f, 2f, 0f);
        }

        private Vector3D<float> GetThirdPersonPosition()
        {

            return redballPosition + new Vector3D<float>(5f, 4f, -4f);
        }
    }
}
