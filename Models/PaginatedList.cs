namespace CountryBlockingAPI.Models;

public class PaginatedList<T> // used generic type param to allow the class to work with blocked countries, logs ,...
{
    public List<T> Items { get; set; } = new List<T>();
    public int PageIndex { get; set; } // page number 
    public int TotalPages { get; set; }
    public int TotalCount { get; set; } // total num of items across all pages 

    public bool HasPreviousPage => PageIndex > 1 ;
    public bool HasNextPage => PageIndex < TotalPages ;

    public PaginatedList(List<T> items , int count , int pageIndex, int pageSize) 
    {
        // handle pagination requirements specified in the assignement. (get all blocked countries , log failed blocked
        // attempts 
        Items = items ; 
        PageIndex = pageIndex ;
        TotalPages = (int)Math.Ceiling(count/(double)pageSize) ;
        TotalCount = count ;
    }

    public static PaginatedList<T> Create(IQueryable<T>source , int pageIndex , int pageSize)
    {
        var list = source.ToList();
        var count = list.Count;
        var items = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {

        var list = source.ToList();
        var count = list.Count;
        var items  list.Skip((pageIndex- 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }



}