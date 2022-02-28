namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }//Id is special in EF
        public string UserName { get; set; } //Upper N due to identity
    }
}