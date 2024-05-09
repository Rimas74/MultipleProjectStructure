namespace MultipleProjectStructure.Database.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public byte[] Data { get; set; }

        public Thumbnail Thumbnail { get; set; }
    }
}
