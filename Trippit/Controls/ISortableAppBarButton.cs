using System;

namespace Trippit.Controls
{
    public interface ISortableAppBarButton : IComparable<ISortableAppBarButton>
    {
        int Position { get; set; }
        bool IsSecondaryCommand { get; set; }
    }
}
