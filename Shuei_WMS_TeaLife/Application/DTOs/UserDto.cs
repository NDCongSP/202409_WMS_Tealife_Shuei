namespace Application.DTOs
{
    public class UserDto
    {
        public UserDto()
        {
        }

        public UserDto(ApplicationUser user)
        {
            Id = user.Id.ToString();
            FullName = user.FullName;
            Localtion = user.Localtion;
            Status = user.Status;
        }

        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Localtion { get; set; }
        public EnumStatus? Status { get; set; }
    }
}
