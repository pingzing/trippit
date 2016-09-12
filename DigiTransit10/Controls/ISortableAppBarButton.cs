using System;

namespace DigiTransit10.Controls
{
    public interface ISortableAppBarButton : IComparable<ISortableAppBarButton>
    {
        int Position { get; set; }
        bool IsSecondaryCommand { get; set; }
    }
}
