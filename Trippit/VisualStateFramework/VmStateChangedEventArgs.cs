namespace Trippit.VisualStateFramework
{
    public class VmStateChangedEventArgs
    {        
        public string NewStateName { get; set; }

        public VmStateChangedEventArgs()
        {
            NewStateName = null;            
        }

        public VmStateChangedEventArgs(string newStateName)
        {            
            NewStateName = newStateName;
        }
    }
}