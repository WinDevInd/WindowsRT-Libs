# IncrementalLoadingCollection
Incremental Loading ready with Observable-Collection, Observable-List, ObservableDictionary

http://windowsphonelearning.blogspot.com/2015/02/incrementalloading-in-windows-phone-81.html

# Example

    public class BookCollection : IncrementalLoadingList<BookSummaryInfo>
            {
                private BookSearchDataContext _bookDataContext;
    
                public BookCollection(BookSearchDataContext bookDataContext)
                {
                    this._bookDataContext = bookDataContext;
                }
    
                protected async override Task<int> LoadNextItemsAsync(int count)
                {
                    if (_bookDataContext != null)
                    {
                       //Network request and get Data
                       SearchResponse response = await _bookDataContext.SearchBooks(location.Latitude, location.Longitude, _currentpage);
                        _currentpage++;
                        if (response != null && response.Search != null && response.Search.Length > 0)
                        {
                            foreach (var bookinfo in response.Search)
                            {
                                //Add data to Collection
                                this.Add(new BookSummaryInfo(bookinfo));
                            }
                            //return the no of items added
                            return response.Search.Length;
                        }
                    }
                    return 0;
                }
            
                protected override bool CanLoadMoreItems()
                {
                    ////your Own logic or network can tell if you have more items
                    return _bookDataContext.hasMore;
                }
        }
        
        ////Your View-Model which contain the IncrementalLoadingCollection
        public class BookDataVM
        {
            BookSearchDataContext data;
            bool _IsBusy;
            public BookDataVM()
            {
                data = new BookSearchDataContext();
                ////Collection with Incrmental Loading capability
                BookCollection = new BookCollection(data);
            }
        }  

# Test Example coming soon in Repo
# Feel free to send me the issues or create pull request for changes you want for library...
