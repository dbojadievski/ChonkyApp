using System;
using System.Collections.Generic;
using System.Text;

namespace ChonkyApp.Models
{
    public class DailyGoal: BaseModel
    {
        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public DailyGoal(string name, Guid? guid = null)
        {
            Name = name;
            if (guid.HasValue)
                ID = guid.Value;
        }
    }

    public class DailyGoalEntry: BaseModel
    {
        private Guid goaID;

        public Guid GoalID
        {
            get => goaID;
            set => SetProperty(ref goaID, value);
        }
        
    }
}
