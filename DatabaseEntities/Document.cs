public struct Document
{
    public string Hash { get; set; }
    public string FileId { get; set; }
    public string Caption { get; set; }
    public long UserId { get; set; }

    public Document(string hash, string fileId, string caption, long userId)
    {
        Hash = hash;
        FileId = fileId;
        Caption = caption;
        UserId = userId;
    }
}