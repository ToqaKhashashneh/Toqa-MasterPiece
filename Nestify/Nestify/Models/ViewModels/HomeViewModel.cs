namespace Nestify.Models.ViewModels
{

    public class HomeViewModel
    {
        public List<Property> LatestProperties { get; set; } = new List<Property>();

        public List<Property> FeaturedProperties { get; set; } = new List<Property>();
    }

}
