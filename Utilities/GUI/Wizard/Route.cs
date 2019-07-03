namespace Utilities.GUI.Wizard
{
    public class Route
    {
        private string title;
        private Step[] steps;
        string completion_message;
        private int current_step_index = 0;

        public Route(string title, Step[] steps, string completion_message)
        {
            this.title = title;
            this.steps = steps;
            this.completion_message = completion_message;
            this.current_step_index = 0;
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public string CompletionMessage
        {
            get
            {
                return completion_message;
            }
        }

        public int CurrentStepIndex
        {
            get
            {
                return current_step_index;
            }
        }

        public int MaxStepIndex
        {
            get
            {
                return steps.Length;
            }
        }

        public Step CurrentStep
        {
            get
            {
                if (current_step_index < steps.Length)
                {
                    return steps[current_step_index];
                }
                else
                {
                    return null;
                }
            }
        }

        public void StepNext()
        {
            ++current_step_index;
        }
    }
}
