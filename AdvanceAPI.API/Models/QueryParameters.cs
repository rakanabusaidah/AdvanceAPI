namespace AdvanceAPI.API.Models
{
    public class QueryParameters
    {
        const int _maxSize = 100;
        private int _size = 50;

        public int Page { get; set; } = 1;
        // should set value of size that is not more than _maxSize
        public int Size
        {
            get { return _size; }
            set
            {
                _size = Math.Min(_maxSize, value);
            }
        }
        public string SortBy { get; set; } = "Id";
        private string _sortOrder = "asc";
        public string SortOrder 
        { 
            get 
            {
                return _sortOrder; 
            } 
            set 
            {
                if (value == "asc" || value == "desc")
                _sortOrder = value; 
            }
        }
    }
}
