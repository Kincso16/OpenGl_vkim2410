namespace GrafikaSzeminarium
{
    internal class RectangleArrangementModel
    {
        /// <summary>
        /// The time of the simulation. It helps to calculate time dependent values.
        /// </summary>
        private double Time { get; set; } = 0;

        internal void AdvanceTime(double deltaTime)
        {
            // set a simulation time
            Time += deltaTime;
        }
    }
}
