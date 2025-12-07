using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;

namespace Monaco.Template.Backend.ArchitectureTests.Extensions;

public static class ArchUnitExtensions
{
	extension(IType type)
	{
		public bool IsNestedWithin(params IType[] types) =>
			types.Any(t => type.FullName.StartsWith($"{t.FullName}+"));

		public IType? NestType(Architecture architecture) =>
			architecture.Types
						.SingleOrDefault(t => type.IsNestedWithin(t));
	}

	extension(ClassesShould should)
	{
		public ClassesShouldConjunction HavePropertySetterWithVisibility(params Visibility[] visibility) =>
			should.FollowCustomCondition(c => c.GetPropertyMembers()
											   .Any(p => visibility.Contains(p.SetterVisibility)),
										 $"have properties setters with visibility {string.Join(", ", visibility)}",
										 $"does not have a property setter with visibility {string.Join(", ", visibility)}");

		public ClassesShouldConjunction NotHavePropertySetterWithVisibility(params Visibility[] visibility) =>
			should.FollowCustomCondition(c => c.GetPropertyMembers()
											   .All(p => !visibility.Contains(p.SetterVisibility)),
										 $"not have properties setters with visibility {string.Join(", ", visibility)}",
										 $"has property setter with visibility {string.Join(", ", visibility)}");
	}
}