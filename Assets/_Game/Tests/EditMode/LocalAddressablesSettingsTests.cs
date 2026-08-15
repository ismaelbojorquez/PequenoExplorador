using NUnit.Framework;
using PequenoExplorador.Editor.BuildTools;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class LocalAddressablesSettingsTests
    {
        [Test]
        public void ProfilesGroupsLabelsAndDependenciesSatisfyLocalOnlyContract()
        {
            Assert.That(LocalAddressablesValidationService.Validate(), Is.Empty);
        }
    }
}
