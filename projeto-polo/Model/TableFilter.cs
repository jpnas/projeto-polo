namespace projeto_polo.Model
{
    public class TableFilter
    {
        public bool ShowIPCA {  get; set; }
        public bool ShowIGPM { get; set; }
        public bool ShowSelic { get; set; } 
        public DateTime FromDate {  get; set; }
        public DateTime ToDate { get; set; }
    }
}
