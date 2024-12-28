using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Solidarity.Application.Common;

/// <summary>
/// Apply EF AutoInclude() to the attributed navigation property
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class AutoIncludeAttribute : Attribute { }

/// <summary>
/// EF convention that enables AutoInclude for all navigation properties that have the <see cref="AutoIncludeAttribute"/>
/// </summary>
/// <inheritdoc/>
public sealed class AutoIncludeAttributeConvention(ProviderConventionSetBuilderDependencies dependencies) : NavigationAttributeConventionBase<AutoIncludeAttribute>(dependencies), INavigationAddedConvention, ISkipNavigationAddedConvention
{
	/// <inheritdoc/>
	public override void ProcessNavigationAdded(IConventionNavigationBuilder navigationBuilder, AutoIncludeAttribute attribute, IConventionContext<IConventionNavigationBuilder> context)
		=> navigationBuilder.AutoInclude(true, true);

	/// <inheritdoc/>
	public override void ProcessSkipNavigationAdded(IConventionSkipNavigationBuilder skipNavigationBuilder, AutoIncludeAttribute attribute, IConventionContext<IConventionSkipNavigationBuilder> context)
		=> skipNavigationBuilder.AutoInclude(true, true);
}