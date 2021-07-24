using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solidarity.Core.Application
{
	public static class DatabaseExtensions
	{
		public static IQueryable<T> IncludeAll<T>(this DbSet<T> dbSet) where T : class
		{
			var query = dbSet.AsQueryable();

			var navigations = dbSet.GetNavigationProperties();

			foreach (var propertyName in navigations)
			{
				query = query.Include(propertyName);
			}

			return query;
		}

		private static IEnumerable<string> GetNavigationProperties<T>(this DbSet<T> dbSet, int maxDepth = int.MaxValue) where T : class
		{
			if (maxDepth < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(maxDepth));
			}

			var entityType = dbSet.EntityType;
			var includedNavigations = new HashSet<INavigation>();
			var stack = new Stack<IEnumerator<INavigation>>();
			while (true)
			{
				var entityNavigations = new List<INavigation>();
				if (stack.Count <= maxDepth)
				{
					foreach (var navigation in entityType.GetNavigations())
					{
						if (includedNavigations.Add(navigation))
						{
							entityNavigations.Add(navigation);
						}
					}
				}
				if (entityNavigations.Count == 0)
				{
					if (stack.Count > 0)
					{
						yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
					}
				}
				else
				{
					foreach (var navigation in entityNavigations)
					{
						var inverseNavigation = navigation.Inverse;
						if (inverseNavigation != null)
						{
							includedNavigations.Add(inverseNavigation);
						}
					}
					stack.Push(entityNavigations.GetEnumerator());
				}
				while (stack.Count > 0 && !stack.Peek().MoveNext())
				{
					stack.Pop();
				}

				if (stack.Count == 0)
				{
					break;
				}

				entityType = stack.Peek().Current.TargetEntityType;
			}
		}
	}
}