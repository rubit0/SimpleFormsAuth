using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rubito.XamarinForms.SimpleAuth.Behaviours
{
    public class TopGroupBehaviour : Behavior<View>
    {
        public Label Title { get; set; }
        public Label TitleLabel { get; set; }
        public Image TopIcon { get; set; }
        public Image CloseIcon { get; set; }

        private readonly ImageSource _closeIconImage;
        private readonly ImageSource _heroImageDefault;
        private readonly ImageSource _heroImageSucces;

        private View _rootView;

        public TopGroupBehaviour()
        {
            _closeIconImage = ImageSource.FromResource("Companista.Mobile.Services.Authentication.Assets.close_modal.png");
            _heroImageDefault = ImageSource.FromResource("Companista.Mobile.Services.Authentication.Assets.header.png");
            _heroImageSucces = ImageSource.FromResource("Companista.Mobile.Services.Authentication.Assets.verified.png");
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            _rootView = bindable;
        }

        public async Task ViewStart(int delay = 350)
        {
            CloseIcon.Opacity = 0;
            CloseIcon.Source = _closeIconImage;
            TopIcon.Opacity = 0;
            TopIcon.Source = _heroImageDefault;

            await Task.Delay(delay);

            await Task.WhenAll(
                DropInCard(1250),
                CloseIcon.FadeTo(1, 1000)
            );
        }

        public async Task SwitchStateToDefault()
        {
            Title.Text = "Authenticate";

            await FlipImageTo(_heroImageDefault);
        }

        public async Task SwitchStateToBusy()
        {
            Title.Text = "Authenticating…";

            if (TopIcon.Source != _heroImageDefault)
                await FlipImageTo(_heroImageDefault);
        }

        public async Task SwitchStateToSuccess()
        {
            Title.Text = "Authenticated!";

            await Task.WhenAll(
                FlipImageTo(_heroImageSucces),
                FillScreen()
            );

            await FlipImageTo(_heroImageSucces);
        }

        private async Task FlipImageTo(ImageSource newImage)
        {
            if (TopIcon.Source == newImage)
                return;

            await Task.WhenAll(
                TopIcon.RotateYTo(90, 250, Easing.SinIn),
                TopIcon.FadeTo(0, 150)
            );

            TopIcon.Source = newImage;
            TopIcon.RotationY = -90;

            await Task.WhenAll(
                TopIcon.RotateYTo(0, 250, Easing.SinIn),
                TopIcon.FadeTo(100, 150)
            );
        }

        private async Task DropInCard(uint duration)
        {
            TopIcon.Rotation = 100;
            TopIcon.RotationX = 120;
            TopIcon.Opacity = 0;

            var durationFade = (uint)(duration * 0.75);
            var durationXRotation = (uint)(duration * 0.7);

            await Task.WhenAll(
                TopIcon.FadeTo(1, durationFade),
                TopIcon.RotateTo(0, duration, Easing.SpringOut),
                TopIcon.RotateXTo(0, durationXRotation, Easing.SpringOut)
                );
        }

        private async Task FillScreen()
        {
            var width = Application.Current.MainPage.Width;
            var height = Application.Current.MainPage.Height;

            await Task.WhenAll(
                Title.TranslateTo(0, 45, 300),
                TopIcon.TranslateTo(0, height / 8, 500),
                TopIcon.ScaleTo(1.5, 500),
                CloseIcon.FadeTo(0, 300),
                TitleLabel.FadeTo(0, 300),
                _rootView.LayoutTo(Rectangle.FromLTRB(0, 0, width, height), 800, Easing.SinIn)
            );

            CloseIcon.IsEnabled = false;
        }
    }
}
