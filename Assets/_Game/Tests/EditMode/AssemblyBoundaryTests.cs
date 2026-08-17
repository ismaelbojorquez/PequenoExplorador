using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PequenoExplorador.Editor;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class AssemblyBoundaryTests
    {
        [Test]
        public void ProjectAssemblyGraphSatisfiesAllBoundaryRules()
        {
            IReadOnlyList<AssemblyDefinitionSnapshot> definitions =
                AssemblyDefinitionLoader.LoadProjectDefinitions();

            IReadOnlyList<string> violations = AssemblyBoundaryRules.Validate(definitions);

            Assert.That(violations, Is.Empty, string.Join("\n", violations));
            Assert.That(definitions.Count, Is.EqualTo(10));
        }

        [Test]
        public void PresentationCannotReferenceInfrastructure()
        {
            List<AssemblyDefinitionSnapshot> fixture = AssemblyDefinitionLoader
                .LoadProjectDefinitions()
                .ToList();
            int presentationIndex = fixture.FindIndex(
                item => item.Name == "PequenoExplorador.Presentation");
            fixture[presentationIndex] = fixture[presentationIndex].WithReferences(new[]
            {
                "PequenoExplorador.Application",
                "PequenoExplorador.Infrastructure"
            });

            IReadOnlyList<string> violations = AssemblyBoundaryRules.Validate(fixture);

            Assert.That(
                violations.Any(message => message.StartsWith("ARCH004 PequenoExplorador.Presentation")),
                Is.True,
                "The controlled invalid fixture must be rejected without editing a real asmdef.");
        }

        [Test]
        public void CycleInControlledFixtureIsRejected()
        {
            List<AssemblyDefinitionSnapshot> fixture = AssemblyDefinitionLoader
                .LoadProjectDefinitions()
                .ToList();
            int domainIndex = fixture.FindIndex(item => item.Name == "PequenoExplorador.Domain");
            fixture[domainIndex] = fixture[domainIndex].WithReferences(new[]
            {
                "PequenoExplorador.Application"
            });

            IReadOnlyList<string> violations = AssemblyBoundaryRules.Validate(fixture);

            Assert.That(
                violations.Any(message => message.StartsWith("ARCH011 cyclic dependency")),
                Is.True);
        }
    }
}
