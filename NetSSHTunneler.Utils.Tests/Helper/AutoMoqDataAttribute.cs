using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace NetSSHTunneler.Utils.Tests.Helper
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        private static IFixture FixtureFactory()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }

        public AutoMoqDataAttribute() : base(FixtureFactory){ }
    }
}
