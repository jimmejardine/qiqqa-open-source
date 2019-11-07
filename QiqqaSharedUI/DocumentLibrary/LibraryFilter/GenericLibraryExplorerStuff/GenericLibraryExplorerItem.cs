using System.Collections.Generic;
using Qiqqa.DocumentLibrary.TagExplorerStuff;

namespace Qiqqa.DocumentLibrary.LibraryFilter.GenericLibraryExplorerStuff
{
    public class GenericLibraryExplorerItem
    {
        public GenericLibraryExplorerControl GenericLibraryExplorerControl;
        public Library library;

        public string tag;
        public HashSet<string> fingerprints;

        public GenericLibraryExplorerControl.OnItemPopupDelegate OnItemPopup;
        public GenericLibraryExplorerControl.OnItemDragOverDelegate OnItemDragOver;
        public GenericLibraryExplorerControl.OnItemDropDelegate OnItemDrop;

        public bool IsSelected;
    }
}
