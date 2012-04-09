﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace AppHarbor
{
	public class AliasMatcher : IAliasMatcher
	{
		private readonly IEnumerable<Type> _candidateTypes;

		public AliasMatcher(IEnumerable<Type> candidateTypes)
		{
			_candidateTypes = candidateTypes;
		}

		public Type GetMatchedType(string commandArgument)
		{
			try
			{
				return _candidateTypes.Single(
					x => x.GetCustomAttributes(true).OfType<CommandHelpAttribute>().Single().Alias.ToLower() == commandArgument.ToLower());
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException("Command doesn't match any command alias");
			}
		}
	}
}