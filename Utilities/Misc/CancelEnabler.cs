namespace Utilities.Misc
{
    public class CancelEnabler
    {
        private bool cancelled = false;

        public CancelEnabler()
        {
        }

        public CancelEnabler Cancel()
        {
            cancelled = true;

            return new CancelEnabler();
        }

        public bool IsCancelled
        {
            get
            {
                return cancelled;
            }
        }
    }
}
