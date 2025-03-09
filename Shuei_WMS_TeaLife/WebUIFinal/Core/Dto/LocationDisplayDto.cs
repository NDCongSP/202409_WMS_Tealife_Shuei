using System.ComponentModel.DataAnnotations;

namespace WebUIFinal.Core.Dto
{
    public class LocationDisplayDto
    {
        [Required]
        public string Id { get; set; }
        public string? LocationName { get; set; }

        public LocationDisplayDto() { }
        public LocationDisplayDto (Location l)
        {
            Id = (l.Id).ToString();
            LocationName = l.LocationName;
        }
    }
}
