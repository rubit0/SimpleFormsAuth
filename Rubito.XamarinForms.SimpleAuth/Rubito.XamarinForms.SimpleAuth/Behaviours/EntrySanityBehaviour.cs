using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Rubito.SimpleFormsAuth.Behaviours
{
    public interface IEntryValidator
    {
        Tuple<bool, string> Validate();
    }

    public class EntrySanityBehaviour : Behavior<Entry>, IEntryValidator
    {
        public bool FailOnEmtpyEntry { get; set; } = true;
        public bool CheckIsEmail { get; set; } = false;
        public bool DoRemoveWhiteSpace { get; set; } = true;
        public uint MinCharLength { get; set; } = 2;

        Entry context;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(context);
            context = bindable;
            context.TextChanged += OnTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            context.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(context);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (DoRemoveWhiteSpace)
                RemoveWhiteSpace(e.NewTextValue);
        }

        private bool CheckInputIsEmail()
        {
            return Regex.IsMatch(context.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        private void RemoveWhiteSpace(string input)
        {
            context.Text = input.Replace(" ", "");
        }

        public Tuple<bool, string> Validate()
        {
            var valid = true;
            var message = string.Empty;

            if (FailOnEmtpyEntry)
            {
                if (string.IsNullOrWhiteSpace(context.Text))
                {
                    valid = false;
                    message = $"{context.Placeholder} can not be empty.";
                }
            }

            if (valid && context.Text.Length < this.MinCharLength)
            {
                valid = false;
                message = $"{context.Placeholder} must be at least {this.MinCharLength} characters long.";
            }

            if (valid && CheckIsEmail && !CheckInputIsEmail())
            {
                valid = false;
                message = "Invalid email address.";
            }

            return new Tuple<bool, string>(valid, message);
        }
    }
}
