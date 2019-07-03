namespace Utilities.GUI.Wizard
{
    public class Step
    {
        public delegate bool PostConditionDelegate(PointOfInterestLocator point_of_interest_locator);

        public string Instructions = "";
        public string[] PointOfInterests = new string[] {};
        public PostConditionDelegate PostCondition = (point_of_interest_locator) => { return true; };
        public bool PostCondition_GreyInstructions = true;
        public string[] PostCondition_PointOfInterests = new string[] { };
    }
}
