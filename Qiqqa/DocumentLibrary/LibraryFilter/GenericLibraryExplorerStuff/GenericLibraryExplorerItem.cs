using System.Collections.Generic;
using Qiqqa.DocumentLibrary.TagExplorerStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;

namespace Qiqqa.DocumentLibrary.LibraryFilter.GenericLibraryExplorerStuff
{
    public class GenericLibraryExplorerItem
    {
        public GenericLibraryExplorerControl GenericLibraryExplorerControl;
        public WebLibraryDetail web_library_detail;

        public string tag;
        public HashSet<string> fingerprints;

        public GenericLibraryExplorerControl.OnItemPopupDelegate OnItemPopup;
        public GenericLibraryExplorerControl.OnItemDragOverDelegate OnItemDragOver;
        public GenericLibraryExplorerControl.OnItemDropDelegate OnItemDrop;

        public bool IsSelected;
    }
}
