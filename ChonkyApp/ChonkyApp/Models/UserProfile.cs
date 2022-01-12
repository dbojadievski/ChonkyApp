using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChonkyApp.Models
{
    public enum Sex
    {
        Male = 0,
        Female
    }

    public enum UserGoal
    {
        Gain = 0,
        Lose = 1
    }

    public class UserProfile: BaseModel
    {
        private string firstName;
        private string lastName;

        private string emailAddress;

        private Sex sex;

        private double height;
        private DateTime birthDate;

        private UserGoal goal;

        public string FirstName
        {
            get => firstName;
            set { SetProperty(ref firstName, value); }
        }

        public string LastName
        {
            get => lastName;
            set { SetProperty(ref lastName, value); }
        }

        public string EmailAddress
        {
            get => emailAddress;
            set { SetProperty(ref emailAddress, value); }
        }

        public Sex Sex
        {
            get => sex;
            set { SetProperty(ref sex, value); }
        }

        public double Height
        {
            get => height;
            set { SetProperty(ref height, value); }
        }

        public DateTime BirthDate
        {
            get => birthDate;
            set { SetProperty(ref birthDate, value); }
        }

        public int AgeInYears
        {
            get => (DateTime.Now.Year - BirthDate.Year - 1);
        }

        public UserGoal Goal
        {
            get => goal;
            set { SetProperty(ref goal, value); }
        }
    }
}
