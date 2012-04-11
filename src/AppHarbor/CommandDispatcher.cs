﻿using System;
using System.Linq;
using Castle.MicroKernel;

namespace AppHarbor
{
	public class CommandDispatcher
	{
		private readonly IAliasMatcher _aliasMatcher;
		private readonly ITypeNameMatcher _typeNameMatcher;
		private readonly IKernel _kernel;

		public CommandDispatcher(IAliasMatcher aliasMatcher, ITypeNameMatcher typeNameMatcher, IKernel kernel)
		{
			_aliasMatcher = aliasMatcher;
			_typeNameMatcher = typeNameMatcher;
			_kernel = kernel;
		}

		public void Dispatch(string[] args)
		{
			var commandArgument = args.Any() ? string.Concat(args.Skip(1).FirstOrDefault(), args[0]) : "help";
			Type matchingType = null;
			if (_typeNameMatcher.IsSatisfiedBy(commandArgument))
			{
				matchingType = _typeNameMatcher.GetMatchedType(commandArgument);
			}

			var command = (ICommand)_kernel.Resolve(matchingType);

			try
			{
				command.Execute(args.Skip(2).ToArray());
			}
			catch (CommandException exception)
			{
				Console.WriteLine(string.Format("Error: {0}", exception.Message));
			}
		}
	}
}
