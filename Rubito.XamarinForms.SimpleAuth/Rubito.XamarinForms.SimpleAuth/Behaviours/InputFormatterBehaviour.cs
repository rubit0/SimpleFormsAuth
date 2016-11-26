using Xamarin.Forms;

namespace Rubito.XamarinForms.SimpleAuth.Behaviours
{
    public class InputFormatterBehaviour : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnTextChanged;

            base.OnAttachedTo(bindable);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var context = (Entry)sender;
            context.Text = e.NewTextValue.Replace(" ", "");
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnTextChanged;

            base.OnDetachingFrom(bindable);
        }
    }
}
