namespace TypeSafe_API.Models
{
    public class ModelDesignUploadImage
    {
        public int? DesignId { get; set; }
        public int? Id { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }
        public byte[]? TriggerImageData { get; set; }
        public float? ImageArea { get; set; }
        public float? TriggerImageArea { get; set; }
        public float? TriggerImagePresentage { get; set; }
        public float? TriggerImagePossibilty { get; set; }
        public bool? IsTriggerImagePossibilty { get; set; }
    }
}
