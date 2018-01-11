namespace Climb
{
    public struct ImageRules
    {
        public readonly int maxFileSize;
        public readonly int width;
        public readonly int height;
        public readonly string missingUrl;

        public ImageRules(int maxFileSize, int width, int height, string missingUrl = null)
        {
            this.maxFileSize = maxFileSize;
            this.width = width;
            this.height = height;
            this.missingUrl = missingUrl;
        }
    }
}