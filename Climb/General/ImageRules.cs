namespace Climb
{
    public struct ImageRules
    {
        public readonly int maxFileSize;
        public readonly int width;
        public readonly int height;
        public readonly string missingUrl;
        public readonly string folder;

        public ImageRules(int maxFileSize, int width, int height, string folder, string missingUrl = null)
        {
            this.maxFileSize = maxFileSize;
            this.width = width;
            this.height = height;
            this.folder = folder;
            this.missingUrl = missingUrl;
        }
    }
}