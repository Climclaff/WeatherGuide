using System;

namespace WeatherGuide.Models.ViewModels
{
    [Serializable]
    public class RecommendationViewModel
    {
        public byte[] FirstClothingImage { get; set; }

        public string FirstClothingNameEN { get; set; }

        public string FirstClothingNameUA { get; set; }

        public byte[] SecondClothingImage { get; set; }

        public string SecondClothingNameEN { get; set; }

        public string SecondClothingNameUA { get; set; }

        public byte[] ThirdClothingImage { get; set; }

        public string ThirdClothingNameEN { get; set; }

        public string ThirdClothingNameUA { get; set; }


    }
}
