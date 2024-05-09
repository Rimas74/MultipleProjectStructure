namespace MultipleProjectStructure.Database.Entities
{
    public class Thumbnail
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public required Image Image { get; set; }
        public required string FileName { get; set; }
        public byte[]? Data { get; set; }
    }
}
