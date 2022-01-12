using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ChonkyApp.Behaviors
{
    internal class NumericValidationBehavior: Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += Entry_TextChanged;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= Entry_TextChanged;
            base.OnDetachingFrom(entry);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newValue = e.NewTextValue;
            bool isValid = Double.TryParse(newValue, out double val);

            Entry asEntry = sender as Entry;
            asEntry.Text = isValid ? e.NewTextValue : e.OldTextValue;
        }
    }

    internal class PositiveNumericValidationBehavior: Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += Entry_TextChanged;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= Entry_TextChanged;
            base.OnDetachingFrom(entry);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newValue = e.NewTextValue;
            bool isValid = Double.TryParse(newValue, out double val);
            isValid &= (val > 0);

            Entry asEntry = sender as Entry;
            asEntry.Text = isValid ? e.NewTextValue : e.OldTextValue;
        }

        private static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            var entry = view as Entry;
            if (entry == null)
            {
                return;
            }

            bool attachBehavior = (bool)newValue;
            if (attachBehavior)
            {
                entry.Behaviors.Add(new NumericValidationBehavior());
            }
            else
            {
                var toRemove = entry.Behaviors.FirstOrDefault(b => b is NumericValidationBehavior);
                if (toRemove != null)
                {
                    entry.Behaviors.Remove(toRemove);
                }
            }
        }
    }
}
