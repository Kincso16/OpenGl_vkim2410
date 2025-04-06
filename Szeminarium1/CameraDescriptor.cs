using Silk.NET.Input;
using System.Numerics;
using Silk.NET.Maths;

namespace GrafikaSzeminarium
{
    internal class CameraDescriptor
    {
        // Setup the camera's location, directions, and movement speed
        private static Vector3D<float> CameraPosition = new Vector3D<float>(0.0f, 0.0f, 8.0f);
        private static Vector3D<float> CameraFront = new Vector3D<float>(0.0f, 0.0f, -1.0f);
        private static Vector3D<float> CameraUp = Vector3D<float>.UnitY; 
        private static float CameraYaw = -90f;
        private static float CameraPitch = 0f;
        private static float CameraZoom = 45f;
        private static float MaxPitch = 89f; 

        private static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }

        // Calculate Camera's Front Direction based on yaw and pitch
        private void UpdateCameraFront()
        {
            var front = new Vector3D<float>(
                MathF.Cos(DegreesToRadians(CameraYaw)) * MathF.Cos(DegreesToRadians(CameraPitch)),
                MathF.Sin(DegreesToRadians(CameraPitch)),
                MathF.Sin(DegreesToRadians(CameraYaw)) * MathF.Cos(DegreesToRadians(CameraPitch))
            );

            CameraFront = Vector3D.Normalize(front);
        }

        public Matrix4X4<float> getView()
        {
            return Matrix4X4.CreateLookAt<float>(CameraPosition, CameraPosition + CameraFront, CameraUp);
        }

        public Matrix4X4<float> getProjection(Vector2D<int> size)
        {
            return Matrix4X4.CreatePerspectiveFieldOfView(DegreesToRadians(CameraZoom), (float)size.X / size.Y, 0.1f, 100.0f);
        }

        // Move the camera upward
        public void MoveUp(float moveSpeed)
        {
            CameraPosition += moveSpeed * CameraUp;
        }

        // Move the camera downward
        public void MoveDown(float moveSpeed)
        {
            CameraPosition -= moveSpeed * CameraUp;
        }

        // Move the camera to the right
        public void MoveRight(float moveSpeed)
        {
            CameraPosition += Vector3D.Normalize(Vector3D.Cross(CameraFront, CameraUp)) * moveSpeed;
        }

        // Move the camera to the left
        public void MoveLeft(float moveSpeed)
        {
            CameraPosition -= Vector3D.Normalize(Vector3D.Cross(CameraFront, CameraUp)) * moveSpeed;
        }

        // Move the camera forward
        public void MoveForward(float moveSpeed)
        {
            CameraPosition += moveSpeed * CameraFront;
        }

        // Move the camera backward
        public void MoveBackward(float moveSpeed)
        {
            CameraPosition -= moveSpeed * CameraFront;
        }

        // Look up (tilt camera upwards)
        public void LookUp(float pitchSpeed)
        {
            CameraPitch += pitchSpeed;
            if (CameraPitch > MaxPitch) CameraPitch = MaxPitch; 
            UpdateCameraFront();
        }

        // Look down (tilt camera downwards)
        public void LookDown(float pitchSpeed)
        {
            CameraPitch -= pitchSpeed;
            if (CameraPitch < -MaxPitch) CameraPitch = -MaxPitch; 
            UpdateCameraFront();
        }

        // Look left (tilt camera to the left)
        public void LookLeft(float angle)
        {
            CameraYaw -= angle; 
            if (CameraYaw < -180.0f) CameraYaw += 360.0f;
        }

        // Look right (tilt camera to the right)
        public void LookRight(float angle)
        {
            CameraYaw += angle; 
            if (CameraYaw > 180.0f) CameraYaw -= 360.0f; 
        }
    }
}
