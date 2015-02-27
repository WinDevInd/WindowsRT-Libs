using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3
{
    class Class1
    {
        //        public class BookCollection : IncrementalLoadingList<BookSummaryInfo>
        //{
        //private BookSearchDataContext _bookDataContext;

        //public BookCollection(BookSearchDataContext bookDataContext)
        //{
        //   this._bookDataContext = bookDataContext;
        //} 

        //protected async override Task<int> LoadNextItemsAsync(int count)
        //{
        //     if (_bookDataContext != null)
        //     {
        //         //Network request and get Data
        //         SearchResponse response = await _bookDataContext.SearchBooks(location.Latitude, 

        //                                                                                location.Longitude, _currentpage);
        //             _currentpage++;
        //             if (response != null && response.Search != null && response.Search.Length > 0)
        //             {
        //                 foreach (var bookinfo in response.Search)
        //                 {
        //                       //Add data to Collection
        //                        this.Add(new BookSummaryInfo(bookinfo));
        //                 }
        //                //return the no of items added
        //                return response.Search.Length;
        //             }
        //       }
        //       return 0;
        //       }
    }
}
