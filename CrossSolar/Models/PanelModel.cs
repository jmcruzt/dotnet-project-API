using System.ComponentModel.DataAnnotations;

namespace CrossSolar.Models
{
    public class PanelModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Latitude is Required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        [RegularExpression(@"^\d+(\.\d{6})$", ErrorMessage = "Latitude must have 6 decimals")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is Required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        [RegularExpression(@"^\d+(\.\d{6})$", ErrorMessage = "Longitude must have 6 decimals")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Serial Number is Required")]
        [MinLength(16, ErrorMessage = "Serial Number must be 16 characters length")]
        public string Serial { get; set; }

        public string Brand { get; set; }

    }
}
