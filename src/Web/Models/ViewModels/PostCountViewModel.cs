namespace NetBooru.Web.Models
{
    public class PostCountViewModel
    {
        public int PostCount { get; set; }

        public string PostCountString => PostCount.ToString();
    }
}
