namespace MonoPhysics.Core
{
    public enum ScreenResolution
    {
        R720p = 0,  // 1280x720
        R900p = 1,  // 1600x900
        R1080p = 2, // 1920x1080
    }

    public static class ResolutionHelper
    {
        #region Public Methods

        public static (int width, int height) GetResolution(ScreenResolution resolution)
        {
            return resolution switch
            {
                ScreenResolution.R720p => (1280, 720),
                ScreenResolution.R900p => (1600, 900),
                ScreenResolution.R1080p => (1920, 1080),
                _ => (1280, 720),
            };
        }

        #endregion Public Methods
    }
}