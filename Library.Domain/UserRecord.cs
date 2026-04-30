public class UserRecord
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool HasPendingFines { get; set; }

    public decimal TotalFines { get; set; }
}
