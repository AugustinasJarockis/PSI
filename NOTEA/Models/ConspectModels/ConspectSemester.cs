using System.ComponentModel.DataAnnotations;

namespace NOTEA.Models.ConspectModels
{
    public enum ConspectSemester
    {
        [Display(Name = "Unknown")] Unknown = 0,
        [Display(Name = "2022 autumn")] Autumn_2022 = 1,
        [Display(Name = "2023 spring")] Spring_2023 = 2,
        [Display(Name = "2023 autumn")] Autumn_2023 = 3,
        [Display(Name = "2024 spring")] Spring_2024 = 4,
        [Display(Name = "2024 autumn")] Autumn_2024 = 5,


    }
}
